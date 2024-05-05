

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Employer : User
    {
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public string ContactInfo { get; set; }

        public User User { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Proposal> Proposals { get; set; }
    }
}
