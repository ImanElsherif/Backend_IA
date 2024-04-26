using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDataRepository<Department> _departmentRepository;
        private readonly IDataRepository<User> _userRepository;
        
        public DepartmentController(IDataRepository<Department> departmentRepository, IDataRepository<User> userRepository)
        {
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
        }

        //[Authorize]
        [HttpPost("create")] // localhost:7000/api/department/create
        public async Task<IActionResult> CreateDepartment(DepartmentDto dep)
        {
            if (dep == null)
            {
                return BadRequest();
            }

            var department = new Department()
            {
                Name = dep.Name,
            };

            await _departmentRepository.AddAsync(department);
            await _departmentRepository.Save();

            return Ok();
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartmentsWithUsers()
        {
            var departments = await _departmentRepository.GetAllAsync();
            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found");
            }

            foreach (var department in departments)
            {
                department.User = (await _userRepository.GetAllAsync()).ToList();
            }

            return Ok(departments);
        }
    }
}
