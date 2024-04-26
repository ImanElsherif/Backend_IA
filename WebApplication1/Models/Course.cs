using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<StudentCourse> StudentCourse { get; set; }
    }
}
