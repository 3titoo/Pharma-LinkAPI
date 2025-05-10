namespace Pharma_LinkAPI.DTO
{
    public class PharmacyInvoiceDTO
    {
        public int OrderID { get; set; }
        public string CompanyName { get; set; }

        public string CompanyUserName { get; set; }

        // Company Address information 
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        // End Company Address information
        public DateOnly OrderDate { get; set; }
        public string StatusOrder { get; set; }
    }
}
