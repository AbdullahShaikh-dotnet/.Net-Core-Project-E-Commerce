using ECom.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECom.Models.ViewModels
{
    public class OrderPayment
    {
        public int ID { get; set; }

        [Required]
        public string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ContactNo { get; set; }
        public string Description { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        [Required]
        public int OrderID { get; set; }

        [Required]
        public string Public_Key { get; set; }

        [Required]
        public double Amount { get; set; }
        public string? razorpay_payment_id { get; set; }
        public string? razorpay_order_id { get; set; }
        public string? razorpay_signature { get; set; }
        public bool isPaymentSuccessfull { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
