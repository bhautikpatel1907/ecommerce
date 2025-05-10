using Ecom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();

                // Apply any pending migrations
                context.Database.Migrate();

                // Seed products if none exist
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Product { Name = "Laptop", Description = "High performance laptop", Price = 999.99m, StockQuantity = 10 },
                        new Product { Name = "Smartphone", Description = "Latest smartphone", Price = 699.99m, StockQuantity = 15 },
                        new Product { Name = "Headphones", Description = "Wireless noise-cancelling", Price = 199.99m, StockQuantity = 20 },
                        new Product { Name = "Smart Watch", Description = "Fitness tracker", Price = 149.99m, StockQuantity = 12 },
                        new Product { Name = "Tablet", Description = "10-inch tablet", Price = 349.99m, StockQuantity = 8 },
                        new Product { Name = "Wireless Earbuds", Description = "True wireless", Price = 129.99m, StockQuantity = 18 },
                        new Product { Name = "External SSD", Description = "1TB portable", Price = 129.99m, StockQuantity = 10 },
                        new Product { Name = "Bluetooth Speaker", Description = "Waterproof", Price = 79.99m, StockQuantity = 15 },
                        new Product { Name = "Monitor", Description = "27-inch 4K", Price = 399.99m, StockQuantity = 5 },
                        new Product { Name = "Keyboard", Description = "Mechanical", Price = 89.99m, StockQuantity = 12 }
                    );

                    // Add a default customer with shopping cart
                    var customer = new Customer
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john@example.com",
                        ShoppingCart = new ShoppingCart()
                    };

                    context.Customers.Add(customer);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Fix: Use a non-static type for the logger
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
