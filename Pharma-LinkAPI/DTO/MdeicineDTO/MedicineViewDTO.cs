namespace Pharma_LinkAPI.DTO.MdeicineDTO
{
    public class MedicineViewDTO
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? InStock { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public string? ImageUrl { get; set; }

        public int NumberOfPages { get; set; } = 1;
    }
}
