using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos
{
    public class EmployerDto : UserDto
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyDescription { get; set; }

        [Required]
        public string ContactInfo { get; set; }
    }
}
