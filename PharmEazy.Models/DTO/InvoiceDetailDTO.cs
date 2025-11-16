namespace PharmEazy.Models.DTO
{
    public class InvoiceDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
