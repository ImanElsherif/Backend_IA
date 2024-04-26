using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }

        public IdentityCard IdentityCard { get; set; }

        public ICollection<StudentCourse> StudentCourse { get; set; }

    }
}
