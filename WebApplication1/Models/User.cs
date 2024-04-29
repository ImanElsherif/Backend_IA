using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }


        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }



        //emp
        public string? CompanyDescription { get; set; }
        public string? ContactInfo { get; set; }

        //job seeker
        public string? Skills { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string? ProfilePicContentType { get; set; }
        public int? Age { get; set; }
        public string? DescriptionBio { get; set; }

    }
}
