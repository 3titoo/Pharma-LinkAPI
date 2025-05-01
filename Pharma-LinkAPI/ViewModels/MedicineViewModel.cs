namespace Pharma_LinkAPI.ViewModels
{
    public class MedicineViewModel
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? InStock { get; set; }
        public string? CompanyName { get; set; }

        public string? ImageUrl { get; set; }

    }
}
