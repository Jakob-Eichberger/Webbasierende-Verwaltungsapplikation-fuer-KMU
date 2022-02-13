using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Ticket
{
    public class IndexModel : PageModel
    {
        private readonly TicketService _ticketService;
        public readonly AuthService _authService;
        private readonly DocumentService _documentService;
        private readonly ElementService _elementService;

        [BindProperty]
        public string Time { get; set; } = default!;

        [BindProperty]
        public string Description { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public Model.Ticket Ticket { get; set; } = default!;

        [BindProperty]
        public IFormFile UploadedDocument { get; set; } = default!;

        [BindProperty]
        public int DocumentType { get; set; }

        [BindProperty]
        public Dto.TagDto TagDto { get; set; } = new();

        public IndexModel(TicketService ticketService, AuthService authService, DocumentService documentService, ElementService elementService)
        {
            _ticketService = ticketService;
            _authService = authService;
            _documentService = documentService;
            _elementService = elementService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!(_authService.IsCustomerOrCompany || _authService.IsEmployee)) return RedirectToPage("/404");
            var ticket = _ticketService.GetTable<Model.Ticket>()
                .Include(i => i.Status)
                .Include(i => i.Order)
                .ThenInclude(i => i.Customer)
                .Include(i => i.Elements)
                .Include(i => i.EmployeeParty)
                .Include(i => i.Documents)
                .Include(i => i.Tags)
                .SingleOrDefault(g => g.Guid == guid);

            if (ticket is null || (ticket.Order.CustomerId != _authService.PartyId && _authService.IsCustomerOrCompany)) return RedirectToPage("/404");
            Ticket = ticket;
            return Page();
        }

        public async Task<IActionResult> OnPostAddTimeElement(Guid guid)
        {
            try
            {
                if (Time == "0" || string.IsNullOrEmpty(Time)) throw new ApplicationException("Die gearbeitete Zeit darf nicht 0 sein!");
                if (!_authService.IsEmployee) return RedirectToPage("/404");
                var found = _ticketService.GetTable<Model.Ticket>().Include(i => i.TimeRecordingElements).Include(e => e.Order).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (found.Order.OrderStatus != OrderStatus.Open) return RedirectToPage("/404");
                var employee = _ticketService?.GetTable<Model.Employee>().Where(e => e.Role == Model.Role.Employee).FirstOrDefault(i => i.Id == _authService.PartyId) ?? throw new ApplicationException("Kein Nutzer mit ID gefunden");
                var element = new Model.TimeRecordingElement()
                {
                    Description = string.IsNullOrWhiteSpace(Description) ? throw new ApplicationException("Beschreibung darf nicht leer sein!") : Description,
                    Created = DateTime.UtcNow,
                    Minutes = Decimal.TryParse(Time, out decimal result) ? result : throw new ApplicationException("Bitte geben Sie"),
                    PricePerHour = employee.HourlyRate,
                    Party = employee,
                    Ticket = found
                };
                found.TimeRecordingElements.Add(element);
                await _ticketService.Update(found);
                SuccessMessage = "Eintrag wurde hinzugef�gt";
            }
            catch (ApplicationException e)
            {
                ErrorMessage = e.Message;

            }

            return RedirectToPage("/Ticket/Index", new { guid });
        }


        public async Task<IActionResult> OnPostUploadDocument(Guid guid)
        {
            if (!(_authService?.Rights?.CanUploadFiles ?? false)) return RedirectToPage("/404");
            try
            {
                if (UploadedDocument is null) return RedirectToPage("Index", new { guid });
                var type = _authService.IsEmployee ? (Model.DocumentType)DocumentType : Model.DocumentType.Standard;
                var found = _ticketService.GetTable<Model.Ticket>()?.Include(e => e.Order).SingleOrDefault(g => g.Guid == guid);
                if (found is null) throw new ApplicationException($"Order mit der Guid '{guid}' existiert nicht!");
                var maxFileSizeInBytes = 10000000;
                if (UploadedDocument.Length > maxFileSizeInBytes) throw new ApplicationException($"Dokument ist zu gro�! Die Maximale Filegr��e betr�gt  {maxFileSizeInBytes / 1000000} MB!");
                var document = await _documentService.AddAsync<Model.Ticket>(UploadedDocument, found, type);
                SuccessMessage = $"Das Dokuement {document.GetFullFileName} wurde erfolgreich hochgeladen!";
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
                var found = _ticketService?.GetTable<Model.Document>()?.Include(e => e.Ticket)?.ThenInclude(e => e!.Order)?.SingleOrDefault(g => g.Guid == documentGuid) ?? throw new ApplicationException($"Das Dokument mit der Guid '{documentGuid}' wurde nicht gefunden.");
                if (found.Ticket?.Order.OrderStatus != OrderStatus.Open) return RedirectToPage("/404");

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
                var found = _documentService?.GetTable<Document>()?.SingleOrDefault(g => g.Guid == documentGuid) ?? throw new ApplicationException($"Das File mit der Guid '{documentGuid}' konnt nicht gefunden werden");
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

        public async Task<IActionResult> OnPostDeleteElementAsync(Guid elementGuid, Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            try
            {
                var found = _documentService?.GetTable<Model.Element>()?.SingleOrDefault(g => g.Guid == elementGuid) ?? throw new ApplicationException($"Die E-Mail mit der Guid '{elementGuid}' konnt nicht gefunden werden");
                await _elementService.DeleteAsync(found.Id);
                SuccessMessage = "Das Element wurde erfolgreich gelöscht!";
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { guid });
        }

        public async Task<IActionResult> OnPostDeleteTagAsync(Guid tagGuid, Guid ticketGuid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            try
            {
                var found = _documentService?.GetTable<Model.Tag>()?.FirstOrDefault(g => g.Guid == tagGuid) ?? throw new ApplicationException($"Das Tag mit der Guid '{tagGuid}' konnt nicht gefunden werden");
                await _ticketService.DeleteTagAsync(found.Id);
                SuccessMessage = "Der Tag wurde erfolgreich gelöscht!";
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("Index", new { ticketGuid });
        }

        public async Task<IActionResult> OnPostAddTagAsync(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            if (!string.IsNullOrWhiteSpace(TagDto.Name))
            {
                try
                {
                    Model.Ticket ticket = _documentService?.GetTable<Model.Ticket>()?.FirstOrDefault(g => g.Guid == guid) ?? throw new ApplicationException($"Das Ticket mit der Guid '{guid}' konnt nicht gefunden werden");
                    await _ticketService.AddTagAsync(id: ticket.Id, tagDto: TagDto);
                    SuccessMessage = "Der Tag wurde erfolgreich hinzugefügt!";
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ErrorMessage = ex.Message;
                }
            }
            return RedirectToPage("Index", new { guid });
        }
    }
}