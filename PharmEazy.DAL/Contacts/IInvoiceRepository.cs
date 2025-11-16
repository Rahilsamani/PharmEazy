using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Contacts
{
    public interface IInvoiceRepository
    {
        public Task<bool> CreateInvoice(List<Invoice> invoices);
        public Task<List<AllInvoicesDTO>> GetAllInvoices(string userId);
        public Task<InvoiceDTO> GetInvoiceDetail(int id);
        public Task<(bool status, string message)> UpdateInvoiceStatus(int id);
    }
}
