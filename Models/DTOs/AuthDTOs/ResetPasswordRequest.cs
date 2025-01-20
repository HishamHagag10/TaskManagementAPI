using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.AuthDTOs
{
    public class ResetPasswordRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
