using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace Services.Stripe
{
    public class StripeService
    {
        private readonly string _stripeSecretKey;

        

        public async Task<string> CreatePaymentIntent(PaymentRequest paymentRequest)
        {
            long amountInCents = (long)(paymentRequest.Amount * 100);

            var options = new PaymentIntentCreateOptions
            {
                
                Amount = amountInCents,
                Currency = "egp",
                PaymentMethodTypes = new List<string> { "card"},
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.ClientSecret; 
        }
        public async Task<string> CreateSession(PaymentRequest paymentrequest)
        {
            long amountInCents = (long)(paymentrequest.Amount * 100);

            var options = new SessionCreateOptions
            {
                SuccessUrl = "http://localhost:5210/payments/success",
                CancelUrl = "http://localhost:5210/payments/cancel",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = amountInCents,
                    Currency = "egp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Your Product Name" 
                    }
                },
                Quantity = 1 
            };

            options.LineItems.Add(sessionLineItem);

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return session.Id;
        }
    }
}
