using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class SearchDTO
    {
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public decimal? Price { get; set; }
        public int? InStock { get; set; }
        public string? img { get; set; }
    }
}
