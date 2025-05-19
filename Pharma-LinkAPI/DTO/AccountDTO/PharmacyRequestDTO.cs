using Pharma_LinkAPI.Services.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO.AccountDTO
{
    public class PharmacyRequestDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Street { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string? PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [IsEmailInUse]

        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s])(?!.*\s).{8,}$",
    ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل وتحتوي على حرف كبير وصغير ورقم ورمز بدون مسافات.")]
        public string? Password { get; set; }
        [Required]
        public string? LicenseNumber { get; set; }

        [Required]
        [IsUserNameInUse]
        public string? UserName { get; set; }

        [Required]
        public string? DrName { get; set; }

        [Required]
        public string? pdfURL { get; set; }

        //[Required]
        //public IFormFile? img { get; set; }

    }
}
