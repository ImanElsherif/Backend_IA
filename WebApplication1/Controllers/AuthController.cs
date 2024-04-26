using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IDataRepository<Department> _departmentRepository;

        public AuthController(IAuthRepository repo, IConfiguration config, IDataRepository<UserType> userTypeRepository, IDataRepository<Department> departmentRepository)
        {
            _config = config;
            _repo = repo;
            _userTypeRepository = userTypeRepository;
            _departmentRepository = departmentRepository;
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

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

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
         /*   var departments = await _departmentRepository.GetAllAsync();
            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found");
            }*/

            var userTypes = await _userTypeRepository.GetAllAsync();
            if (userTypes == null || !userTypes.Any())
            {
                return NotFound("No user types found");
            }

            return Ok(new
            {
              /*  departments = departments,*/
                roles = userTypes
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userForRegisterDto)
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

            var userToCreate = new User
            {
                Name = userForRegisterDto.Name,
                Email = userForRegisterDto.Email,
                UserTypeId = userForRegisterDto.UserTypeId,
                // Initialize fields that may be assigned conditionally
                CompanyDescription = userType.Role == "employer" ? userForRegisterDto.CompanyDescription : null,
                ContactInfo = userType.Role == "employer" ? userForRegisterDto.ContactInfo : null,
                Skills = userType.Role == "job seeker" ? userForRegisterDto.Skills : null,
                ProfilePic = userType.Role == "job seeker" ? userForRegisterDto.ProfilePic : null,
                Age = userType.Role == "job seeker" ? userForRegisterDto.Age : 0,
                DescriptionBio = userType.Role == "job seeker" ? userForRegisterDto.DescriptionBio : null,
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
                    if (string.IsNullOrWhiteSpace(dto.ProfilePic))
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
