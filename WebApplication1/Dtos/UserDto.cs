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
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        public int UserTypeId { get; set; }


    }
}
