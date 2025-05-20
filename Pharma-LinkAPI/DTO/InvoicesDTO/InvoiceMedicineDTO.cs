namespace Pharma_LinkAPI.DTO.InvoicesDTO
{
    public class InvoiceMedicineDTO
    {
        public string? Name { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Count { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
