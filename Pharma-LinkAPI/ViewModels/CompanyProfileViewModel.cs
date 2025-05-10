namespace Pharma_LinkAPI.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string? ReviewerName { get; set; }
        public float Rating { get; set; }
        public string? Comment { get; set; }
    }
    public class CompanyProfileViewModel
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyLicenseNumber { get; set; }
        public string? CompanyImagePath { get; set; }
        public float? CompanyRating { get; set; }

        public string? Role { get; set; }
        public float CurrentUserReview { get; set; } = 0;

        public decimal MinPriceToOrder { get; set; } = 0;

        public int TotalReviws { get; set; } = 0;

        public List<ReviewViewModel>? Reviews { get; set; } = new List<ReviewViewModel>();


    }
}
