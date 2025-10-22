using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PharmEazy.Models
{
    public class User : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Please Enter Name Below 150 Characters")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Please Enter Address Below 200 Characters")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }
        public string? GstNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum GenderType
    {
        Male, Female
    }

    public enum RoleTypes
    {
        Admin, Buyer, Both
    }
}
