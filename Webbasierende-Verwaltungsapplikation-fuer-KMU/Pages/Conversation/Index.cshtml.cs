using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Message
{
    public class IndexModel : PageModel
    {
        private readonly AuthService _authservice;
        private readonly ConversationService _conversationService;
        public Model.Conversation Conversation { get; set; } = default!;

        public IReadOnlyList<Model.Message> Messages { get; set; } = new List<Model.Message>();

        [BindProperty]
        public string Text { get; set; } = "";

        [FromRoute]
        public Guid Guid { get; set; }

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public IndexModel(AuthService authservice, ConversationService conversationService)
        {
            _authservice = authservice;
            _conversationService = conversationService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (_authservice.IsAdmin) return RedirectToPage("/404");
            Conversation = _conversationService.GetTable<Model.Conversation>().Where(c => c.Guid == guid).Include(c => c.Messages).ThenInclude(m => m.SendByParty).Include(e=>e.Order).FirstOrDefault() ?? throw new ApplicationException($"die Conversation mit der Guid'{guid}' existiert nicht!");
            Messages = Conversation.Messages;
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid guid)
        {
            try
            {
                if (!(_authservice.IsCustomerOrCompany || _authservice.IsEmployee)) return RedirectToPage("/404");
                if (string.IsNullOrEmpty(Text)) throw new ApplicationException("Eine Anfrage one Inhalt kann  nicht versendet werden!");
                Conversation = _conversationService
                    .GetTable<Model.Conversation>()
                    .Where(c => c.Guid == guid)
                    .FirstOrDefault(c => c.Guid == guid)
                    ?? throw new ApplicationException($"Die Anfragen mit der Guid'{guid}' existiert nicht!");
                await _conversationService.AddMessageToConversation(guid, new Dto.MessageDto() { Text = Text });
                SuccessMessage = "Die Antwort wurde erfolgreich gesendet!";
            }
            catch (ApplicationException e)
            {
                ErrorMessage = e.Message;
            }
            return RedirectToPage("/Conversation/Index", new { guid });
        }
    }
}
