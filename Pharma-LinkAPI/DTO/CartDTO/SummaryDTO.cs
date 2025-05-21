namespace Pharma_LinkAPI.DTO.CartDTO
{

    public class SummaryItemDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public decimal MedicinePrice { get; set; }
        public int Count { get; set; }
        public decimal TotalItemPrice { get; set; }
    }
    public class SummaryDTO
    {
        public string PharmacyName { get; set; }
        public string PharmacyAddress { get; set; }
        public string PharmacyPhone { get; set; }
        public string PharmacyEmail { get; set; }
        public decimal TotalCartPrice { get; set; }
        public int CartId { get; set; }
        public int CompanyId { get; set; }
        public string CompayName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public DateTime orderDate { get; set; } = DateTime.Now;
        public decimal MinPriceToMakeOrder { get; set; } = 0;
        public List<SummaryItemDTO> Medicines { get; set; } = new List<SummaryItemDTO>();

    }
}
