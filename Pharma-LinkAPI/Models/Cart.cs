using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharma_LinkAPI
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public decimal? TotalPrice { get; set; }

        // علاقة: الكارت مرتبط بواحدة صيدلية (اختياري حسب المشروع)
        public int? PharmacyId { get; set; }

        [ForeignKey("PharmacyId")]
        public AppUser? Pharmacy { get; set; }

        // كارت يحتوي على عناصر (CartItems)
        public ICollection<CartItem>? CartItems { get; set; }
    }
}