using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class ConversationService : BaseService
    {
        PartyService _partyService;
        AuthService _authService;

        public ConversationService(Database db, PartyService partyService, AuthService authService) : base(db)
        {
            _partyService = partyService;
            _authService = authService;
        }

        public async Task<Task> NewConversation(MessageDto messageDto, int? orderId = null)
        {
            try
            {
                if (!_authService.IsCustomerOrCompany) throw new ApplicationException($"User darf keine neue Conversation erstellen!");
                Party party = base.GetTable<Party>()?.FirstOrDefault(e => e.Id == _authService.PartyId) ?? throw new ApplicationException($"Party mit id '{_authService.PartyId}' nicht gefunden!");

                Conversation conv = new() { CreatedByPartyId = party.Id };
                Message message = new()
                {
                    CreatedBy = MessageCreatedBy.Customer,
                    SendByParty = party,
                    Text = messageDto.Text
                };
                conv.Messages.Add(message);
                if (orderId is not null) conv.OrderId = orderId;
                await base.AddAsync<Conversation>(conv);
                return Task.CompletedTask;
            }
            catch (ApplicationException ex) { return Task.FromException(ex); }
        }

        public async Task AddMessageToConversation(Guid conversation, MessageDto messageDto)
        {
            Conversation found = base.GetTable<Conversation>()?.FirstOrDefault(e => e.Guid == conversation) ?? throw new ApplicationException($"Conversation mit guid '{conversation}' nicht gefunden!");
            Party party = base.GetTable<Party>()?.FirstOrDefault(e => e.Id == _authService.PartyId) ?? throw new ApplicationException($"Party mit id '{_authService.PartyId}' nicht gefunden!");
            Message message = new();

            if (party.Role == Role.Employee)
                message.CreatedBy = MessageCreatedBy.Employee;
            else
                message.CreatedBy = MessageCreatedBy.Customer;

            message.SendByParty = party;
            message.Text = messageDto.Text;
            found.Messages.Add(message);
            await base.UpdateAsync<Conversation>(found);
        }
    }
}
