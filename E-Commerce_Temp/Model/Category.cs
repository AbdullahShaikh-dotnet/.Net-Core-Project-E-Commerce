using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace E_Commerce_Temp.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(50, ErrorMessage = $"Category Max Length Should be 50")]
        public string Name { get; set; }

        [Range(1, 100)]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
