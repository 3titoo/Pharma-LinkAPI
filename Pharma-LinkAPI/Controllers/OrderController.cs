using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Transactions;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.AspNetCore.Authorization;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAccountRepositry _account;
        private readonly IOrderRepositry _orderRepositry;
        private readonly ImedicineRepositiry _medicineRepositiry;
        public OrderController(AppDbContext context, IAccountRepositry account, IOrderRepositry orderRepositry, ImedicineRepositiry medicineRepositiry)
        {
            _context = context;
            _account = account;
            _orderRepositry = orderRepositry;
            _medicineRepositiry = medicineRepositiry;
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpGet("IndexCompanyOrder")]
        public async Task<ActionResult<IEnumerable<CompanyInvoiceDTO>>> IndexCompanyOrder()
        {
            var currentUser = await _account.GetCurrentUser(User);
            var CompanyId = currentUser.Id;

            var orders = await _orderRepositry.GetAllOrdersForCompany(CompanyId);

            if (orders == null)
            {
                return NotFound("Orders for Company not found.");
            }

            var CompanyInvoices = new List<CompanyInvoiceDTO>();
            foreach (var item in orders)
            {
                if (item == null)
                {
                    return NotFound("Order not found.");
                }

                var TempCompanyInvoice = new CompanyInvoiceDTO
                {
                    OrderID = item.OrderID,
                    PharmacyName = item.Pharmacy.Name,
                    PharmacyUserName = item.Pharmacy.UserName,
                    DRName = item.Pharmacy.DrName,
                    PharmacyPhone = item.Pharmacy.PhoneNumber,
                    Street = item.Pharmacy.Street,
                    State = item.Pharmacy.State,
                    City = item.Pharmacy.City,
                    OrderDate = (DateOnly)item.OrderDate,
                    StatusOrder = item.StatusOrder
                };
                CompanyInvoices.Add(TempCompanyInvoice);
            }
            return Ok(CompanyInvoices);

        }

        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpGet("IndexPharmacyOrder")]
        public async Task<ActionResult<IEnumerable<PharmacyInvoiceDTO>>> IndexPharmacyOrder()
        {
            var currentUser = await _account.GetCurrentUser(User);

            if (currentUser == null)
            {
                return NotFound("currentUser not found.");
            }

            var PharmacyId = currentUser.Id;

            var orders = await _orderRepositry.GetAllOrdersForPharmacy(PharmacyId);

            if (orders == null)
            {
                return NotFound("Orders for Pharmacy not found.");
            }

            var PharmacyInvoices = new List<PharmacyInvoiceDTO>();
            foreach (var item in orders)
            {
                if (item == null)
                {
                    return NotFound("Order not found.");
                }

                var TempPharmacyInvoice = new PharmacyInvoiceDTO
                {
                    OrderID = item.OrderID,
                    CompanyName = item.Company.Name,
                    CompanyUserName = item.Company.UserName,
                    Street = item.Company.Street,
                    State = item.Company.State,
                    City = item.Company.City,
                    OrderDate = (DateOnly)item.OrderDate,
                    StatusOrder = item.StatusOrder
                };
                PharmacyInvoices.Add(TempPharmacyInvoice);
            }
            return Ok(PharmacyInvoices);

        }

        [Authorize]
        [HttpGet("Invoice/{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int OrderId)
        {
            var order = await _orderRepositry.GetOrderById(OrderId);
            
            if(order == null)
            {
                return NotFound("Orders not found.");
            }

            var currentUser = await _account.GetCurrentUser(User);

            if(currentUser == null)
            {
                return NotFound("currentUser not found.");
            }

            if (currentUser.Id != order.CompanyID && currentUser.Id != order.PharmacyID)
            {
                return Problem("You are not authorized to view this order.");
            }      

            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                return NotFound("Order items not found.");
            }

            if (order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            var Invoice = new InvoiceDTO
            {
                OrderID = OrderId,
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = order.Company.Name,
                CompanyLicense = order.Company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = order.OrderDate,
                StatusOrder = order.StatusOrder,
                TotalPriceOrder = order.TotalPrice,
                Medicines = new List<InvoiceMedicineDTO>()
            };
            foreach (var item in order.OrderItems)
            {
                var InvoiceMedicineDTO = new InvoiceMedicineDTO
                {
                    Name = item.MedicineName,
                    Image_URL = item.MedicineImage,
                    UnitPrice = item.UnitPrice,
                    Count = item.Count,
                    TotalPrice = item.TotalPrice
                };
                Invoice.Medicines.Add(InvoiceMedicineDTO);
            }
            return Ok(Invoice);
        }

        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpPost("PlaceOrder/{CartId:int}/{companyId:int}")]
        public async Task<ActionResult> PlaceOrder(int CartId, int companyId)
        {

            using var transaction = await _context.Database.BeginTransactionAsync(); // Begin transaction

            try
            {
                var CurrentCart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(c => c.Medicine)
                    .ThenInclude(m => m.Company)
                    .FirstOrDefaultAsync(c => c.CartId == CartId);

                if (CurrentCart == null)
                {
                    return NotFound("Cart not found.");
                }

                if (CurrentCart.CartItems == null || CurrentCart.CartItems.Count == 0)
                {
                    return NotFound("Cart items not found.");
                }




                var MedicinesForCompany = await _medicineRepositiry.GetMedicinesForCompany(companyId);



                if (MedicinesForCompany == null)
                {
                    return NotFound("Medicines for Company not found.");
                }

                var totalprice = CurrentCart.CartItems.Sum(ci => ci.Count * ci.UnitPrice);

                var test = CurrentCart.CartItems.FirstOrDefault();
                if (test != null && test.Medicine.Company.MinPriceToMakeOrder > totalprice)
                {
                    return BadRequest($"Total price must be at least {test.Medicine.Company.MinPriceToMakeOrder} to make an order for {test.Medicine.Company.Name}.");
                }

                // First check all the items on the card.
                foreach (var item in CurrentCart.CartItems)
                {
                    Medicine CurMedicine = MedicinesForCompany[item.MedicineId.Value];

                    if (CurMedicine == null)
                    {
                        return NotFound("Medicine not found.");
                    }

                    var availableStock = CurMedicine.InStock;

                    if (availableStock == null || availableStock < item.Count)
                    {
                        return BadRequest($"Quantity not available for the medicine Name = {item.Medicine.Name}.");
                    }
                }

                // All quantities are available - we start creating the order
                Order newOrder = new Order
                {
                    OrderItems = new List<OrderItem>(),
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusOrder = SD.StatusOrder_pending,
                    PharmacyID = CurrentCart.PharmacyId,
                    CompanyID = companyId,
                    TotalPrice = 0
                };

                foreach (var item in CurrentCart.CartItems)
                {

                    // Quantity discount
                    Medicine CurMedicine = MedicinesForCompany[item.MedicineId.Value];

                    CurMedicine.InStock -= item.Count;

                    OrderItem newOrderItem = new OrderItem
                    {
                        MedicineID = item.MedicineId,
                        MedicineName = item.Medicine.Name,
                        MedicineImage = item.Medicine.Image_URL,
                        Count = item.Count,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Count * item.UnitPrice
                    };

                    newOrder.TotalPrice += newOrderItem.TotalPrice;
                    newOrder.OrderItems.Add(newOrderItem);
                }

                var CurCartItems = CurrentCart.CartItems.ToList();

                // Deleted the CartItems
                _context.CartItems.RemoveRange(CurCartItems);


                CurrentCart.TotalPrice = 0;

                _orderRepositry.AddOrder(newOrder);

                // Commit the transaction
                await transaction.CommitAsync();

                return Ok("order created");
            }
            catch (Exception ex)
            {

                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();

                return Problem($"An error occurred while executing the request.: {ex.Message}");
            }
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPatch("done/{OrderId:int}")]
        public async Task<ActionResult> DoneOrder(int OrderId)
        {
            var order = await _orderRepositry.GetOrderById(OrderId);

            var currentUser = await _account.GetCurrentUser(User);

            if (order == null)
            {
                return NotFound("Orders not found.");
            }

            if (currentUser == null)
            {
                return NotFound("Company not found.");
            }

            if (currentUser.Id != order.CompanyID)
            {
                return Problem("You are not authorized to view this order.");
            }

            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                return NotFound("Order items not found.");
            }

            if (order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            if (order.StatusOrder != SD.StatusOrder_pending)
            {
                return BadRequest($"Order is {order.StatusOrder}");
            }

            _orderRepositry.ChangeStatusOrder(order, SD.StatusOrder_shipped);

            return Ok("The order has been shipped successfully");
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPatch("deliver/{OrderId:int}")]
        public async Task<ActionResult> DeliverOrder(int OrderId)
        {
            var order = await _orderRepositry.GetOrderById(OrderId);

            var currentUser = await _account.GetCurrentUser(User);            

            if (order == null)
            {
                return NotFound("Orders not found.");
            }

            if (currentUser == null)
            {
                return NotFound("Company not found.");
            }

            if (currentUser.Id != order.CompanyID)
            {
                return Problem("You are not authorized to view this order.");
            }

            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                return NotFound("Order items not found.");
            }

            if (order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            if (order.StatusOrder != SD.StatusOrder_shipped)
            {
                return BadRequest($"Order is {order.StatusOrder}");
            }

            _orderRepositry.ChangeStatusOrder(order, SD.StatusOrder_delivered);

            return Ok("The order has been delivered successfully");
        }


        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpDelete("Cancel/{OrderId:int}")]
        public async Task<ActionResult> CancelOrder(int OrderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Begin transaction

            try
            {
                var order = await _orderRepositry.GetOrderById(OrderId);

                var currentUser = await _account.GetCurrentUser(User);                

                if (order == null)
                {
                    return NotFound("Orders not found.");
                }

                if(currentUser == null)
                {
                    return NotFound("Pharmacy not found.");
                }

                if (currentUser.Id != order.PharmacyID)
                {
                    return Problem("You are not authorized to view this order.");
                }

                if (order.OrderItems == null || order.OrderItems.Count == 0)
                {
                    return NotFound("Order items not found.");
                }

                if (order.StatusOrder != SD.StatusOrder_pending)
                {
                    return BadRequest($"Order is {order.StatusOrder}");
                }

                if (order.CompanyID == null)
                {
                    return NotFound("Company not found.");
                }

                if (order.PharmacyID == null)
                {
                    return NotFound("Pharmacy not found.");
                }

                var companyId = order.CompanyID;
                var Medicines = await _medicineRepositiry.GetMedicinesForCompany((int)companyId);

                if (Medicines == null)
                {
                    return NotFound("Medicines for Company not found.");
                }

                foreach (var item in order.OrderItems)
                {

                    // Return the quantity
                    Medicine CurMedicine = Medicines[item.MedicineID.Value];
                    if (CurMedicine == null)
                    {
                        return NotFound("Medicine not found");
                    }
                    CurMedicine.InStock += item.Count;
                }

                _orderRepositry.DeleteOrder(order);

                // Commit the transaction
                await transaction.CommitAsync();

                return Ok("The order has been successfully cancelled.");

            }
            catch (Exception ex)
            {

                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();

                return Problem($"An error occurred while executing the request.: {ex.Message}");
            }

        }

    }
}