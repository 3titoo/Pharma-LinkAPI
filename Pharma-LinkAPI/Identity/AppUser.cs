using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string? LiscnceNumber { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }

        public string? pdfPath { get; set; }




    }
}
