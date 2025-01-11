using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECom.Models
{
    public  class OrderHeader
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public ApplicationUser _ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }
        public string PaymentIntendID { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        [ValidateNever]
        public DateTime? UpdateDate { get; set; }

        [ValidateNever]
        public DateTime? DeletedAt { get; set; }

        [ValidateNever]
        public bool IsDeleted { get; set; }
    }
}
