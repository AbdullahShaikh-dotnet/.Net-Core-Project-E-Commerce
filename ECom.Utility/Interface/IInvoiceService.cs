using ECom.Models.InvoiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Utility.Interface
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoicePdf(InvoiceModel invoice);
        MemoryStream ViewInvoice(InvoiceModel invoice);
    }
}
