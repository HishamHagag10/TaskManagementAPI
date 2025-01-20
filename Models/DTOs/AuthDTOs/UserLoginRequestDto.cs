using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.AuthDTOs
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
