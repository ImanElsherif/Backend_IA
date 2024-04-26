using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(50)]
        //public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; }
        public int UserTypeId { get; set; }
        public string Code { get; set; }
        public int Gender { get; set; }
    }
}
