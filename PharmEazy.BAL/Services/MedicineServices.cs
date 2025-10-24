using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;

namespace PharmEazy.BAL.Services
{
    public class MedicineServices : IMedicineServices
    {
        private ApplicationDbContext _context;
        private readonly string? cs;
        private const int PageSize = 5;

        public MedicineServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            cs = configuration.GetConnectionString("UserDbContextConnection");
        }

        /// <summary>
        /// Gets The Medicine Of The Specified Medicine Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Medicine With Their Stocks</returns>
        public async Task<Medicine> GetMedicine(int id)
        {
            string sql = "spGetMedicine";
            Dictionary<int, Medicine> medicineDictionary = new Dictionary<int, Medicine>();
            Medicine? medicine = new Medicine();

            try
            {
                using (var connection = new SqlConnection(cs))
                {
                    medicine = (await connection.QueryAsync<Medicine, Stock, Medicine>(sql, (medicine, stock) =>
                    {
                        Medicine medicineEntry;

                        if (!medicineDictionary.TryGetValue(medicine.Id, out medicineEntry))
                        {
                            medicineEntry = medicine;
                            medicineEntry.stocks = new List<Stock>();
                            medicineDictionary.Add(medicineEntry.Id, medicineEntry);
                        }

                        medicineEntry.stocks.Add(stock);
                        return medicineEntry;
                    },
                    new { medicineId = id },
                    splitOn: "Id")).First();
                }
            }
            catch (Exception ex)
            {
            }

            return medicine;
        }

        /// <summary>
        /// Updates The Medicine Of The Provided Medicine Id
        /// </summary>
        /// <param name="medicine"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> EditMedicine(Medicine medicine)
        {
            string spMedicineEdition = "spEditMedicine";
            string spStockEdition = "spEditStock";

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.QueryAsync(spMedicineEdition, new { medicineId = medicine.Id, description = medicine.Description, name = medicine.Name, imageUrl = medicine.ImageUrl }, transaction: transaction);

                        foreach (var stock in medicine.stocks)
                        {
                            await connection.QueryAsync(spStockEdition, new { stockId = stock.Id, ExpiryDate = stock.ExpiryDate, Price = stock.Price, Quantity = stock.Quantity }, transaction: transaction);
                        }

                        await transaction.CommitAsync();
                        return (true, "Medicine Edited Successfully");
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
        /// Gets All The Medicine Present Under Search Query And Page Number
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="SearchQuery"></param>
        /// <returns>List Of Medicines</returns>
        public async Task<List<Medicine>> GetSearchedAndPaginationMedicine(int PageNumber, string? SearchQuery)
        {
            string sql = "spGetMedicinesByQueryAndPagination";
            List<Medicine> medicines = new List<Medicine>() { };

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    Dictionary<int, Medicine> medicineDictionary = new Dictionary<int, Medicine>();

                    medicines = (await connection.QueryAsync<Medicine, Stock, Medicine>(sql, (medicine, stock) =>
                    {
                        Medicine medicineEntry;

                        if (!medicineDictionary.TryGetValue(medicine.Id, out medicineEntry))
                        {
                            medicineEntry = medicine;
                            medicineEntry.stocks = new List<Stock>();
                            medicineDictionary.Add(medicineEntry.Id, medicineEntry);
                        }

                        medicineEntry.stocks.Add(stock);
                        return medicineEntry;
                    },
                    new { PageNumber, PageSize, SearchQuery },
                    splitOn: "Id")).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return medicines;
        }

        /// <summary>
        /// Gets The Count Of The Medicine Under Search Query
        /// </summary>
        /// <param name="SearchQuery"></param>
        /// <returns>Count Of The Medicines</returns>
        public async Task<int> GetMedicineCountOnSearch(string? SearchQuery)
        {
            int count = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    count = await connection.ExecuteScalarAsync<int>("spGetMedicinesCountOnSearch", new { SearchQuery }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
            }

            return count;
        }
    }
}
