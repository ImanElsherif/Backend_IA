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
        private readonly IDataRepository<Employer> _employerRepository;
        private readonly IDataRepository<JobSeeker> _jobSeekerRepository;
        public UserController(
            IDataRepository<User> userRepository, 
            IDataRepository<IdentityCard> identityCardRepository,
            IDataRepository<Employer> employerRepository,
            IDataRepository<JobSeeker> jobSeekerRepository
            )
        {
            _userRepository = userRepository;
            _identityCardRepository = identityCardRepository;
            _employerRepository = employerRepository;
            _jobSeekerRepository = jobSeekerRepository;
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
[Authorize(Roles = "Admin")]
[HttpGet("employers")]
public async Task<IActionResult> GetEmployers()
{
    var employers = await _employerRepository.GetAllAsync();

    if (employers == null || !employers.Any())
    {
        return NotFound("No employers found.");
    }

    return Ok(employers);
}

        /*        [HttpGet("{id}")]
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
                }*/

        
        [HttpGet("employer/{id}")]
        public async Task<IActionResult> Getemployer(int id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                return NotFound("Job seeker not found");
            }

      
            var jobSeekerDto = new
            {
                CompanyName = employer.CompanyName,
                Email = employer.Email,
                CompanyDescription = employer.CompanyDescription,
                ContactInfo = employer.ContactInfo,
             
            };

            return Ok(jobSeekerDto);
        }


        [HttpGet("seeker/{id}")]
        public async Task<IActionResult> Getseeker(int id)
        {
            var jobSeeker = await _jobSeekerRepository.GetByIdAsync(id);
            if (jobSeeker == null)
            {
                return NotFound("Job seeker not found");
            }

            // Convert profile picture byte array to base64 string
            var profilePicBase64 = Convert.ToBase64String(jobSeeker.ProfilePic);

            var jobSeekerDto = new
            {
                Name = jobSeeker.Name,
                Email = jobSeeker.Email,
                Skills = jobSeeker.Skills,
                ProfilePic = profilePicBase64, // Use the base64-encoded string
                Age = jobSeeker.Age,
                DescriptionBio = jobSeeker.DescriptionBio,
            };

            return Ok(jobSeekerDto);
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

        [HttpDelete("employer/{id}")]
        public async Task<IActionResult> DeleteEmployer(int id)
        {
            try
            {
                var employer = await _employerRepository.GetByIdAsync(id);
                if (employer == null)
                {
                    return NotFound($"Employer with ID {id} not found.");
                }

                await _employerRepository.DeleteAsync(employer);
                await _employerRepository.Save();
                return NoContent(); // Successful response with no content, indicating the deletion was successful
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting employer");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] EmployerDto userDto)
        {
            // Retrieve the existing user from the repository
            var existingUser = await _employerRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                // If the user with the specified ID doesn't exist, return NotFound
                return NotFound($"User with ID {id} not found.");
            }

            // Update only the name, email, company description, and contact info fields of the existing user
            existingUser.CompanyName = userDto.CompanyName;
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

         /*   // Check if ProfilePic is not null, indicating a profile picture update
            if (userDto.ProfilePic != null)
            {
                // Convert the uploaded file to a byte array and assign it to ProfilePic
                using (var ms = new MemoryStream())
                {
                    await userDto.ProfilePic.CopyToAsync(ms);
                    existingUser.ProfilePic = ms.ToArray();
                }
            }*/

            // Save the changes to the database
            try
            {
                await _employerRepository.Save();
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
