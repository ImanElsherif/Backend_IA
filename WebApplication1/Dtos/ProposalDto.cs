using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
namespace WebApplication1.Dtos
{
    public class ProposalDto
    {
        public int JobId { get; set; }
        public int JobSeekerId { get; set; }
        public int EmployerId { get; set; }
        public IFormFile Attachment { get; set; }
        public string Status { get; set; }
    }
}
