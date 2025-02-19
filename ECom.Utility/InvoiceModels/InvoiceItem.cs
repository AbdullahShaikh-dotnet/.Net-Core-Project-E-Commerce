namespace ECom.Models.InvoiceModels
{
    public class InvoiceItem
    {
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

    }
}
