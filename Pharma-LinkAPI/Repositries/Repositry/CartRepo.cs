using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
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
            var cart = await _context.Carts
    .Where(c => c.CartId == cartId)
    .Select(c => new Cart
    {
        CartId = c.CartId,
        Pharmacy = new Identity.AppUser
        {
            UserName = c.Pharmacy.UserName,
            Email = c.Pharmacy.Email,
            PhoneNumber = c.Pharmacy.PhoneNumber,
            City = c.Pharmacy.City,
            State = c.Pharmacy.State,
            Street = c.Pharmacy.Street,
        },
        CartItems = c.CartItems.Select(ci => new CartItem
        {
            CartItemId = ci.CartItemId,
            Count = ci.Count,
            UnitPrice = ci.UnitPrice,
            MedicineId = ci.MedicineId,
            Medicine = new Medicine
            {
                Name = ci.Medicine.Name,
                Image_URL = ci.Medicine.Image_URL,
                InStock = ci.Medicine.InStock,
                Price = ci.Medicine.Price,
                Company = new Identity.AppUser
                {
                    Id = ci.Medicine.Company.Id,
                    Name = ci.Medicine.Company.Name,
                    MinPriceToMakeOrder = ci.Medicine.Company.MinPriceToMakeOrder
                }
            }
        }).ToList()
    })
    .FirstOrDefaultAsync();

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
        }
    }
}
