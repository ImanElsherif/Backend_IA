using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos
{
    public class JobSeekerDto : UserDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Skills { get; set; }


        [Required]
        public IFormFile ProfilePic { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string DescriptionBio { get; set; }
    }
}
