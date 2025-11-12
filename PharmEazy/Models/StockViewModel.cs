using System.ComponentModel.DataAnnotations;

namespace PharmEazy.Models
{
    public class StockViewModel
    {
        public int StockId { get; set; }

        [Required(ErrorMessage = "The Quantity field is required")]
        [Range(0, 5000, ErrorMessage = "Please Enter Stock Between 0 to 5000")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "The Expiry field is required")]
        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "The Price field is required")]
        [Range(1, 50000, ErrorMessage = "Please Enter Stock Between 1 to 50000")]
        public double? Price { get; set; }
    }
}
