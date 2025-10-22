using System.ComponentModel.DataAnnotations;

namespace PharmEazy.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Please Enter Category Name Below 100 Characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please Enter Alphabets Only")]
        public string Name { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "Please Enter Category Description Below 300 Characters")]
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool isDeleted { get; set; }
    }
}
