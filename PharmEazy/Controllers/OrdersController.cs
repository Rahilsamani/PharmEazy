using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly INotyfService _notyf;
        private readonly UserManager<User> _userManager;
        private readonly ICartItemRepository _cartItemRepository;

        public OrdersController(IInvoiceRepository invoiceRepository, INotyfService notyf, UserManager<User> userManager, ICartItemRepository cartItemRepository)
        {
            _invoiceRepository = invoiceRepository;
            _notyf = notyf;
            _userManager = userManager;
            _cartItemRepository = cartItemRepository;
        }

        /// <summary>
        /// Action Method For Displaying All The Invoices Related to Specific User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List Of Invoices Of The User</returns>
        [Authorize]
        public async Task<IActionResult> Index(string? userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            string? loggedInId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = await _userManager.FindByIdAsync(loggedInId);
            var role = await _userManager.GetRolesAsync(user);

            if (!userId.Equals(loggedInId) && !role.Contains(RoleTypes.Admin.ToString()))
                return Unauthorized();

            List<AllInvoicesDTO> invoiceList = await _invoiceRepository.GetAllInvoices(userId);

            return View(invoiceList);
        }

        /// <summary>
        /// Action Method For Displaying Invoice Details Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Invoice Details Page Of the Specified Id</returns>
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            InvoiceDTO invoice = await _invoiceRepository.GetInvoiceDetail(id);

            return View(invoice);
        }

        /// <summary>
        /// Action Method For Updating The Status Of The Invoice
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            (bool status, string message) result = await _invoiceRepository.UpdateInvoiceStatus(id);

            return Json(new { success = result.status, message = result.message });
        }

        /// <summary>
        /// Action Method For Buying Medicine From The Medicine Deatils Page
        /// </summary>
        /// <param name="medicine"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        public async Task<IActionResult> BuyMedicine([FromBody] Medicine medicine)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User Not Found" });
            }

            List<CartItemDTO> cartItems = new List<CartItemDTO>()
            {
                new CartItemDTO()
                {
                    Id = 0,
                    Quantity = medicine.stocks[0].Quantity ?? 0,
                    Name = medicine.Name,
                    Description = medicine.Description,
                    ImageUrl = medicine.ImageUrl,
                    Price = medicine.stocks[0].Price ?? 0,
                    ExpiryDate = medicine.stocks[0].ExpiryDate.Value,
                    MedicineId = medicine.Id,
                    SellerId = medicine.SellerId,
                    StockId = medicine.stocks[0].Id,
                }
            };

            (bool status, string message) result = await _cartItemRepository.BuyAllCartItems(cartItems, userId);

            return Json(new { success = result.status, message = result.message });
        }
    }
}
