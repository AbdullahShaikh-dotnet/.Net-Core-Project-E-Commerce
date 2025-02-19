using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECom.Models
{
    public class ShoppingCart
    {
        public int ID { get; set; }

        public int ProductID { get; set; }
        [ValidateNever]
        [ForeignKey("ProductID")]
        public Product product { get; set; }

        [Range(1, 100, ErrorMessage = "Count Must in Between 1 to 1000")]
        public int Count { get; set; }

        public string ApplicationUserID { get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }

        [ValidateNever]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [ValidateNever]
        public bool IsDeleted { get; set; }

        [ValidateNever]
        public DateTime? DeletedAt { get; set; }

        [NotMapped]
        public double ShoppingCartPrice { get; set; }

        public bool isOrderPlaced { get; set; } = false;
    }
}
