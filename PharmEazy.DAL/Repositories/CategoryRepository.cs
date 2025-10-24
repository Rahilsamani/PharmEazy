using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string? cs;

        public CategoryRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            cs = configuration.GetConnectionString("UserDbContextConnection");
            _context = context;
        }

        /// <summary>
        /// Checks Category with same name already exists or not
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="categoryName"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> CategoryExists(int categoryId, string categoryName)
        {
            try
            {
                bool isExist = await _context.Category.Where(e => e.isDeleted == false).AnyAsync(e => e.Name.ToLower() == categoryName.ToLower() && categoryId != e.Id);

                return (isExist, "Please Enter Unique Category");
            }
            catch (Exception ex)
            {
                return (true, "Something Went Wrong");
            }
        }

        /// <summary>
        /// Use to Create Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> CreateCategory(Category category)
        {
            try
            {
                category.CreatedOn = DateTime.Now;
                category.isDeleted = false;

                await _context.AddAsync(category);
                await _context.SaveChangesAsync();

                return (true, "Category Created Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Category Creation Failed");
            }
        }

        /// <summary>
        /// Use to Soft Delete the Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> DeleteCategory(int id)
        {
            try
            {
                Category? category = await _context.Category.FindAsync(id);

                if (category == null)
                    return (false, "Category Not Found");

                if (await _context.Medicine.AnyAsync(m => m.CategoryId == id))
                {
                    return (false, "You cannot delete Category Because Medicine is present Under the Category");
                }

                category.isDeleted = true;
                category.DeletedOn = DateTime.Now;
                _context.Update(category);

                await _context.SaveChangesAsync();

                return (true, "Category Deleted Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Something Went Wrong While Deleting Category");
            }
        }

        /// <summary>
        /// Use To Edit The Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> EditCategory(Category category)
        {
            try
            {
                category.UpdatedOn = DateTime.Now;
                _context.Update(category);

                await _context.SaveChangesAsync();
                return (true, "Category Updated Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Category Updation Failed");
            }
        }

        /// <summary>
        /// Use to Get Category Of Specific Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Catgeory Of The Specified Id</returns>
        public async Task<Category> GetCategory(int id)
        {
            Category category = new Category();

            try
            {
                category = await _context.Category.FirstAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
            }

            return category;
        }

        /// <summary>
        /// Use to fetch All The Categories Having Id And Name
        /// </summary>
        /// <returns>List Of Catgeories Having Name And Id</returns>
        public async Task<List<CategoryIdNameDTO>> GetCategoryIdAndName()
        {
            string spGetCategories = "spGetCategoryNameAndId";
            List<CategoryIdNameDTO> categories = new List<CategoryIdNameDTO>() { };

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    categories = (await connection.QueryAsync<CategoryIdNameDTO>(spGetCategories, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return categories;
        }
    }
}
