using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Models.ViewModels
{
    public class ProductFilterViewModel
    {
        public List<string>? SelectedCategories { get; set; } = new();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? SortOptions { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}


