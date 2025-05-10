using Ecom.Domain;
using Ecom.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;

        public ShoppingCartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // For simplicity, we'll assume customer with ID 1 is the current user
            int customerId = 1;

            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customerId,
                    CartItems = new List<CartItem>()
                };
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            // For simplicity, we'll assume customer with ID 1 is the current user
            int customerId = 1;

            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index");
            }

            // Create order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Clear the cart
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Add success message that will disappear
            TempData["CheckoutSuccess"] = true;

            // Redirect to order details page
            return RedirectToAction("OrderDetails", "Account", new { id = order.OrderId });
        }
    }
}
