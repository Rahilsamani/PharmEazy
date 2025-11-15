using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models.DTO;

namespace PharmEazy.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyf;

        public UsersController(IUserRepository userRepository, INotyfService notyf)
        {
            _userRepository = userRepository;
            _notyf = notyf;
        }

        /// <summary>
        /// Action Method For Displaying All The Available User
        /// </summary>
        /// <returns>Users Page Where All Users Details Are Available</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(new AllUserDTO());
        }

        /// <summary>
        /// Action Method For Getting All The Users List And Total Pages
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="currentPage"></param>
        /// <returns>List Of Users For Specified Search Query And Current Page</returns>
        public async Task<IActionResult> GetUsers(string? searchQuery, int currentPage = 1)
        {
            int pageSize = 5;
            int totalRecords = await _userRepository.GetUsersCountOnSearch(searchQuery);

            List<AllUserDTO> users = await _userRepository.GetAllUsers(searchQuery, currentPage);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Json(new { users, totalPages });
        }

        /// <summary>
        /// Action Method For Displaying The User Update Page
        /// </summary>
        /// <returns>User Details Page View</returns>
        public async Task<IActionResult> Edit()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId))
            {
                _notyf.Error("User Not Found");
                return View(new EditUserDTO());
            }

            EditUserDTO userDetail = await _userRepository.GetUserDetail(userId);

            return View(userDetail);
        }

        /// <summary>
        /// Action Method For Updating the User's Detail
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserDTO user)
        {
            (bool status, string message) result = (false, "User Edition Failed");

            if (ModelState.IsValid)
            {
                result = await _userRepository.EditUserDetails(user);
            }

            return Json(new { success = result.status, message = result.message });
        }
    }
}
