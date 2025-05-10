using Ecom.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CartSummaryViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // For demo, using customer ID 1 - in real app, use authenticated user
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == 1);

            return View(cart?.CartItems.Sum(ci => ci.Quantity) ?? 0);
        }
    }
}
