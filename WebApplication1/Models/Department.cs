using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> User { get; set; }
    }
}
