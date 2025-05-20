namespace Pharma_LinkAPI.DTO.InvoicesDTO
{
    public class InvoiceDTO
    {
        public int OrderID { get; set; }
        // Pharmacy information 
        public string? PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }
        // Pharmacy Address information 
        public string? PharmacyStreet { get; set; }
        public string? PharmacyState { get; set; }
        public string? PharmacyCity { get; set; }
        // End Pharmacy Address information
        // End Pharmacy information

        // Company information 
        public string? CompanyName { get; set; }
        public string? CompanyPhone { get; set; }
        // Company Address information 
        public string? CompanyStreet { get; set; }
        public string? CompanyState { get; set; }
        public string? CompanyCity { get; set; }
        // End Company Address information
        // End Company information
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPriceOrder { get; set; }
        public string? StatusOrder { get; set; }
        public ICollection<InvoiceMedicineDTO>? Medicines { get; set; }

    }
}
