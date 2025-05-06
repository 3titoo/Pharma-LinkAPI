using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = SD.Role_Pharmacy)]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAccountRepositry _accountRepositry;
    public CartController(AppDbContext context, IAccountRepositry accountRepositry)
    {
        _context = context;
        _accountRepositry = accountRepositry;
    }

    // GET: api/Cart
    [HttpGet]
    public async Task<ActionResult<CartViewModel>> GetCurrentUserCart()
    {
        var user = await _accountRepositry.GetCurrentUser(User);


        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.PharmacyId == user.Id);

        if (cart == null)
        {
            return NotFound("Cart not found for the current user.");
        }

        if(cart.CartItems == null || !cart.CartItems.Any())
        {
            return Ok(new CartViewModel
            {
                CartId = cart.CartId,
                TotalPrice = 0,
                CartItems = new List<CartItemViewModel>()
            });
        }

        var totalPrice = cart.CartItems.Sum(ci => ci.Count * ci.UnitPrice);

        var ret = new CartViewModel
        {
            CartId = cart.CartId,
            TotalPrice = totalPrice,
            CartItems = cart.CartItems.Select(ci => new CartItemViewModel
            {
                MedicineId = ci.CartItemId,
                MedicineName = ci.Medicine.Name,
                MedicineImage = ci.Medicine.Image_URL,
                MedicinePrice = ci.UnitPrice.ToString("c"),
                Count = ci.Count
            }).ToList()
        };
        return Ok(ret);
    }



    [HttpPost("AddToCart")]
    public async Task<IActionResult> AddToCart(CartItemDTO dto)
    {
        if (!ModelState.IsValid)
        {
            string errorMessage = string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest($"Invalid data: {errorMessage}");
        }



        var user = await _accountRepositry.GetCurrentUser(User);

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.PharmacyId == user.Id);

        if (cart == null)
            return NotFound("Cart not found.");

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MedicineId == dto.Id);
        if (existingItem != null)
        {
            existingItem.Count += dto.Count;
        }
        else
        {
            var medicine = await _context.Medicines.FindAsync(dto.Id);
            cart.CartItems.Add(new CartItem
            {
                MedicineId = dto.Id,
                Count = dto.Count,
                UnitPrice = dto.Count * medicine.Price.Value,
            });
        }

        await _context.SaveChangesAsync();
        return Ok("Item added to cart.");
    }


    [HttpPut("UpdateCartItem/{id}")]
    public async Task<IActionResult> UpdateCartItem(CartItemDTO dto)
    {
        if (!ModelState.IsValid)
        {
            string errorMessage = string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest($"Invalid data: {errorMessage}");
        }
        var user = await _accountRepositry.GetCurrentUser(User);
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.PharmacyId == user.Id);

        var cartItem = await _context.CartItems.FindAsync(dto.Id);
        if (cartItem == null)
            return NotFound();

        if(dto.Count <= 0)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok("Item removed from cart.");
        }

        cartItem.Count = dto.Count;
        await _context.SaveChangesAsync();

        return Ok("Quantity updated.");
    }

    [HttpDelete("DeleteCartItem/{id}")]
    public async Task<IActionResult> DeleteCartItem(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null)
            return NotFound();

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return Ok("Item removed from cart.");
    }

    [HttpGet("Summary/{cartId}")]
    public async Task<ActionResult<SummaryDTO>> GetCartSummary(int cartId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Medicine)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart == null)
            return NotFound("Cart not found.");

        var totalPrice = cart.CartItems.Sum(ci => ci.Count * ci.UnitPrice);
        var summary = new SummaryDTO
        {
            PharmacyName = cart.Pharmacy.Name,
            PharmacyAddress = cart.Pharmacy.City + ", " + cart.Pharmacy.State + ", " + cart.Pharmacy.Street,
            PharmacyPhone = cart.Pharmacy.PhoneNumber,
            PharmacyEmail = cart.Pharmacy.Email,
            TotalCartPrice = totalPrice,
            CartId = cart.CartId,
            CompanyId = cart.CartItems.FirstOrDefault().Medicine.ID, // company ID from the first medicine in the cart

            Medicines = cart.CartItems.Select(ci => new SummaryItemDTO
            {
                MedicineId = ci.MedicineId.Value,
                MedicineName = ci.Medicine.Name,
                MedicinePrice = ci.UnitPrice,
                Count = ci.Count,
                TotalItemPrice = ci.Count * ci.UnitPrice
            }).ToList()
        };
        return Ok(summary);
    }
}
