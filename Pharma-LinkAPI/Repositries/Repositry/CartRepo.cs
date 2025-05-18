using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class CartRepo : ICartRepositry
    {
        private readonly AppDbContext _context;
        public CartRepo(AppDbContext context)
        {
            _context = context;
        }
        public void AddCart(Cart cart)
        {
            _context.Carts.Add(cart);
        }

        public async Task DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItem(int Id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == Id);
            return cartItem;
        }

        public async Task<Cart?> GetCart(int cartId)
        {
            var cart = await _context.Carts.Include(ph => ph.Pharmacy)
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Medicine).ThenInclude(m => m.Company)
                        .FirstOrDefaultAsync(c => c.CartId == cartId);
            return cart;
        }

        public async Task<List<CartItem>> GetCartItemsByMedicineId(int medicineId)
        {
            var items = await _context.CartItems.Where(c => c.MedicineId == medicineId).ToListAsync();
            return items;
        }

        public async Task RemoveCartItems(List<CartItem> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                return;
            }
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
