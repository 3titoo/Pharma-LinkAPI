using Pharma_LinkAPI.DTO.CartDTO;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface ICartRepositry
    {
        Task<Cart?> GetCart(int cartId);
        Task<CartItem?> GetCartItem(int Id);

        Task DeleteCartItem(CartItem cartItem);

        Task AddCart(Cart cart);

        Task<List<CartItem>> GetCartItemsByMedicineId(int medicineId);

        Task RemoveCartItems(List<CartItem> cartItems);

        Task<CartViewDTO?> GetCartView(int cartId);


    }
}
