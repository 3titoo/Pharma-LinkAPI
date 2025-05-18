using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class ChangeMinPriceDTO
    {
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "should be positive")]
        public decimal MinPriceToOrder { get; set; } = 0;

    }
}
