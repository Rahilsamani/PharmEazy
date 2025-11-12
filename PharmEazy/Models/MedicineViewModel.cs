using System.ComponentModel.DataAnnotations;

namespace PharmEazy.Models
{
    public class MedicineViewModel
    {
        public int MedicineId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Please Enter Name Below 100 Characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "Please Enter Name Below 300 Characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Category Field is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<StockViewModel> Stocks { get; set; }

        [Required(ErrorMessage = "Medicine Image Is Required")]
        [Display(Name = "Image")]
        public IFormFile medicineImage { get; set; }
    }
}
