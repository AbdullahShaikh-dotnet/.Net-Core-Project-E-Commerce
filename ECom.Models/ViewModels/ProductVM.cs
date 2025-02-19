using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECom.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> categoryList { get; set; }
    }
}
