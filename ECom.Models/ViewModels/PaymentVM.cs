namespace ECom.Models.ViewModels
{
    public class PaymentVM
    {
        public string Key { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; } = "INR";
        public int OrderID { get; set; }
        public string? razorpay_payment_id { get; set; }
        public string? razorpay_order_id { get; set; }
        public string? razorpay_signature { get; set; }
        public bool isPaymentSuccessfull { get; set; } = false;
    }
}
