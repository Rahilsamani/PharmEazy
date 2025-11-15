using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models;

namespace PharmEazy.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMedicineServices _medicineServices;
        private readonly INotyfService _notyf;
        private readonly UserManager<User> _userManager;

        public HomeController(IMedicineRepository medicineRepository, IMedicineServices medicineServices, INotyfService notyf, UserManager<User> userManager)
        {
            _medicineRepository = medicineRepository;
            _medicineServices = medicineServices;
            _notyf = notyf;
            _userManager = userManager;
        }

        /// <summary>
        /// Action Method For Displaying All The Medicines In The Home Page
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="currentPage"></param>
        /// <returns>Home Page View Having Medicines</returns>
        public async Task<IActionResult> Index(string? searchQuery, int currentPage = 1)
        {
            List<Medicine> medicines = await _medicineServices.GetSearchedAndPaginationMedicine(currentPage, searchQuery);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.userId = userId;

            if (!String.IsNullOrEmpty(userId))
            {
                User? user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                ViewBag.isBuyer = roles.Contains(RoleTypes.Buyer.ToString());
            }

            return View(medicines);
        }
    }
}
