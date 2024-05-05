using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class JobSeeker : User
    {
        public string Name { get; set; }
        public string Skills { get; set; }
        public byte[] ProfilePic { get; set; }
        public int Age { get; set; }
        public string DescriptionBio { get; set; }

      
        public User User { get; set; }
        public ICollection<Proposal> Proposals { get; set; }
    }
}
