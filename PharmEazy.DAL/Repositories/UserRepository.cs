using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string? cs;
        private const int PageSize = 5;

        public UserRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            cs = configuration.GetConnectionString("UserDbContextConnection");
        }

        /// <summary>
        /// Use To Edit The User Details
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> EditUserDetails(EditUserDTO user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    await connection.QueryAsync("spUpdateUserDetail", new { userId = user.Id, name = user.Name, phoneNumber = user.PhoneNumber, address = user.Address, dob = user.DateOfBirth, gstNumber = user.GstNumber }, commandType: System.Data.CommandType.StoredProcedure);

                    return (true, "User Edited Successfully");
                }
            }
            catch (Exception ex)
            {
                return (false, "User Edition Failed");
            }
        }

        /// <summary>
        /// Use To Get All The Available Users
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="currentPage"></param>
        /// <returns>List Of Available Users</returns>
        public async Task<List<AllUserDTO>> GetAllUsers(string? searchQuery, int currentPage)
        {
            List<AllUserDTO> users = new List<AllUserDTO>() { };

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    users = (await connection.QueryAsync<AllUserDTO>("spGetAllUsers", new { pagesize = PageSize, pageNumber = currentPage, searchQuery = searchQuery }, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return users;
        }

        /// <summary>
        /// Use To Get User Details Of The Specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User Details</returns>
        public async Task<EditUserDTO> GetUserDetail(string id)
        {
            string sql = "spGetUserDetails";
            EditUserDTO user = new EditUserDTO();

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    user = await connection.QueryFirstAsync<EditUserDTO>(sql, new { userId = id }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
            }

            return user;
        }

        /// <summary>
        /// Use To Get Users Count Present Under Search Query
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns>Count Of The Users</returns>
        public async Task<int> GetUsersCountOnSearch(string? searchQuery)
        {
            int count = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    count = await connection.ExecuteScalarAsync<int>("spGetAllUsersCountOnSearch", new { SearchQuery = searchQuery }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
            }

            return count;
        }
    }
}
