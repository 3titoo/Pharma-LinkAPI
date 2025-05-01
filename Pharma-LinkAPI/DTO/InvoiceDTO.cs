namespace Pharma_LinkAPI.DTO
{
    public class InvoiceDTO
    {
        public string? DRName { get; set; }
        public string? Phone { get; set; } // Pharmacy phone
        public string? PharmacyLicense { get; set; }
        public string? PharmacyName { get; set; }

        // Pharmacy Address information 
         public string? Street { get; set; }
         public string? State { get; set; }
         public string? City { get; set; }
        // End Pharmacy Address information

        public string? CompanyLicense { get; set; }
        public string? CompanyName { get; set; }

        public DateOnly? OrderDate { get; set; }

        public decimal? TotalPriceOrder { get; set; }
        public string? StatusOrder { get; set; }

       public ICollection<InvoiceMedicineDTO>? Medicines { get; set; }

    }
}
