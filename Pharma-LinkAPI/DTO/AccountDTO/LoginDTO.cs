using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class LoginDTO
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
