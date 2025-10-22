using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmEazy.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Please Enter Name Below 100 Characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "Please Enter Name Below 300 Characters")]
        public string Description { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required]
        [ForeignKey("User")]
        public string SellerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
        public List<Stock> stocks { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }

    }
}
