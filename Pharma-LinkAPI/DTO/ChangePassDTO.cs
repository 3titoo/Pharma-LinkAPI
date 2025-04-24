using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class ChangePassDTO
    {
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "min length must be 6")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

    }
}
