using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.AuthDTOs
{
    public class EmailRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
