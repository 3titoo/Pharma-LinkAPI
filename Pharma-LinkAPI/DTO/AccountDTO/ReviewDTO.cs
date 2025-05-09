using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class ReviewDTO
    {
        //public string? ReviewerName { get; set; }

        //public string? ReviewerEmail { get; set; }
        [Required]
        [Range(0,5, ErrorMessage = "Rating must be between 0 and 5.")]
        public float? Rating { get; set; }

        public string? Review { get; set; }
    }
}
