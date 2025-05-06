namespace Pharma_LinkAPI.ViewModels
{

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }

        public string? MedicineImage { get; set; }

        public string MedicinePrice { get; set; }
        public int Count { get; set; }
    }
    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel>? CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; } = 0;
    }
}
