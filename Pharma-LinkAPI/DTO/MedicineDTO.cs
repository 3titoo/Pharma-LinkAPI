using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class MedicineDTO
    {


        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "InStock quantity cannot be negative")]
        public int? InStock { get; set; }

        [Required]
        public IFormFile? Image { get; set; }

    }
}
