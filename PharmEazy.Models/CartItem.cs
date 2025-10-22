using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmEazy.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Medicine")]
        [Required]
        public int MedicineId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("Stock")]
        public int StockId { get; set; }
        public virtual User User { get; set; }
        public virtual Medicine Medicine { get; set; }
        public virtual Stock Stock { get; set; }
    }
}

