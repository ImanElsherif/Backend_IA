using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }

       
        [ForeignKey("Job")]
        public int JobId { get; set; }
        public Job Job { get; set; } // Navigation property to Job

        
        [ForeignKey("JobSeeker")]
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; } // Navigation property to JobSeeker

        [ForeignKey("Employer")]
        public int EmpId { get; set; }
        public Employer Employer { get; set; } // Navigation property to JobSeeker

        public string Attachment { get; set; }
        public string Status { get; set; }
    }
}
