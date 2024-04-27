using System.Data;
using System.Text;
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

        [HttpGet("employers")]
        public async Task<IActionResult> GetEmployers()
        {
            // You would need to know the ID or criteria that defines an employer
            // Example: assuming 'employer' role is user type id 3
            const int employerTypeId = 5; // This should be dynamically retrieved or configured if possible

            var employers = await _userRepository.GetAllAsync(u => u.UserTypeId == employerTypeId);
            if (employers == null || !employers.Any())
            {
                return NotFound("No employers found.");
            }

            return Ok(employers);
        }




        /*     [HttpGet("{id}")]
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
             }*/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userDto = new
            {
                Name = user.Name,
                Email = user.Email,
                CompanyDescription = user.CompanyDescription, // Assuming user has CompanyDescription property
                ContactInfo = user.ContactInfo // Assuming user has ContactInfo property
            };

            return Ok(userDto);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
        

            // Retrieve the existing user from the repository
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                // If the user with the specified ID doesn't exist, return NotFound
                return NotFound($"User with ID {id} not found.");
            }

            // Update only the name and email fields of the existing user
            existingUser.Name = userDto.Name;
            existingUser.Email = userDto.Email;
            existingUser.CompanyDescription = userDto.CompanyDescription;
            existingUser.ContactInfo = userDto.ContactInfo;

            // Check if the password field in the userDto is not null, indicating a password update
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                // You should implement a method to hash the password before storing it
                // For simplicity, I'm assuming you have a method called HashPassword
                existingUser.PasswordHash = HashPassword(userDto.Password);
            }

            // Explicitly assign the values of other fields from existingUser to userDto

            userDto.Skills = existingUser.Skills;
            userDto.ProfilePic = existingUser.ProfilePic;
            userDto.Age = existingUser.Age;
            userDto.DescriptionBio = existingUser.DescriptionBio;
            userDto.UserTypeId = existingUser.UserTypeId; // Assuming UserTypeId should not be changed

            try
            {
                // Save the changes to the database
                await _userRepository.Save();
                return NoContent(); // Or return Ok if you prefer to return the updated object
            }
            catch (Exception ex)
            {
                // Log the exception if necessary and return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }
        }

        // Example method for hashing password (replace with appropriate implementation)
        private byte[] HashPassword(string password)
        {
            // You should use a secure hashing algorithm like bcrypt
            // For simplicity, I'm just using a basic hashing function
            var sha256 = System.Security.Cryptography.SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }



    }
}
