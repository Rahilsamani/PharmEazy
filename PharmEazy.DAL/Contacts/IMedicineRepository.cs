using PharmEazy.Models;

namespace PharmEazy.DAL.Contacts
{
    public interface IMedicineRepository
    {
        public Task<(bool status, string message)> CreateMedicine(Medicine medicine);
        public Task<List<Medicine>> GetMedicines();
        public Task<(bool status, string message)> DeleteMedicine(int id);
        public Task<(bool status, string message)> IsMedicineNameExist(int medicineId, string name);
    }
}
