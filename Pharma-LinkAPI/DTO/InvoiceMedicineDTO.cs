using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class InvoiceMedicineDTO
    {
        public string? Name { get; set; }
        public string? Image_URL { get; set; }        
        public decimal? UnitPrice { get; set; }
        public int? Count { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
