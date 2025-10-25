using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.Models;

namespace PharmEazy.BAL.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly string? cs;

        public CategoryServices(IConfiguration configuration)
        {
            cs = configuration.GetConnectionString("UserDbContextConnection");
        }

        /// <summary>
        /// Gets All The Category Items Present Under Search Query And Page Number
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="SearchQuery"></param>
        /// <param name="PageSize"></param>
        /// <returns>List Of Categories</returns>
        public async Task<List<Category>> GetSearchedAndPaginationCategory(int PageNumber, string? SearchQuery, int PageSize)
        {
            List<Category> categories = new List<Category> { };

            using (SqlConnection connection = new SqlConnection(cs))
            {
                try
                {
                    categories = (await connection.QueryAsync<Category>("spGetCategoriesByQueryAndPagination", new { PageNumber, PageSize, SearchQuery }, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
                catch (Exception ex)
                {
                }
            }
            return categories;
        }

        /// <summary>
        /// Gets The Count Of The Categories Present Inside The Search Query
        /// </summary>
        /// <param name="SearchQuery"></param>
        /// <returns></returns>
        public async Task<int> GetCategoriesCountOnSearch(string? SearchQuery)
        {
            int count = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    count = await connection.ExecuteScalarAsync<int>("spGetCategoriesCountOnSearch", new { SearchQuery }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
            }
            return count;
        }
    }
}
