using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
    }
}
