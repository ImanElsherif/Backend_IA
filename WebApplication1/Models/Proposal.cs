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
      

        [ForeignKey("JobSeeker")]
        public int JobSeekerId { get; set; }


        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
  

        public string Attachment { get; set; }
        public string Status { get; set; }
    }
}
