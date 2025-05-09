using Pharma_LinkAPI.Identity;

namespace Pharma_LinkAPI.ViewModels
{
    public class PharmacyProfileViewModel
    {
        public int Id { get; set; }
        public string? PharmacyName { get; set; }
        public string? DrName { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? PharmacyPhoneNumber { get; set; }
        public string? PharmacyEmail { get; set; }
        public string? PharmacyLicenseNumber { get; set; }
        public string? PharmacyImagePath { get; set; }
        public string? Role { get; set; } = SD.Role_Pharmacy;
    }
}
