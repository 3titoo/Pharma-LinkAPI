using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Pharma_LinkAPI.Models
{
    public class Medicine
    {
        [Required]
        public int ID { get; set; }
        [Required]
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

        [ForeignKey("Company_Id")]
        public  AppUser? Company { get; set; }

    }
}
