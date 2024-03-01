using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
