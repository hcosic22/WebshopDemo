using System.ComponentModel.DataAnnotations.Schema;

namespace WebshopDemo.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public string ProductName { get; set; }
    }
}
