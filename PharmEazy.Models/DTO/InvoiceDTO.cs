namespace PharmEazy.Models.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public int TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
        public string GstNumber { get; set; }
        public int PhoneNumber { get; set; }
        public List<InvoiceDetailDTO> InvoiceDetails { get; set; }
    }
}
