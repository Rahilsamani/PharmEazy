using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmEazy.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public string Userid { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        public double? TotalAmount { get; set; }

        [Required]
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }

        public virtual User User { get; set; }
        public List<InvoiceDetails> InvoiceDetails { get; set; }
    }
}
