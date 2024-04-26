using System.Data;
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
    public class UserController : ControllerBase
    {
        private readonly IDataRepository<User> _userRepository;
        private readonly IDataRepository<IdentityCard> _identityCardRepository;
        private readonly IDataRepository<Department> _departmentRepository;

        public UserController(
            IDataRepository<User> userRepository, 
            IDataRepository<IdentityCard> identityCardRepository,
            IDataRepository<Department> departmentRepository
            )
        {
            _userRepository = userRepository;
            _identityCardRepository = identityCardRepository;
            _departmentRepository = departmentRepository;
        }


        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found");
                 }
            return Ok(users);
        }
        


  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserWithDepartment(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var department = await _departmentRepository.GetByIdAsync(user.DepartmentId);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            user.Department = department;

            return Ok(user);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                await _userRepository.DeleteAsync(user);
                await _userRepository.Save();
                return NoContent(); // Successful response with no content, indicating the deletion was successful
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }
/*
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (userDto == null || id != userDto.Id)
            {
                return BadRequest("User data is invalid");
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Map the DTO to your domain model
            existingUser.Name = userDto.Name;
            existingUser.Email = userDto.Email;
            
         
           
            // Add other fields as necessary

            try
            {
                _userRepository.UpdateAsync(existingUser);
                await _userRepository.Save();
                return NoContent(); // Or return Ok if you prefer to return the updated object
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }
        }*/


    }
}
