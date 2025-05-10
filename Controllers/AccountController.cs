using Ecom.Infrastructure;
using Ecom.Models.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Account
        public async Task<IActionResult> Index()
        {
            // For demo, using customer ID 1 - in real app, use authenticated user
            int customerId = 1;

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Account/Edit
        public async Task<IActionResult> Edit()
        {
            int customerId = 1;

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new AccountInfoModel
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

            return View(model);
        }

        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountInfoModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(model.CustomerId);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Email = model.Email;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your account information has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Account/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            int customerId = 1;

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == customerId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
