using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models;

namespace PharmEazy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryServices _categoryServices;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotyfService _notyf;

        public CategoriesController(ICategoryRepository categoryRepository, ICategoryServices categoryServices, INotyfService notyf)
        {
            _notyf = notyf;
            _categoryRepository = categoryRepository;
            _categoryServices = categoryServices;
        }

        /// <summary>
        /// Action Method For Displaying Categories Page Where Admin Can Do CRUD Operations Related To Category
        /// </summary>
        /// <returns>Category View Page</returns>
        public IActionResult Index()
        {
            return View(new Category());
        }

        /// <summary>
        /// Action Method For Saving The Category Either By Updating Or Creating New Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        public async Task<IActionResult> Save(Category category)
        {
            (bool status, string message) result = (false, "Please Fill All the required Field");

            // Checking All Fields Are Valid Or Not
            if (!ModelState.IsValid)
            {
                return Json(new { success = result.status, message = result.message });
            }

            // Check Category Name Already Exists or not
            result = await _categoryRepository.CategoryExists(category.Id, category.Name);

            if (result.status)
            {
                return Json(new { success = false, message = result.message });
            }

            // Category Creation
            if (category.Id.Equals(0))
            {
                result = await _categoryRepository.CreateCategory(category);

                return Json(new { success = result.status, message = result.message });
            }
            // Category Edition
            else
            {
                result = await _categoryRepository.EditCategory(category);

                return Json(new { success = result.status, message = result.message });
            }
        }

        /// <summary>
        /// Action Method For Deleting Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            (bool status, string message) result = await _categoryRepository.DeleteCategory(id);

            return Json(new { success = result.status, message = result.message });
        }

        /// <summary>
        /// Action Method For Getting All The Categories Items According to Search Query And Page
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="currentPage"></param>
        /// <returns>List Of Categories And Total Pages Under Search Query</returns>
        public async Task<IActionResult> GetCategories(string? searchQuery, int currentPage = 1)
        {
            int pageSize = 5;
            int totalRecords = await _categoryServices.GetCategoriesCountOnSearch(searchQuery);

            List<Category> categories = await _categoryServices.GetSearchedAndPaginationCategory(currentPage, searchQuery, pageSize);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Json(new { categories, totalPages });
        }

        /// <summary>
        /// Action Method For Getting Specific Category With The Help Of Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Category Of the Specified Id</returns>
        public async Task<JsonResult> GetCategory(int id)
        {
            Category category = await _categoryRepository.GetCategory(id);

            return Json(category);
        }
    }
}
