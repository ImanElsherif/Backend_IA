using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class IdentityCard
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }
        public string Code { get; set; }
        public int Gender { get; set; }
        public User User { get; set; }
    }

}
