using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class CartItemDTO
    {
        [Required]
        public int Id { get; set; } // medicineId or cartItemId
        public int Count { get; set; }
    }
}
