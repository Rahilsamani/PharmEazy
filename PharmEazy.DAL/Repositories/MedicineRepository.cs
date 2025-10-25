using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;

namespace PharmEazy.DAL.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string? cs;

        public MedicineRepository(ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            cs = configuration.GetConnectionString("UserDbContextConnection");
            _context = applicationDbContext;
        }

        /// <summary>
        /// Use To Create Medicine And Their Stock
        /// </summary>
        /// <param name="medicine"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> CreateMedicine(Medicine medicine)
        {
            string spMedicineCreation = "spCreateMedicine";
            string spStockCreation = "spCreateStock";

            using (SqlConnection connection = new SqlConnection(cs))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Create Medicine
                        int medicineId = await connection.ExecuteScalarAsync<int>(spMedicineCreation, new { Name = medicine.Name, Description = medicine.Description, CategoryId = medicine.CategoryId, ImageUrl = medicine.ImageUrl, SellerId = medicine.SellerId, CreatedOn = DateTime.Now }, transaction: transaction);

                        // Create each stock
                        foreach (var stock in medicine.stocks)
                        {
                            await connection.QueryAsync(spStockCreation, new { MedicineId = medicineId, Quantity = stock.Quantity, ExpiryDate = stock.ExpiryDate, Price = stock.Price, CreatedOn = DateTime.Now }, transaction: transaction);
                        }

                        await transaction.CommitAsync();
                        return (true, "Medicine Created Successfully");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        return (false, "Something Went Wrong While Creating Medicine");
                    }
                }
            }
        }

        /// <summary>
        /// Use To Check Medicine Name Already Exists Or Not
        /// </summary>
        /// <param name="medicineId"></param>
        /// <param name="name"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> IsMedicineNameExist(int medicineId, string name)
        {
            try
            {
                bool isExist = await _context.Medicine.Where(m => m.IsDeleted == false).AnyAsync(m => m.Name.ToLower() == name.ToLower() && m.Id != medicineId);

                return (isExist, "Medicine Already Exists With this Name");
            }
            catch (Exception ex)
            {
                return (true, "Something Went Wrong");
            }
        }

        /// <summary>
        /// Use To Get All The Medicines
        /// </summary>
        /// <returns>List Of Medicines</returns>
        public async Task<List<Medicine>> GetMedicines()
        {
            return await _context.Medicine.Where(m => m.IsDeleted == false).ToListAsync();
        }

        /// <summary>
        /// Use To Soft Delete Medicine And Their Stock Of The Specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> DeleteMedicine(int id)
        {
            string spMedicineDeletion = "spDeleteMedicine";
            string spStockDeletion = "spDeleteStock";

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // delete medicine and return all the stocks id
                        List<int> ids = (await connection.QueryAsync<int>(spMedicineDeletion, new { MedicineId = id }, transaction: transaction)).ToList();

                        foreach (int stockId in ids)
                        {
                            await connection.QueryAsync(spStockDeletion, new { StockId = stockId }, transaction: transaction);
                        }

                        await transaction.CommitAsync();
                        return (true, "Medicine Deleted Successfully");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        return (false, "Something Went Wrong While Deleting Medicine");
                    }
                }
            }
        }
    }
}
