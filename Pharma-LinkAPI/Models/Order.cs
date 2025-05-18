using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharma_LinkAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public DateTime? OrderDate { get; set; }
        public string? StatusOrder { get; set; }
        public decimal? TotalPrice { get; set; }

        [ForeignKey("Pharmacy")]
        public int? PharmacyID { get; set; }
        public virtual AppUser? Pharmacy { get; set; }

        [ForeignKey("Company")]
        public int? CompanyID { get; set; }
        public virtual AppUser? Company { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
