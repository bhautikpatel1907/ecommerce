using Ecom.Domain;
using Ecom.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Ecom.Services
{
    // Services/AuthService.cs
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> Register(Customer customer, string password)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
                throw new Exception("Email already exists");

            CreatePasswordHash(password, out byte[] hash, out byte[] salt);
            customer.PasswordHash = hash;
            customer.PasswordSalt = salt;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> Login(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null || !VerifyPassword(password, customer.PasswordHash, customer.PasswordSalt))
                return null;

            return customer;
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}
