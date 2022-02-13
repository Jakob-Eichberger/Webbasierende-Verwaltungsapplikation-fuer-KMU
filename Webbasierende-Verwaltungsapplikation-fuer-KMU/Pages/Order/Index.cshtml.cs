using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Documents;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Order
{
    public class IndexModel : PageModel
    {
        public Model.Order Order { get; set; } = default!;
        private readonly OrderService _orderService;
        private readonly DocumentService _documentService;
        private readonly PdfService _pdfService;
        public readonly AuthService _authService;

        #region Controll Variables for the HTMl Page
        //bool canEdit
        #endregion

        [BindProperty]
        public int DocumentType { get; set; }

        [TempData]
        public string ErrorMessage { get; set; } = null!;
        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public bool InvoiceCreated { get; set; } = true;

        public bool AllTicketsClosed { get; set; } = false;

        [BindProperty]
        public IFormFile UploadedDocument { get; set; } = default!;

        public IndexModel(AuthService authService, OrderService orderService, DocumentService documentService, PdfService pdfService)
        {
            _authService = authService;
            _pdfService = pdfService;
            _orderService = orderService;
            _documentService = documentService;
        }

        public IActionResult OnGet(Guid guid)
        {
            try
            {
                if (_authService.IsCustomerOrCompany || _authService.IsEmployee)
                {
                    Order = _orderService?.GetTable<Model.Order>()?
                    .Include(e => e.DeliveryAddress)?
                    .Include(e => e.Tickets)?
                    .ThenInclude(t => t.Status)?
                    .Include(e => e.Documents)?
                    .Include(e => e.Customer)?
                    .Include(e => e.Conversations)?
                    .ThenInclude(e => e.Messages)?
                    .FirstOrDefault(o => o.Guid == guid) ?? default!;
                    if (Order is null) return RedirectToPage("/404");
                    if (_authService.IsCustomerOrCompany && (Order.CustomerId != _authService.PartyId)) return RedirectToPage("/404");
                    AllTicketsClosed = !(Order.Tickets.Where(x => x.Status.Sequence > 0).ToList().Any());
                    InvoiceCreated = Order.Documents.Where(e => e.DocumentType == Model.DocumentType.Invoice).Any();
                }
                else
                {
                    return RedirectToPage("/404");
                }

            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUploadDocument(Guid guid)
        {
            if (!(_authService?.Rights?.CanUploadFiles ?? false)) return RedirectToPage("/404");
            try
            {
                if (UploadedDocument is null) return RedirectToPage("Index", new { guid });
                var type = _authService.IsEmployee ? (Model.DocumentType)DocumentType : Model.DocumentType.Standard;
                var found = _orderService.GetTable<Model.Order>()?.SingleOrDefault(g => g.Guid == guid);
                if (!(found is object)) throw new ApplicationException($"Order mit der Guid '{guid}' existiert nicht!");
                var maxFileSizeInBytes = 10000000;
                if (UploadedDocument.Length > maxFileSizeInBytes) throw new ApplicationException($"Dokument ist zu groß! Die Maximale Filegröße beträgt  {maxFileSizeInBytes / 1000000} MB!");
                var document = await _documentService.AddAsync<Model.Order>(UploadedDocument, found, type);
                SuccessMessage = $"Das Dokuement {document.GetFullFileName} wurde erfolgreich hochgeladen!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { guid });
        }

        public async Task<IActionResult> OnPostRechnungGenerieren(Guid guid)
        {
            try
            {
                if (!_authService.IsEmployee) return RedirectToPage("/404");

                var found = _documentService.GetTable<Model.Order>().Include(i => i.Documents).Include(i => i.Tickets).Include(e => e.Customer).Include(e => e.BillingAddress).SingleOrDefault(e => e.Guid == guid);

                if (found is null) throw new ApplicationException("Das Dokument wurde nicht gefunden.");
                if (found.OrderStatus != OrderStatus.Open || !found.Tickets.Any()) return RedirectToPage("/404");

                found.Documents.Add(await _documentService.AddAsync<Model.Order>(_pdfService.GenerateInvoice(found), $"Rechnung_{found.Id}_{found.Customer.Fullname.Replace(" ", "_")}.pdf", found, Model.DocumentType.Invoice));

                found.OrderStatus = OrderStatus.AwaitingPayment;

                await _orderService.UpdateAsync(found);

                return RedirectToPage("Index", new { guid });
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { guid });

        }

        public async Task<IActionResult> OnPostDeleteDocument(Guid documentGuid, Guid guid)
        {
            if (!_authService.UserIsAuthenticated) return RedirectToPage("/404");
            try
            {
                var found = _orderService?.GetTable<Model.Document>()?.Include(e => e.Order)?.SingleOrDefault(g => g.Guid == documentGuid) ?? throw new ApplicationException($"Das Dokument mit der Guid '{documentGuid}' wurde nicht gefunden.");
                if (found.Order!.OrderStatus != Model.OrderStatus.Open) RedirectToPage("/404");
                await _documentService.DeleteAsync(found);
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { guid });
        }

        public async Task<IActionResult> OnPostDownloadDocument(Guid documentGuid, Guid guid)
        {
            if (!_authService.UserIsAuthenticated) return RedirectToPage("/404");
            try
            {
                var found = _documentService?
                    .GetTable<Document>()?
                    .SingleOrDefault(g => g.Guid == documentGuid) ?? throw new ApplicationException($"Das File mit der Guid '{documentGuid}' konnt nicht gefunden werden");
                var fileStream = await _documentService.GetDocumentAsync(found);
                return File(fileStream, "application/octet-stream", found.GetFullFileName);
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { guid });
        }
    }
}
