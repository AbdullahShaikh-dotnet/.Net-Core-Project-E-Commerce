namespace ECom.Models.InvoiceModels
{
    public class InvoiceModel
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string InvoiceNumber { get; set; } = Guid.NewGuid().ToString();
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
