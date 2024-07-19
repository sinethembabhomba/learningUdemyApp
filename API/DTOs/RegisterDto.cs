using System.ComponentModel.DataAnnotations;
namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string KnownAs { get; set; }


        public string Gender { get; set; }


        public DateOnly? DateOfBirth { get; set; }


        public string City { get; set; }


        public string Country { get; set; }

        [Required]
        public string Password { get; set; }
    }
}