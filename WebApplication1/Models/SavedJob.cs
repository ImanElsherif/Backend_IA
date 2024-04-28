using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class SavedJob
    {
        [Key]
        public int SavedJobId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Job")]
        public int JobId { get; set; }

        public DateTime DateSaved { get; set; }
    }
}
