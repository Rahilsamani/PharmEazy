using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.Controllers
{
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMedicineServices _medicineServices;
        private readonly INotyfService _notyf;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<User> _userManager;

        public MedicineController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IMedicineRepository medicineRepository, IMedicineServices medicineServices, INotyfService notyf, ICategoryRepository categoryRepository, UserManager<User> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _medicineRepository = medicineRepository;
            _medicineServices = medicineServices;
            _notyf = notyf;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Action Method For Displaying Medicine Page Where Admin Can Do CRUD Operations for the Medicine
        /// </summary>
        /// <returns>Medicine View Page</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<CategoryIdNameDTO> categories = await _categoryRepository.GetCategoryIdAndName();

            ViewBag.CategoryList = categories;

            MedicineViewModel medicineViewModel = new MedicineViewModel()
            {
                Stocks = new List<StockViewModel>() { new StockViewModel() },
            };

            return View(medicineViewModel);
        }

        /// <summary>
        /// Action Method For Getting All The Medicines For The Specified Search Query And Current Page
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="currentPage"></param>
        /// <returns>List of Medicines And Total Pages</returns>
        public async Task<IActionResult> GetMedicines(string? searchQuery, int currentPage = 1)
        {
            int pageSize = 5;
            int totalRecords = await _medicineServices.GetMedicineCountOnSearch(searchQuery);

            List<Medicine> medicines = await _medicineServices.GetSearchedAndPaginationMedicine(currentPage, searchQuery);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Json(new { medicines, totalPages });
        }

        /// <summary>
        /// Action Method For Saving Medicine Either by Creating new Medicine Or Editing
        /// </summary>
        /// <param name="medicineViewModel"></param>
        /// <returns>Success Status And Message In JSON Format OR Errors In JSON Format When Validation Fails</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Save(MedicineViewModel medicineViewModel)
        {
            (bool status, string message) result = (false, "User Not Found");

            if (!medicineViewModel.MedicineId.Equals(0))
            {
                ModelState.Remove("medicineImage");
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(userId))
                return Json(new { success = result.status, message = result.message });

            if (ModelState.IsValid)
            {
                string? uniqueFileName = null;

                if (medicineViewModel.medicineImage != null && medicineViewModel.medicineImage.Length > 0)
                {
                    uniqueFileName = UploadFile(medicineViewModel.medicineImage);

                    if (String.IsNullOrEmpty(uniqueFileName))
                    {
                        return Json(new { success = false, message = "Something went wrong while uploading Medicine Image" });
                    }
                }

                result = await _medicineRepository.IsMedicineNameExist(medicineViewModel.MedicineId, medicineViewModel.Name);

                if (result.status)
                {
                    return Json(new { success = false, message = result.message });
                }

                var stockList = new List<Stock>() { };

                // Create All Stocks
                foreach (StockViewModel stock in medicineViewModel.Stocks)
                {
                    Stock newStock = new Stock()
                    {
                        ExpiryDate = stock.ExpiryDate,
                        Price = stock.Price,
                        Quantity = stock.Quantity,
                        Id = stock.StockId,
                        MedicineId = medicineViewModel.MedicineId
                    };

                    stockList.Add(newStock);
                }

                // Create Medicine
                Medicine medicine = new Medicine()
                {
                    CategoryId = medicineViewModel.CategoryId,
                    Description = medicineViewModel.Description,
                    ImageUrl = uniqueFileName,
                    Name = medicineViewModel.Name,
                    SellerId = userId,
                    stocks = stockList,
                    Id = medicineViewModel.MedicineId
                };

                // Create New Medicine
                if (medicineViewModel.MedicineId == 0)
                {
                    result = await _medicineRepository.CreateMedicine(medicine);

                    return Json(new { success = result.status, message = result.message });
                }
                else
                {
                    result = await _medicineServices.EditMedicine(medicine);

                    return Json(new { success = result.status, message = result.message });
                }
            }

            return Json(ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToList());
        }

        /// <summary>
        /// Method For Uploading The File Inside wwwRoot Folder
        /// </summary>
        /// <param name="medicineImage"></param>
        /// <returns>File Name When Available Else Empty String</returns>
        private string UploadFile(IFormFile medicineImage)
        {
            string fileName = "";
            if (medicineImage != null)
            {
                try
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    fileName = Guid.NewGuid().ToString() + "-" + medicineImage.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        medicineImage.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            return fileName;
        }

        /// <summary>
        /// Action Method For Getting Medicine Specific To Given Medicine Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Medicine For The Specified Id</returns>
        [AllowAnonymous]
        public async Task<JsonResult> GetMedicine(int id)
        {
            Medicine medicine = await _medicineServices.GetMedicine(id);

            return Json(medicine);
        }

        /// <summary>
        /// Action Method For Deleting The Medicine
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> Delete(int id)
        {
            (bool status, string message) result = await _medicineRepository.DeleteMedicine(id);

            return Json(new { success = result.status, message = result.message });
        }

        /// <summary>
        /// Action Method For Getting Medicine Detail of The Specified Medicine Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Medicine Details Of The Specified Medicine Id</returns>
        public async Task<ActionResult> MedicineDetail(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Medicine medicine = await _medicineServices.GetMedicine(id);

            if (!String.IsNullOrEmpty(userId))
            {
                User? user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                bool eligibleToBuy = roles.Contains(RoleTypes.Buyer.ToString()) && !userId.Equals(medicine.SellerId);
                ViewBag.EligibleToBuy = eligibleToBuy;
            }

            return View("Details", medicine);
        }
    }
}
