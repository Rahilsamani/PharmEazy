using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmEazy.Models
{
    public class InvoiceDetails
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Medicine")]
        [Required]
        public int MedicineId { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [ForeignKey("Invoice")]
        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("Stock")]
        [Required]
        public int StockId { get; set; }

        [Required]
        public double? Price { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Medicine Medicine { get; set; }
        public virtual Stock Stock { get; set; }

    }
}
