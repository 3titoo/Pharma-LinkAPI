using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class PharmacyRequestDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string? PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Remote(action: "IsEmailInUse", controller: "Account", ErrorMessage = "email is already in use")]

        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
        [Required]
        public string? LicenseNumber { get; set; }

        [Required]
        [Remote(action: "IsUserNameInUse", controller: "Account", ErrorMessage = "User name is already in use")]
        public string? UserName { get; set; }

        [Required]
        public string? DrName { get; set; } 

        [Required]
        public string? pdfURL { get; set; }

        [Required]
        public IFormFile? img { get; set; }

    }
}
