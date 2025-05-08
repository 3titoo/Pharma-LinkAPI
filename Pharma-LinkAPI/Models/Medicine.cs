using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Pharma_LinkAPI.Models
{
    public class Medicine
    {
        [Key]
        public int ID { get; set; }
        public int? Company_Id { get; set; }

        public string? Image_URL { get; set; }

        public string? Description { get; set; }
        public string? Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "InStock quantity cannot be negative")]
        public int? InStock { get; set; }

        [ForeignKey("Company_Id")]
        [ValidateNever]
        public  AppUser? Company { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
