using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.AuthDTOs
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}
