using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmEazy.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Medicine")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "The Quantity field is required")]
        [Range(0, 5000, ErrorMessage = "Please Enter Stock Between 0 to 5000")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "The Expiry field is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateNotInPast(ErrorMessage = "Expiry Date Cannot be in Past")]
        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "The Price field is required")]
        [Range(1, 50000, ErrorMessage = "Please Enter Stock Between 1 to 50000")]
        public double? Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Medicine Medicine { get; set; }
    }

    public class DateNotInPastAttribute : ValidationAttribute
    {
        public DateNotInPastAttribute()
        {
            ErrorMessage = "Expiry Date Cannot be in Past";
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue < DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else
            {
                return new ValidationResult("Invalid data type for DateNotInFutureAttribute");
            }
            return ValidationResult.Success;
        }
    }
}
