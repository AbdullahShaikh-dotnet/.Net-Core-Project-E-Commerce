using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace ECom.Utility
{
    public class RazorPayService
    {
        public string _key = "";
        private static string _secret = "";
        
        public RazorPayService(IConfiguration configuration)
        {
            var razorPaySettings = configuration.GetSection("Razorpay").Get<RazorPaySettings>();
            _key = razorPaySettings?.PublishableKey;
            _secret = razorPaySettings?.SecretKey;
        }


        /// <summary>
        /// Creates an order in Razorpay.
        /// </summary>
        /// <param name="amount">The amount to be charged (in INR, not paise).</param>
        /// <param name="currency">The currency for the payment. Default is INR.</param>
        /// <returns>The created Razorpay order.</returns>
        public Order CreateOrder(decimal amount, string currency = "INR")
        {
            var client = new RazorpayClient(_key, _secret);

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, // Convert to paise
                { "currency", currency },
                { "receipt", Guid.NewGuid().ToString() },
                { "payment_capture", 1 }
            };

            // Create and return the order
            return client.Order.Create(options);
        }

        /// <summary>
        /// Verifies the payment using Razorpay's signature validation.
        /// </summary>
        /// <param name="razorpayOrderId">The Razorpay order ID.</param>
        /// <param name="razorpayPaymentId">The Razorpay payment ID.</param>
        /// <param name="razorpaySignature">The Razorpay signature for validation.</param>
        /// <returns>True if the payment is verified, otherwise false.</returns>
        public bool VerifyPayment(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
        {
            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", razorpayOrderId },
                { "razorpay_payment_id", razorpayPaymentId },
                { "razorpay_signature", razorpaySignature }
            };

            try
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);
                Utils.verifyPaymentSignature(attributes);
                return true; // Payment is verified
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment verification failed: {ex.Message}");
                return false; // Payment verification failed
            }
        }

        /// <summary>
        /// Fetches the details of a payment from Razorpay API.
        /// </summary>
        /// <param name="paymentId">The payment ID from Razorpay.</param>
        /// <returns>The payment object containing details.</returns>
        public Payment GetPaymentDetails(string paymentId)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);
                return client.Payment.Fetch(paymentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch payment details: {ex.Message}");
                throw;
            }
        }
    }
}
