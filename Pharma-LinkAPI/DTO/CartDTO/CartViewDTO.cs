namespace Pharma_LinkAPI.DTO.CartDTO
{

    public class CartItemDTO
    {
        public int CartItemId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }

        public string? MedicineImage { get; set; }

        public string MedicinePrice { get; set; }
        public int Count { get; set; }
    }
    public class CartViewDTO
    {
        public int CartId { get; set; }
        public List<CartItemDTO>? CartItems { get; set; } = new List<CartItemDTO>();
        public decimal TotalPrice { get; set; } = 0;
    }
}
