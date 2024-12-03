using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Stripe
{
    public class PaymentRequest
    {
        [Required(ErrorMessage ="Email Address is required")]
        [EmailAddress(ErrorMessage ="Please Enter a Valid Email Address")]
        public string Email { get; set; }
        public string? Token { get; set; }
        public double Amount { get; set; } 
        public int BookingId { get; set; } 
    }
}
