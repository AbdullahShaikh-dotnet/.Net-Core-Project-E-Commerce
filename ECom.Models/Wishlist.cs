using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECom.Models
{
    public class Wishlist
    {
        public int ID { get; set; }



        [ValidateNever]
        [ForeignKey("ProductID")]
        public Product product { get; set; }
        public int ProductID { get; set; }



        [ValidateNever]
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserID { get; set; }



        [ValidateNever]
        public DateTime CreateDate { get; set; } = DateTime.Now;


        [ValidateNever]
        public bool IsDeleted { get; set; }


        [ValidateNever]
        public DateTime? DeletedAt { get; set; }
    }
}
