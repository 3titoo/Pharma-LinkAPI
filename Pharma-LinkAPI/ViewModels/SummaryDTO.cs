namespace Pharma_LinkAPI.ViewModels
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

        public List<SummaryItemDTO> Medicines { get; set; } = new List<SummaryItemDTO>();

    }
}
