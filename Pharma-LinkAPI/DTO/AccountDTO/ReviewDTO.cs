using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class ReviewDTO
    {
        public string? ReviewerName { get; set; }

        public string? ReviewerEmail { get; set; }
        [Required]
        public float? Rating { get; set; }

        public string? Review { get; set; }
    }
}
