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
            .Include(c => c.CartItems).ThenInclude(ci => ci.Medicine)
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
                CartItemId = ci.CartItemId,
                MedicineId = ci.MedicineId.Value,
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

        if (user == null)
        {
            return BadRequest("user should log in");
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Medicine).ThenInclude(m => m.Company)
            .FirstOrDefaultAsync(c => c.PharmacyId == user.Id);


        if (cart.CartItems != null && cart.CartItems.Count > 0)
        {
            var company = cart.CartItems.FirstOrDefault().Medicine.Company;

            var compMedicine = await _context.Medicines
                .Include(m => m.Company).Where(d => dto.Id == d.ID).FirstOrDefaultAsync();

            if (company.Id != compMedicine.Company_Id)
            {
                return BadRequest($"You can only add medicines from the same company ({company.Name}) to the cart.");
            }
        }


        if (cart == null)
            return NotFound("Cart not found.");

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MedicineId == dto.Id);
        if (existingItem != null)
        {
            if(dto.Count + existingItem.Count > existingItem.Medicine.InStock)
            {
                return BadRequest("Not enough stock available.");
            }
            existingItem.Count += dto.Count;
        }
        else
        {
            var medicine = await _context.Medicines.FindAsync(dto.Id);
            if(medicine == null)
            {
                return BadRequest("medicine not found");
            }
            cart.CartItems.Add(new CartItem
            {
                MedicineId = dto.Id,
                Count = dto.Count,
                UnitPrice = medicine.Price.Value,
            });
        }

        await _context.SaveChangesAsync();
        return Ok("Item added to cart.");
    }


    [HttpPut("UpdateCartItem")]
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
            .Include(c => c.CartItems).ThenInclude(ci => ci.Medicine)
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

        if (dto.Count > cartItem.Medicine.InStock)
        {
            return BadRequest("Not enough stock available.");
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

    [HttpGet("Summary")]
    public async Task<ActionResult<SummaryDTO>> GetCartSummary()
    {
        var user = await _accountRepositry.GetCurrentUser(User);
        var cartId = user.Cart.CartId;
        var cart = await _context.Carts.Include(ph=>ph.Pharmacy)
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Medicine).ThenInclude(m => m.Company)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart.CartItems == null || cart.CartItems.Count == 0)
            return NotFound("Cart items not found.");
        var totalPrice = cart.CartItems.Sum(ci => ci.Count * ci.UnitPrice);

        var company = cart.CartItems.FirstOrDefault().Medicine.Company;
        //if (totalPrice < company.MinPriceToMakeOrder)
        //{
        //    return BadRequest($"Total price must be at least {company.MinPriceToMakeOrder} to make an order for {company.Name}.");
        //} 


        var summary = new SummaryDTO
        {
            PharmacyName = cart.Pharmacy.UserName,
            PharmacyAddress = cart.Pharmacy.City + ", " + cart.Pharmacy.State + ", " + cart.Pharmacy.Street,
            PharmacyPhone = cart.Pharmacy.PhoneNumber,
            PharmacyEmail = cart.Pharmacy.Email,
            TotalCartPrice = totalPrice,
            CartId = cart.CartId,
            CompanyId = company.Id, // company ID from the first medicine in the cart
            MinPriceToMakeOrder = company.MinPriceToMakeOrder,

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
