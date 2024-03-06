using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebshopDemo.Models
{
    public class Order
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Firtst name is required.")]
        [StringLength(50)]
        public string BillingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string BillingLastName { get; set; }

        [Required(ErrorMessage = "Email adress is required.")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string BillingEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(50)]
        public string BillingPhone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50)]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50)]
        public string BillingCountry { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(20)]
        public string BillingZipCode { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        [NotMapped]
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}
