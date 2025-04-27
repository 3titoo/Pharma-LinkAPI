using Pharma_LinkAPI.Models;

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
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyLicenseNumber { get; set; }
        public string? CompanyImagePath { get; set; }
        public float? CompanyRating { get; set; }
        public List<ReviewViewModel>? Reviews { get; set; }
    }
}
