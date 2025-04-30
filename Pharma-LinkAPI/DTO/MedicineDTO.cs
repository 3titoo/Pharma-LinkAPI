using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class MedicineDTO
    {
        [Required]
        public int ID { get; set; }
        public int? Company_Id { get; set; }

        [Required]
        public string? Image_URL { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "InStock quantity cannot be negative")]
        public int? InStock { get; set; }
    }
}
