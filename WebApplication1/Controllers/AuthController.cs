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
        /*
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
                }*/
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] UserDto adminDto)
        {
            // Normalize email input
            adminDto.Email = adminDto.Email.ToLower();

            // Check for existing email
            if (await _repo.UserExist(adminDto.Email))
            {
                return BadRequest("Email already exists");
            }

            // Validate user type
            var userType = await _userTypeRepository.GetByIdAsync(adminDto.UserTypeId);
            if (userType == null || userType.Role.ToLower() != "admin")
            {
                return BadRequest("Invalid user type for admin.");
            }

            // Prepare for role-based validation
            List<string> validationErrors = new List<string>();

            // Create admin entity
            var admin = new User
            {
                Email = adminDto.Email,
                UserTypeId = adminDto.UserTypeId
            };

            // Register admin
            await _repo.Register(admin, adminDto.Password);

            return StatusCode(201);
        }

        [HttpPost("register-employer")]
public async Task<IActionResult> Register([FromForm] EmployerDto employerDto)
{
    // Normalize email input
    employerDto.Email = employerDto.Email.ToLower();

    // Check for existing email
    if (await _repo.UserExist(employerDto.Email))
    {
        return BadRequest("Email already exists");
    }

    // Validate user type
    var userType = await _userTypeRepository.GetByIdAsync(employerDto.UserTypeId);
    if (userType == null || userType.Role.ToLower() != "employer")
    {
        return BadRequest("Invalid user type.");
    }

    // Prepare for role-based validation
    List<string> validationErrors = new List<string>();

    // Validate employer fields
    if (!ValidateUserForEmployer(employerDto, validationErrors))
    {
        return BadRequest(new { errors = validationErrors });
    }

    // Create employer entity
    var employer = new Employer
    {
        Email = employerDto.Email,
        UserTypeId = employerDto.UserTypeId,
        CompanyName = employerDto.CompanyName,
        CompanyDescription = employerDto.CompanyDescription,
        ContactInfo = employerDto.ContactInfo
    };

    // Register employer
    await _repo.Register(employer, employerDto.Password);

    return StatusCode(201);
}

        [HttpPost("register-jobseeker")]
        public async Task<IActionResult> RegisterJobSeeker([FromForm] JobSeekerDto jobSeekerDto)
        {
            // Normalize email input
            jobSeekerDto.Email = jobSeekerDto.Email.ToLower();

            // Check for existing email
            if (await _repo.UserExist(jobSeekerDto.Email))
            {
                return BadRequest("Email already exists");
            }

            // Validate user type
            var userType = await _userTypeRepository.GetByIdAsync(jobSeekerDto.UserTypeId);
            if (userType == null || userType.Role.ToLower() != "job seeker")
            {
                return BadRequest("Invalid user type.");
            }

            // Prepare for role-based validation
            List<string> validationErrors = new List<string>();

            // Validate job seeker fields
            if (!ValidateUserForJobSeeker(jobSeekerDto, validationErrors))
            {
                return BadRequest(new { errors = validationErrors });
            }

            byte[] profilePicBytes = null;
            string profilePicContentType = null;
            if (jobSeekerDto.ProfilePic != null && jobSeekerDto.ProfilePic.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await jobSeekerDto.ProfilePic.CopyToAsync(ms);
                    profilePicBytes = ms.ToArray();
                    profilePicContentType = jobSeekerDto.ProfilePic.ContentType;
                }
            }

            // Create job seeker entity
            var jobSeeker = new JobSeeker
            {
                Email = jobSeekerDto.Email,
                UserTypeId = jobSeekerDto.UserTypeId,
                Name = jobSeekerDto.Name,
                Skills = jobSeekerDto.Skills,
                ProfilePic = profilePicBytes,
                Age = jobSeekerDto.Age,
                DescriptionBio = jobSeekerDto.DescriptionBio
            };

            // Register job seeker
            await _repo.Register(jobSeeker, jobSeekerDto.Password);

            return StatusCode(201);
        }

        private bool ValidateUserForEmployer(EmployerDto dto, List<string> errors)
{
    if (string.IsNullOrWhiteSpace(dto.CompanyName))
        errors.Add("The CompanyName field is required for employers.");
    if (string.IsNullOrWhiteSpace(dto.CompanyDescription))
        errors.Add("The CompanyDescription field is required for employers.");
    if (string.IsNullOrWhiteSpace(dto.ContactInfo))
        errors.Add("The ContactInfo field is required for employers.");

    return !errors.Any();
}


        private bool ValidateUserForJobSeeker(JobSeekerDto dto, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("The Name field is required for job seekers.");
            if (string.IsNullOrWhiteSpace(dto.Skills))
                errors.Add("The Skills field is required for job seekers.");
            if (string.IsNullOrWhiteSpace(dto.DescriptionBio))
                errors.Add("The DescriptionBio field is required for job seekers.");
            if (dto.Age <= 0)
                errors.Add("A valid Age is required for job seekers.");

            return !errors.Any();
        }


    }
}
