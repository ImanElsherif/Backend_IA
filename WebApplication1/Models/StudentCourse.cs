using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class StudentCourse
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int Grade { get; set; }

    }
}
