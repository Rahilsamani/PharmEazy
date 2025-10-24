using PharmEazy.Models;

namespace PharmEazy.DAL.Contacts
{
    public interface ICategoryServices
    {
        public Task<List<Category>> GetSearchedAndPaginationCategory(int PageNumber, string? SearchQuery, int PageSize);

        public Task<int> GetCategoriesCountOnSearch(string? SearchQuery);
    }
}
