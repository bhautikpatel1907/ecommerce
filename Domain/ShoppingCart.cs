using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecom.Domain
{
    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
