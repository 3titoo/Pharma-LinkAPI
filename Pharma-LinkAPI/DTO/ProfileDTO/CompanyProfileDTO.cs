﻿namespace Pharma_LinkAPI.DTO.ProfileDTO
{
    public class ReviewsDTO
    {
        public int Id { get; set; }
        public string? ReviewerName { get; set; }
        public float Rating { get; set; }
        public string? Comment { get; set; }
    }
    public class CompanyProfileDTO
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

        public string? AboutUs { get; set; }
        public string? Role { get; set; }
        public float CurrentUserReview { get; set; } = 0;

        public decimal MinPriceToOrder { get; set; } = 0;

        public int TotalReviws { get; set; } = 0;

        public List<ReviewsDTO>? Reviews { get; set; } = new List<ReviewsDTO>();


    }
}
