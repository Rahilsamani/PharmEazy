
using PharmEazy.Models;

namespace PharmEazy.DAL.Contacts
{
    public interface IMedicineServices
    {
        public Task<Medicine> GetMedicine(int id);
        public Task<List<Medicine>> GetSearchedAndPaginationMedicine(int PageNumber, string? SearchQuery);

        public Task<int> GetMedicineCountOnSearch(string? SearchQuery);

        public Task<(bool status, string message)> EditMedicine(Medicine medicine);
    }
}
