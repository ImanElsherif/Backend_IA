using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace WebApplication1.Dtos
{
    public class JobDto
    {
        public int JobId { get; set; }
        public int EmployerId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public string JobType { get; set; } // Part-time, Full-time, Remote
        public decimal JobBudget { get; set; }
        public DateTime PostCreationDate { get; set; }
        public int NumProposals { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } // Whether accepted or not from admin
    }
}
