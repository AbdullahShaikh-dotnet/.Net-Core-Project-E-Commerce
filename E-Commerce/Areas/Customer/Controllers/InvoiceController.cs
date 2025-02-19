using ECom.DataAccess.Repository.IRepository;
using ECom.Models.InvoiceModels;
using ECom.Utility.Interface;
using Microsoft.AspNetCore.Mvc;

[Area("Customer")]
public class InvoiceController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public InvoiceController(IInvoiceService invoiceService,
        IUnitOfWork unitOfwork,
        IUserService userService)
    {
        _invoiceService = invoiceService;
        _unitOfWork = unitOfwork;
        _userService = userService;
    }

    public IActionResult GenerateInvoice(int OrderID)
    {
        InvoiceModel invoice = GetInvoiceData(OrderID);
        return View(invoice);
    }

    public IActionResult DownloadInvoice(int OrderID)
    {
        InvoiceModel invoice = GetInvoiceData(OrderID);
        var pdfBytes = _invoiceService.GenerateInvoicePdf(invoice);
        return File(pdfBytes, "application/pdf", "Invoice.pdf");
    }

    private InvoiceModel GetInvoiceData(int OrderID)
    {
        var OrderHeaderDB = _unitOfWork.OrderHeaders.Get(data => data.ID == OrderID, includePropertiesList: "_ApplicationUser");
        var CustomerName = OrderHeaderDB._ApplicationUser.Name;
        var OrderDetailsDB = _unitOfWork.OrderDetails.GetAll(data => data.OrderHeaderID == OrderID);
        var InvoiceItemList = new List<InvoiceItem>();

        foreach (var OrderDetail in OrderDetailsDB)
        {
            var Price = OrderDetail.Price;
            var Quantity = OrderDetail.Count;
            var ProductDetailsDB = _unitOfWork.Product.Get(data => data.Id == OrderDetail.ProductID);
            InvoiceItem invoiceItem = new InvoiceItem
            {
                Title = ProductDetailsDB.Title,
                Quantity = Quantity,
                UnitPrice = (decimal)Price
            };
            InvoiceItemList.Add(invoiceItem);
        }

        var invoice = new InvoiceModel
        {
            OrderID = OrderID,
            OrderDate = OrderHeaderDB.OrderDate,
            CustomerName = CustomerName,
            Items = InvoiceItemList,
            TotalAmount = (decimal)OrderHeaderDB.OrderTotal,
            InvoiceNumber = OrderHeaderDB.InvoiceNumber,
            InvoiceDate = (DateTime)OrderHeaderDB.InvoiceDate
        };

        return invoice;
    }


    public IActionResult ViewInvoice(int OrderID)
    {
        InvoiceModel invoice = GetInvoiceData(OrderID);
        return File(_invoiceService.ViewInvoice(invoice), "application/pdf");
    }

}