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

        public UserController(
            IDataRepository<User> userRepository, 
            IDataRepository<IdentityCard> identityCardRepository
            )
        {
            _userRepository = userRepository;
            _identityCardRepository = identityCardRepository;
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

        [HttpGet("seeker/{id}")]
        public async Task<IActionResult> Getseeker(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Convert profile picture byte array to base64 string
            var profilePicBase64 = Convert.ToBase64String(user.ProfilePic);

            var userDto = new
            {
                Name = user.Name,
                Email = user.Email,
                Skills = user.Skills,
                ProfilePic = profilePicBase64, // Use the base64-encoded string
                Age = user.Age,
                DescriptionBio = user.DescriptionBio,
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

            // Update only the name, email, company description, and contact info fields of the existing user
            existingUser.Name = userDto.Name;
            existingUser.Email = userDto.Email;
            existingUser.CompanyDescription = userDto.CompanyDescription;
            existingUser.ContactInfo = userDto.ContactInfo;

            // Check if the password field in the userDto is not null, indicating a password update
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                // Use the CreatePasswordHash method from AuthRepository to hash the password
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);
                existingUser.PasswordHash = passwordHash;
                existingUser.PasswordSalt = passwordSalt;
            }

            // Check if ProfilePic is not null, indicating a profile picture update
            if (userDto.ProfilePic != null)
            {
                // Convert the uploaded file to a byte array and assign it to ProfilePic
                using (var ms = new MemoryStream())
                {
                    await userDto.ProfilePic.CopyToAsync(ms);
                    existingUser.ProfilePic = ms.ToArray();
                }
            }

            // Save the changes to the database
            try
            {
                await _userRepository.Save();
                return NoContent(); // Or return Ok if you prefer to return the updated object
            }
            catch (Exception ex)
            {
                // Log the exception if necessary and return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }



    }
}
