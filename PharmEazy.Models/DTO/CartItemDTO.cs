using System.ComponentModel.DataAnnotations;

namespace PharmEazy.Models.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        public double Price { get; set; }

        public DateTime ExpiryDate { get; set; }
        public int MedicineId { get; set; }
        public string SellerId { get; set; }
        public int StockId { get; set; }
    }
}
