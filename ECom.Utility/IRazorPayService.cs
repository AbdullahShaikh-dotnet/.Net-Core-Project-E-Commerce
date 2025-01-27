using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace ECom.Utility
{
    public interface IRazorPayService
    {
        /// <summary>
        /// Creates an order in Razorpay.
        /// </summary>
        /// <param name="amount">The amount to be charged (in INR, not paise).</param>
        /// <param name="currency">The currency for the payment. Default is INR.</param>
        /// <returns>The created Razorpay order.</returns>
        Order CreateOrder(decimal amount, string currency = "INR");

        /// <summary>
        /// Verifies the payment using Razorpay's signature validation.
        /// </summary>
        /// <param name="razorpayOrderId">The Razorpay order ID.</param>
        /// <param name="razorpayPaymentId">The Razorpay payment ID.</param>
        /// <param name="razorpaySignature">The Razorpay signature for validation.</param>
        /// <returns>True if the payment is verified, otherwise false.</returns>
        bool VerifyPayment(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature);


        /// <summary>
        /// Fetches the details of a payment from Razorpay API.
        /// </summary>
        /// <param name="paymentId">The payment ID from Razorpay.</param>
        /// <returns>The payment object containing details.</returns>
        Payment GetPaymentDetails(string paymentId);


        /// <summary>
        /// Refund the payment using Razorpay API.
        /// </summary>
        /// <param name="paymentId">The payment ID from Razorpay.</param>
        /// <param name="amount">The payment Amount.</param>
        /// <returns>The JsonResult object containing Refund details.</returns>
        JsonResult Refund(string paymentId, decimal amount);
    }

}
