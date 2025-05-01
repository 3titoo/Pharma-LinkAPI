using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pharma_LinkAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [ForeignKey("Order")]
        public int? OrderID { get; set; }
        [JsonIgnore]
        public virtual Order? Order { get; set; }

        [ForeignKey("Medicine")]
        public int? MedicineID { get; set; }
        public virtual Medicine? Medicine { get; set; }

        public int? Count { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
