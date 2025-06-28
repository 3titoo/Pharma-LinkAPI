namespace Pharma_LinkAPI.DTO.MdeicineDTO
{
    public class GetAllViewDTO
    {
        public IEnumerable<MedicineViewDTO>? medicines { get; set; }
        public int TotalCount { get; set; }
    }
}
