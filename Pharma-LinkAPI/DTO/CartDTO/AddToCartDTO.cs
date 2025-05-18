using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.CartDTO
{
    public class AddToCartDTO
    {
        [Required]
        public int Id { get; set; } // medicineId or cartItemId
        public int Count { get; set; }
    }
}
