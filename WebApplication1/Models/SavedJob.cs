using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class SavedJob
    {
        [Key]
        public int SavedJobId { get; set; }

        [ForeignKey("JobSeeker")]
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; } // Navigation property to JobSeeker

        [ForeignKey("Job")]
        public int JobId { get; set; }
        public Job Job { get; set; } // Navigation property to Job
        public DateTime DateSaved { get; set; }
    }
}
