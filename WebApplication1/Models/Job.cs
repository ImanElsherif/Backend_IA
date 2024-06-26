using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        public ICollection<Proposal> Proposals { get; set; }
        public ICollection<SavedJob> SavedJobs { get; set; }
        public int EmployerId { get; set; }
        
        [ForeignKey("EmployerId")]
        public Employer Employer { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string JobDescription { get; set; }

        [Required]
        public string JobType { get; set; } // E.g., "Part-time", "Full-time", "Remote"

        public decimal JobBudget { get; set; }

        public DateTime PostCreationDate { get; set; }

        public int NumProposals { get; set; }

        public string Location { get; set; }

        [Required]
        public string Status { get; set; } // E.g., "Accepted", "Pending"
    }
}
