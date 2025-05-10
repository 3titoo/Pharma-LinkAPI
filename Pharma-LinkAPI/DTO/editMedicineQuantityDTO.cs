using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class editMedicineQuantityDTO
    {
        [Required]
        public int medicineId { get; set; }
        [Required]
        public int quantity { get; set; }
    }
}
