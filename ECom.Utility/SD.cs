namespace ECom.Utility
{
    public static class SD
    {

        /// <summary>
        /// Role Status Admin 
        /// </summary>
        public const string Role_Admin = "Admin";

        /// <summary>
        /// Role Status Employee 
        /// </summary>
        public const string Role_Employee = "Employee";

        /// <summary>
        /// Role Status Customer 
        /// </summary>
        public const string Role_Customer = "Customer";

        /// <summary>
        /// Role Status Company 
        /// </summary>
        public const string Role_Company = "Company";


        /// <summary>
        /// Shopping Cart Session Key 
        /// </summary>
        public const string ShoppingCartSessionKey = "ShoppingCartSession";



        /// <summary>
        /// Order Status Pending 
        /// </summary>
        public const string Status_Pending = "Pending";

        /// <summary>
        /// Order Status Approved 
        /// </summary>
        public const string Status_Approved = "Approved";

        /// <summary>
        /// Order Status In Processing 
        /// </summary>
        public const string Status_In_Process = "Processing";

        /// <summary>
        /// Order Status Shipped 
        /// </summary>
        public const string Status_Shipped = "Shipped";

        /// <summary>
        /// Order Status Cancelled 
        /// </summary>
        public const string Status_Cancelled = "Cancelled";

        /// <summary>
        /// Order Status Refunded 
        /// </summary>
        public const string Status_Refunded = "Refunded";



        /// <summary>
        /// Payment Status Pending 
        /// </summary>
        public const string Payment_Status_Pending = "Pending";

        /// <summary>
        /// Payment Status Approved 
        /// </summary>
        public const string Payment_Status_Approved = "Approved";

        /// <summary>
        /// Payment Status Approved for Delayed Payment 
        /// </summary>
        public const string Payment_Status_Delayed_Payment = "ApprovedForDelayedPayment";

        /// <summary>
        /// Payment Status Rejected 
        /// </summary>
        public const string Payment_Status_Rejected = "Rejected";

        /// <summary>
        /// Maximum Count of API Request Per Second
        /// </summary>
        public const int RATE_LIMITING_COUNT = 5;
    }
}
