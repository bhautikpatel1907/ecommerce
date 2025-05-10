using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Domain
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("ShoppingCart")]
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public ShoppingCart ShoppingCart { get; set; }
        public Product Product { get; set; }
    }
}
