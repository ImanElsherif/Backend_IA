using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IDataRepository<UserType> _userTypeRepository;

        public AuthController(IAuthRepository repo, IConfiguration config, IDataRepository<UserType> userTypeRepository)
        {
            _config = config;
            _repo = repo;
            _userTypeRepository = userTypeRepository;
      
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Email.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Email, userFromRepo.Email),
                new Claim(ClaimTypes.Name, userFromRepo.Name),
                new Claim(ClaimTypes.Role, userFromRepo.UserType.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
            });
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            var userTypes = await _userTypeRepository.GetAllAsync();
            if (userTypes == null || !userTypes.Any())
            {
                return NotFound("No user types found");
            }

            return Ok(new
            {
                roles = userTypes
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userForRegisterDto)
        {
            // Normalize email input
            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();

            // Check for existing email
            if (await _repo.UserExist(userForRegisterDto.Email))
            {
                return BadRequest("Email already exists");
            }

            // Validate user type and role-specific fields
            var userType = await _userTypeRepository.GetByIdAsync(userForRegisterDto.UserTypeId);
            if (userType == null)
            {
                return BadRequest("Invalid user type.");
            }

            // Prepare for role-based validation
            List<string> validationErrors = new List<string>();
            if (!ValidateUserForRole(userType.Role, userForRegisterDto, validationErrors))
            {
                return BadRequest(new { errors = validationErrors });
            }

            // Process profile picture
            byte[] profilePicBytes = null;
            string profilePicContentType = null;
            if (userForRegisterDto.ProfilePic != null && userForRegisterDto.ProfilePic.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await userForRegisterDto.ProfilePic.CopyToAsync(ms);
                    profilePicBytes = ms.ToArray();
                    profilePicContentType = userForRegisterDto.ProfilePic.ContentType;
                }
            }

            var userToCreate = new User
            {
                Name = userForRegisterDto.Name,
                Email = userForRegisterDto.Email,
                UserTypeId = userForRegisterDto.UserTypeId,
                CompanyDescription = userForRegisterDto.CompanyDescription,
                ContactInfo = userForRegisterDto.ContactInfo,
                Skills = userForRegisterDto.Skills,
                ProfilePic = profilePicBytes,
                ProfilePicContentType = profilePicContentType,
                Age = userForRegisterDto.Age,
                DescriptionBio = userForRegisterDto.DescriptionBio
            };

            await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        private bool ValidateUserForRole(string role, UserDto dto, List<string> errors)
        {
            switch (role.ToLower())
            {
                case "employer":
                    if (string.IsNullOrWhiteSpace(dto.CompanyDescription))
                        errors.Add("The CompanyDescription field is required for employers.");
                    if (string.IsNullOrWhiteSpace(dto.ContactInfo))
                        errors.Add("The ContactInfo field is required for employers.");
                    break;
                case "job seeker":
                    if (string.IsNullOrWhiteSpace(dto.Skills))
                        errors.Add("The Skills field is required for job seekers.");
                    if (string.IsNullOrWhiteSpace(dto.ProfilePic?.FileName)) // Check if ProfilePic is uploaded
                        errors.Add("The ProfilePic field is required for job seekers.");
                    if (string.IsNullOrWhiteSpace(dto.DescriptionBio))
                        errors.Add("The DescriptionBio field is required for job seekers.");
                    if (dto.Age <= 0)
                        errors.Add("A valid Age is required for job seekers.");
                    break;
            }
            return !errors.Any();
        }
    }
}
