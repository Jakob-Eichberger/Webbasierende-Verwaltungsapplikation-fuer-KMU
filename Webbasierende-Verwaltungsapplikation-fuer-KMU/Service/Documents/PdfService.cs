using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.IO;
using System.Linq;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using migra = MigraDoc.DocumentObjectModel;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Documents
{
    public class PdfService
    {
        private readonly OrderService _orderService;
        TextFrame addressFrame = default!;
        Table table = default!;

        public PdfService(OrderService orderStatus) => _orderService = orderStatus;

        public MemoryStream GenerateInvoice(Order order)
        {
            migra.Document document = new();
            document.Info.Title = $"Rechnung_{order.Id}_{order.Customer.Fullname.Replace(" ", "_")}";
            document.Info.Subject = $"Rechnung für Auftrag {order.Id}";
            var @operator = _orderService.GetTable<Operator>()
                .Include(e => e.OfficeAddress)
                .FirstOrDefault() ?? throw new ApplicationException("Rechnungerstellung fehlgeschlagen.Operator existiert nicht!");
            DefineStyles(ref document);
            CreatePage(ref document, @operator);
            FillContent(ref document, order);
            PdfDocumentRenderer pdfRenderer = new(unicode: true);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            MemoryStream ms = new();
            pdfRenderer.PdfDocument.Save(ms, false);
            return ms;
        }

        private void FillContent(ref migra.Document document, Order order)
        {

            Paragraph paragraph = this.addressFrame.AddParagraph();
            paragraph.AddText(order.Customer.Fullname);
            paragraph.AddLineBreak();
            paragraph.AddText(order.BillingAddress.String);

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Format.Borders.Width = 0.75;
            paragraph.Format.Borders.Distance = 3;
            paragraph.Format.Borders.Color = Colors.Black;
            paragraph.Format.Shading.Color = Colors.White;
            paragraph.AddText("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.");

            var (listOfTickets, total) = _orderService.GetInvoiceSummary(order);
            Row r = this.table.AddRow();
            r.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            r.Cells[1].MergeRight = 3;
            paragraph = r.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Ticket", TextFormat.Bold);
            paragraph = r.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Beschreibung", TextFormat.Bold);
            paragraph = r.Cells[5].AddParagraph();
            paragraph.AddFormattedText("Total", TextFormat.Bold);

            foreach (var (ticketTotal, ticket) in listOfTickets)
            {
                Row row1 = this.table.AddRow();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].MergeRight = 3;
                row1.Cells[0].AddParagraph(ticket.Id.ToString());//TicketId
                row1.Cells[1].AddParagraph(ticket.Name);//Quantity
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                row1.Cells[5].AddParagraph($"{ticketTotal} €");//Quantity
                this.table.SetEdge(0, this.table.Rows.Count - 2, 5, 2, Edge.Box, BorderStyle.Single, 0.75);
            }

            // Add an invisible row as a space line to the table
            Row row = this.table.AddRow();
            row.Borders.Visible = false;

            // Add the VAT row
            var vat = (total * 0.20m);
            row = this.table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("VAT (20%)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph(vat.ToString("0.00") + " €");

            // Add the additional fee row
            row = this.table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Shipping and Handling");
            row.Cells[5].AddParagraph(0.ToString("0.00") + " €");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;

            // Add the total due row
            row = this.table.AddRow();
            row.Cells[0].AddParagraph("Total Due");
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph((vat + total).ToString("0.00") + " €");

            // Set the borders of the specified cell range
            this.table.SetEdge(5, this.table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);
        }

        private void CreatePage(ref migra.Document document, Operator @operator)
        {
            Section section = document.AddSection();
            Image image = section.Headers.Primary.AddImage(@"C:\Users\jakob\OneDrive\Schule_Spengergasse\Diplomprojekt\logo.jpg");
            image.Height = "2.5cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText($"{@operator.Name} · {@operator!.OfficeAddress}");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "5.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;
            paragraph = addressFrame.AddParagraph($"{@operator.Name} · {@operator!.OfficeAddress}");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Rechnung", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText($"{@operator!.OfficeAddress!.City}, ");
            paragraph.AddDateField("dd.MM.yyyy");
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Right;
        }

        private static void DefineStyles(ref migra.Document document)
        {
            Style style = document.Styles["Normal"];
            style.Font.Name = "Verdana";
            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
    }
}
