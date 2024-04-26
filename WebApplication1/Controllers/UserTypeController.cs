using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly IDataRepository<UserType> _userTypeRepository;
        public UserTypeController(IDataRepository<UserType> userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserType(UserTypeDto role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            var userType = new UserType()
            {
                Role = role.Role,
            };

            await _userTypeRepository.AddAsync(userType);
            await _userTypeRepository.Save();

            return Ok();
        }
    }
}
