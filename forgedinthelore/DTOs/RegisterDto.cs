using System.ComponentModel.DataAnnotations;

namespace forgedinthelore_net.DTOs;

public class RegisterDto
{
    [Required] public string UserName { get; set; }
    
    [Required]
    [EmailAddress] public string Email { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 4)]
    public string Password { get; set; }
}