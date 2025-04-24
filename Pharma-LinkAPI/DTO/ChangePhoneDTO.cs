using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class ChangePhoneDTO
    {
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string? NewPhone { get; set; }

    }
}
