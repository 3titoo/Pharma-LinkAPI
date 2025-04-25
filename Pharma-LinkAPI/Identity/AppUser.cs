using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string? LiscnceNumber { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? pdfPath { get; set; }

        public ICollection<Review>? ReviewsGiven { get; set; } // for pharmacy
        public ICollection<Review>? ReviewsReceived { get; set; } // for company
    }
}
