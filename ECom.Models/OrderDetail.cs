using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECom.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }

        [Required]
        public int OrderHeaderID { get; set; }

        [ForeignKey("OrderHeaderID")]
        [ValidateNever]
        public OrderHeader _OrderHeader { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        [ValidateNever]
        public Product _Product { get; set; }

        public int Count { get; set; }
        public double Price { get; set; }
    }
}
