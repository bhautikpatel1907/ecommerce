using System.ComponentModel.DataAnnotations;

namespace Ecom.Domain
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [Display(Name = "Account Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
