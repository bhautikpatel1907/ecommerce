using Ecom.Domain;
using Ecom.Models.Customer;
using Ecom.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Controllers
{
    // Controllers/AuthController.cs
    public class CustomerController : Controller
    {
        private readonly AuthService _authService;

        public CustomerController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                await _authService.Register(customer, model.Password);
                return RedirectToAction("Login", new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Auth/Login
        public IActionResult Login(bool success = false)
        {
            ViewBag.RegistrationSuccess = success;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = await _authService.Login(model.Email, model.Password);
            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // TODO: Implement session/cookie authentication
            return RedirectToAction("Index", "Home");
        }
    }
}
