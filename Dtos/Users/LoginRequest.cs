using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Dtos.Users
{
    public class LoginRequest
    {
      
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        
    }
}