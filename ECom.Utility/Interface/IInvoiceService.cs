using ECom.Models.InvoiceModels;

namespace ECom.Utility.Interface
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoicePdf(InvoiceModel invoice);
        MemoryStream ViewInvoice(InvoiceModel invoice);
    }
}
