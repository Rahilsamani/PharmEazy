using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly INotyfService _notyf;

        public CartController(ICartItemRepository cartItemRepository, INotyfService notyf)
        {
            _notyf = notyf;
            _cartItemRepository = cartItemRepository;
        }

        /// <summary>
        /// Action Method for Displaying All Cart Items
        /// </summary>
        /// <returns>Cart View</returns>
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<CartItemDTO> cartItems = new List<CartItemDTO>();

            if (!String.IsNullOrEmpty(userId))
            {
                cartItems = await _cartItemRepository.GetAllCartItems(userId);
            }

            return View(cartItems);
        }

        /// <summary>
        /// Action Method For Getting All Cart Items Of The User
        /// </summary>
        /// <returns>All Cart Items, Count of Cart Items And Total Amount</returns>
        public async Task<JsonResult> GetAllCartItems()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CartItemDTO> cartItems = new List<CartItemDTO>();
            double totalAmount = 0;

            if (!String.IsNullOrEmpty(userId))
            {
                cartItems = await _cartItemRepository.GetAllCartItems(userId);
                totalAmount = cartItems.Sum(c => c.Price * c.Quantity);
            }

            int items = cartItems.Count;

            return Json(new { cartItems, items, totalAmount });
        }

        /// <summary>
        /// Action Method For Deleting Cart Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<JsonResult> Delete(int id)
        {
            (bool status, string message) result = (false, "Something Went Wrong While Deleting Cart Item");

            if (!id.Equals(0))
            {
                result = await _cartItemRepository.DeleteCartItem(id);
            }

            return Json(new { success = result.status, message = result.message });
        }

        /// <summary>
        /// Action Method For Adding Medicine Inside Cart
        /// </summary>
        /// <param name="cart"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddCartItem([FromBody] CartItem cart)
        {
            (bool status, string message) result = (false, "Something Went Wrong While Adding Cart Item");

            if (cart == null)
                return Json(new { success = result.status, message = result.message });

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please Login First" });
            }

            cart.UserId = userId;

            result = await _cartItemRepository.CreateCartItem(cart);

            return Json(new { success = result.status, message = result.message });

        }

        /// <summary>
        ///     Action Method For Buying All The Medicines Present Inside The Cart
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        public async Task<JsonResult> BuyAllCartItems([FromBody] List<CartItemDTO> cartItems)
        {
            (bool status, string message) result;

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found" });
            }

            result = await _cartItemRepository.BuyAllCartItems(cartItems, userId);

            return Json(new { success = result.status, message = result.message });

        }
    }
}
