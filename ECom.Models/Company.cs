using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECom.Models
{
    public class Company
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits.")]
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
