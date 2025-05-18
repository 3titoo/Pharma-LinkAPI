using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.MdeicineDTO
{
    public class editMedicineQuantityDTO
    {
        [Required]
        public int medicineId { get; set; }
        [Required]
        public int quantity { get; set; }
    }
}
