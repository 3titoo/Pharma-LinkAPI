using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO.CartDTO;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class CartRepo : ICartRepositry
    {
        private readonly AppDbContext _context;
        public CartRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCart(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
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

        public async Task<CartViewDTO?> GetCartView(int cartId)
        {
            CartViewDTO? dto = await _context.Carts
                .Where(c => c.CartId == cartId)
                .Select(c => new CartViewDTO
                {
                    CartId = c.CartId,
                    TotalPrice = c.CartItems.Sum(p => p.Count * p.UnitPrice),
                    CartItems = c.CartItems.Select(ci => new CartItemDTO
                    {
                        CartItemId = ci.CartItemId,
                        MedicineId = ci.MedicineId.Value,
                        MedicineName = ci.Medicine.Name,
                        MedicinePrice = ci.UnitPrice.ToString(),
                        MedicineImage = ci.Medicine.Image_URL,
                        Count = ci.Count,
                    }).ToList()
                }).FirstOrDefaultAsync();


            return dto;
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
        }
    }
}
