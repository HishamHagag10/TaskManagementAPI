using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.AuthDTOs
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
