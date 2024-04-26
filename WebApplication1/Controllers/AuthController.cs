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
            var departments = await _departmentRepository.GetAllAsync();
            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found");
            }

            var userTypes = await _userTypeRepository.GetAllAsync();
            if (userTypes == null || !userTypes.Any())
            {
                return NotFound("No user types found");
            }

            return Ok(new
            {
                departments = departments,
                roles = userTypes
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userForRegisterDto)
        {

            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();
            if (await _repo.UserExist(userForRegisterDto.Email))
            {
                return BadRequest("Email already exists");
            }

            var userToCreate = new User()
            {
                Name = userForRegisterDto.Name,
                Email = userForRegisterDto.Email,
                DepartmentId = userForRegisterDto.DepartmentId,
                UserTypeId = userForRegisterDto.UserTypeId,
            };

            var identityCard = new IdentityCard()
            {
                Code = userForRegisterDto.Code,
                Gender = userForRegisterDto.Gender,
            };
            userToCreate.IdentityCard = identityCard;

            await _repo.Register(userToCreate, userForRegisterDto.Password);
            
            return Ok();

        }
    }
}
