using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Contacts
{
    public interface IUserRepository
    {
        public Task<List<AllUserDTO>> GetAllUsers(string? searchQuery, int currentPage);

        public Task<int> GetUsersCountOnSearch(string? searchQuery);

        public Task<EditUserDTO> GetUserDetail(string id);

        public Task<(bool status, string message)> EditUserDetails(EditUserDTO user);
    }
}
