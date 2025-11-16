namespace PharmEazy.Models.DTO
{
    public class AllInvoicesDTO
    {
        public int Id { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SellerName { get; set; }
        public string GstNumber { get; set; }
    }
}
