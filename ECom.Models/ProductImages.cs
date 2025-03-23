using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Models
{
    public class ProductImages
    {
        public int ID { get; set; }

        [Required]
        public string ImageURL { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
    }
}
