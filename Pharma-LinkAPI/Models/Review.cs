using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharma_LinkAPI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? PharmacyId { get; set; }
        [Required]
        public int? CompanyId { get; set; }


        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }



        [ForeignKey("PharmacyId")]
        [ValidateNever]
        public AppUser? pharmacy { get; set; }

        [ForeignKey("CompanyId")]
        [ValidateNever]
        public AppUser? company { get; set; }
    }
}
