using System.ComponentModel.DataAnnotations;

namespace PharmEazy.Models.DTO
{
    public class EditUserDTO
    {
        public string Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Name Must be of 200 Characters")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Address Must be of 200 Characters")]
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? GstNumber { get; set; }
    }
}
