using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pharma_LinkAPI.DTO
{
    public class CompanyInvoiceDTO
    {
        public string PharmacyName { get; set; }
        public string DRName { get; set; }
        public string PharmacyPhone { get; set; }
        // Pharmacy Address information 
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        // End Pharmacy Address information
        public DateOnly OrderDate { get; set; }
        public string StatusOrder { get; set; }
        
    }
}
