using System;
using System.Collections.Generic;
using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace ECom.Utility.Services
{
    public class RazorPayService : IRazorPayService
    {
        private readonly RazorPaySettings _razorPaySettings;
        public RazorPayService(IConfiguration configuration, IOptions<RazorPaySettings> razorPaySettings)
        {
            _razorPaySettings = razorPaySettings.Value;
        }


        /// <summary>
        /// Creates an order in Razorpay.
        /// </summary>
        /// <param name="amount">The amount to be charged (in INR, not paise).</param>
        /// <param name="currency">The currency for the payment. Default is INR.</param>
        /// <returns>The created Razorpay order.</returns>
        public Order CreateOrder(decimal amount, string currency = "INR")
        {
            var client = new RazorpayClient(_razorPaySettings.PublishableKey, _razorPaySettings.SecretKey);

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
                RazorpayClient client = new RazorpayClient(_razorPaySettings.PublishableKey, _razorPaySettings.SecretKey);
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
                RazorpayClient client = new RazorpayClient(_razorPaySettings.PublishableKey, _razorPaySettings.SecretKey);
                return client.Payment.Fetch(paymentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch payment details: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Refund the payment using Razorpay API.
        /// </summary>
        /// <param name="paymentId">The payment ID from Razorpay.</param>
        /// <param name="amount">The payment Amount.</param>
        /// <returns>The JsonResult object containing Refund details.</returns>
        /// 
        public JsonResult Refund(string paymentId, decimal amount)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_razorPaySettings.PublishableKey, _razorPaySettings.SecretKey);
                var options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) } // Amount in paise
                };

                Refund refund = client.Payment.Fetch(paymentId).Refund(options);
                return new JsonResult(new { success = true, message = "Refund initiated successfully", refund });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }

}
