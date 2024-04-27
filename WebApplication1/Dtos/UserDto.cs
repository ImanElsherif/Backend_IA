using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace WebApplication1.Dtos
{
    public class UserDto
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        public int UserTypeId { get; set; }


        // Employer-specific fields
        // Optional fields do not use the [Required] attribute and are marked as nullable
        public string? CompanyDescription { get; set; }
        public string? ContactInfo { get; set; }

        public string? Skills { get; set; }
        public string? ProfilePic { get; set; }
        public int? Age { get; set; }
        public string? DescriptionBio { get; set; }
        public int Id { get; internal set; }
    }
}
