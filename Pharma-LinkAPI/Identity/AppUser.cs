using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string? LiscnceNumber { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Name { get; set; } // name of the pharmacy or company
        public string? Role { get; set; }
        public string? pdfPath { get; set; }

        public string? ImagePath { get; set; }

        public string? DrName { get; set; }

        public ICollection<Review>? ReviewsGiven { get; set; } // for pharmacy
        public ICollection<Review>? ReviewsReceived { get; set; } // for company
        public ICollection<Order>? Ordersrequested { get; set; } // for pharmacy
        public ICollection<Order>? OrderReceived { get; set; } // for company
    }
}
