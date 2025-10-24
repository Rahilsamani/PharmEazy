using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Contacts
{
    public interface ICategoryRepository
    {
        public Task<(bool status, string message)> CreateCategory(Category category);
        public Task<(bool status, string message)> DeleteCategory(int id);
        public Task<(bool status, string message)> CategoryExists(int categoryId, string categoryName);
        public Task<(bool status, string message)> EditCategory(Category category);
        public Task<Category> GetCategory(int id);
        public Task<List<CategoryIdNameDTO>> GetCategoryIdAndName();
    }
}
