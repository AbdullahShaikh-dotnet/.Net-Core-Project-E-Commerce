using ECom.Models.InvoiceModels;
using ECom.Utility.Interface;
using ECom.Utility.Services;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Utility.Services
{
    public class InvoiceService : IInvoiceService
    {
        public byte[] GenerateInvoicePdf(InvoiceModel invoice)
        {
            var pdf = CreateInvoice(invoice);
            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            return stream.ToArray();
        }


        public MemoryStream ViewInvoice(InvoiceModel invoice)
        {
            var pdf = CreateInvoice(invoice);
            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;
            return stream;
        }


        private Document CreateInvoice(InvoiceModel invoice)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Add a border to the entire page content
                    page.Content()
                        .Border(0.5f) // Add a 1-unit border around the content
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);

                            // Add a border to the header
                            x.Item()
                                .Padding(10)
                                .Text("Invoice")
                                .SemiBold().FontSize(24).AlignCenter();

                            x.Item().BorderTop(0.5f).Table(table =>
                            {

                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(0.3f);
                                    columns.RelativeColumn();
                                });


                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(10).Text($"Invoice Number  : ");
                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(10).Text(invoice.InvoiceNumber);

                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(5).Text($"Invoice Date  : ");
                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(5).Text(invoice.InvoiceDate.ToString("dd-MM-yyyy"));

                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(5).Text($"Customer Name  : ");
                                table.Cell().PaddingRight(10).PaddingLeft(10).PaddingTop(5).Text(invoice.CustomerName);
                            });


                            // Add a border to the table
                            x.Item()
                                .Border(0.5f)
                                .Table(table =>
                                {

                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(0.5f).BorderVertical(0.5f).BorderTop(0.5f).Padding(5).Text("Description").AlignCenter();
                                        header.Cell().BorderBottom(0.5f).BorderVertical(0.5f).BorderTop(0.5f).Padding(5).Text("Quantity").AlignCenter();
                                        header.Cell().BorderBottom(0.5f).BorderVertical(0.5f).BorderTop(0.5f).Padding(5).Text("Unit Price").AlignCenter();
                                        header.Cell().BorderBottom(0.5f).BorderVertical(0.5f).BorderTop(0.5f).Padding(5).Text("Total Price").AlignCenter();
                                    });

                                    foreach (var item in invoice.Items)
                                    {
                                        table.Cell().BorderBottom(0.5f).BorderVertical(0.5f).Padding(5).Text(item.Title).AlignCenter();
                                        table.Cell().BorderBottom(0.5f).BorderVertical(0.5f).Padding(5).Text(item.Quantity.ToString()).AlignCenter();
                                        table.Cell().BorderBottom(0.5f).BorderVertical(0.5f).Padding(5).Text(item.UnitPrice.ToString("C")).AlignCenter();
                                        table.Cell().BorderBottom(0.5f).BorderVertical(0.5f).Padding(5).Text(item.TotalPrice.ToString("C")).AlignCenter();
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().Padding(5).Text("");
                                        footer.Cell().Padding(5).Text("");
                                        footer.Cell().Padding(5).Text("Total Amount  : ").AlignEnd();
                                        footer.Cell().Padding(5).Text(invoice.TotalAmount.ToString("C")).AlignCenter();
                                    });
                                });
                        });
                });
            });
            return pdf;
        }

    }
}
