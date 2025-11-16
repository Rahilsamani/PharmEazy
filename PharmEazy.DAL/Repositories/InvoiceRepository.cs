using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string? cs;

        public InvoiceRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            cs = configuration.GetConnectionString("UserDbContextConnection");
            _context = context;
        }

        /// <summary>
        /// Use To Create Invoices And Invoice Details
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns>Result Status</returns>
        public async Task<bool> CreateInvoice(List<Invoice> invoices)
        {
            string spInvoiceCreation = "spCreateInvoice";
            string spInvoiceDetailCreation = "spCreateInvoiceDetail";

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (Invoice invoice in invoices)
                            {
                                // SP for creating invoice and returns invoice id
                                int invoiceId = await connection.ExecuteScalarAsync<int>(spInvoiceCreation, new { UserId = invoice.Userid, TotalAmount = invoice.TotalAmount, SellerId = invoice.SellerId }, transaction: transaction);

                                foreach (InvoiceDetails detail in invoice.InvoiceDetails)
                                {
                                    // SP for creating invoice details
                                    await connection.QueryAsync(spInvoiceDetailCreation, new { Quantity = detail.Quantity, MedicineId = detail.MedicineId, InvoiceId = invoiceId, Price = detail.Price, StockId = detail.StockId }, transaction: transaction);
                                }
                            }
                            await transaction.CommitAsync();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Use To Get All The Invoices Of The User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List Of All The Invoices</returns>
        public async Task<List<AllInvoicesDTO>> GetAllInvoices(string userId)
        {
            string sql = "spGetUserInvoices";
            List<AllInvoicesDTO> allInvoices = new List<AllInvoicesDTO>() { };

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    allInvoices = (await connection.QueryAsync<AllInvoicesDTO>(sql, new { userId }, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return allInvoices;
        }

        /// <summary>
        /// Use to Get Specific Invoice And Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Invoice With Details</returns>
        public async Task<InvoiceDTO> GetInvoiceDetail(int id)
        {
            string sql = "spGetInvoiceDeatail";
            Dictionary<int, InvoiceDTO> invoiceDictionary = new Dictionary<int, InvoiceDTO>();
            InvoiceDTO invoice = new InvoiceDTO();

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    // fetch all the invoices and format in one to many format
                    invoice = (await connection.QueryAsync<InvoiceDTO, InvoiceDetailDTO, InvoiceDTO>(sql, (currInvoice, invoiceDetail) =>
                   {
                       InvoiceDTO invoiceEntry;

                       if (!invoiceDictionary.TryGetValue(currInvoice.Id, out invoiceEntry))
                       {
                           invoiceEntry = currInvoice;
                           invoiceEntry.InvoiceDetails = new List<InvoiceDetailDTO>();
                           invoiceDictionary.Add(currInvoice.Id, invoiceEntry);
                       }

                       invoiceEntry.InvoiceDetails.Add(invoiceDetail);
                       return invoiceEntry;
                   },
                   new { invoiceId = id },
                   splitOn: "Id")).First();
                }
            }
            catch (Exception ex)
            {
            }

            return invoice;
        }

        /// <summary>
        /// Use To Update The Status Of The Invoice
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> UpdateInvoiceStatus(int id)
        {
            try
            {
                Invoice? invoice = await _context.Invoice.FindAsync(id);

                if (invoice == null)
                    return (false, "Invoice Not Found");

                invoice.Status = "Paid";

                _context.Update(invoice);
                await _context.SaveChangesAsync();

                return (true, "Invoice Status Updated Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Something Went Wrong While Updating Invoice Status");
            }

        }
    }
}
