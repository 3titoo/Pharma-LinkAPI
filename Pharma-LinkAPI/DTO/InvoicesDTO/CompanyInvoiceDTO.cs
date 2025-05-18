namespace Pharma_LinkAPI.DTO.InvoicesDTO
{
    public class CompanyInvoiceDTO
    {
        public int OrderID { get; set; }
        public string PharmacyName { get; set; }

        public string PharmacyUserName { get; set; }

        public string DRName { get; set; }
        public string PharmacyPhone { get; set; }
        // Pharmacy Address information 
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        // End Pharmacy Address information
        public DateTime OrderDate { get; set; }
        public string StatusOrder { get; set; }

    }
}
