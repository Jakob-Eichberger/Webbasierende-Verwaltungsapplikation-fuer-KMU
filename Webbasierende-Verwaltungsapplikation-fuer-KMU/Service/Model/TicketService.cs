using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class TicketService : BaseService
    {
        private readonly AuthService _authService;
        /// <summary>
        /// Constructor that initializes a new Service object.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public TicketService(Database db, AuthService authService) : base(db)
        {
            _authService = authService;
        }

        /// <summary>
        /// Method to add an order object to the db.
        /// </summary>
        public async Task Add(TicketDto o, Guid orderGuid)
        {
            if (!_authService.IsEmployee) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            var order = _db.Order?.FirstOrDefault(i => i.Guid == orderGuid) ?? throw new ApplicationException("Die Order mit der Guid '' wurde nicht gefunden!");
            if (order.OrderStatus != OrderStatus.Open) throw new ApplicationException("Die Order muss Open sein um ein Ticket hinzuzufügen.");
            var ticket = new Ticket
            {
                Name = o.Name,
                Description = o.Description,
                LastUpdated = DateTime.UtcNow,
                Created = DateTime.UtcNow,
                Priority = o.Priority,
                SendNotification = true,
                Note = o.Note,
                EmployeeParty = _db.Party.Find(o.EmployeePartyId) ?? throw new ApplicationException("Die party Employee wurde nicht gefunden!"),
                Order = order,
                Status = _db.Status.Find(o.StatusId) ?? throw new ApplicationException("Der Status konnte leider nicht gefunden werden!")

            };
            await base.AddAsync<Ticket>(ticket);

        }

        public async Task Update(Ticket ticket)
        {
            await base.UpdateAsync<Ticket>(ticket);
        }


        /// <summary>
        /// Method to Update an order object in the db.
        /// </summary>
        public async Task Update(Guid guid, TicketDto t)
        {
            //Check if user is allowed to update the order here!;
            var found = GetTable<Ticket>().Include(e => e.Order).SingleOrDefault(e => e.Guid == guid) ?? throw new ApplicationException("Das Ticket konnte leider nicht aktualisiert werden!");
            if (found.Order.OrderStatus != OrderStatus.Open) throw new ApplicationException("Tickets können nur in offenen Orders geändert werden.");
            found.Name = t.Name;
            found.Description = t.Description;
            found.LastUpdated = DateTime.UtcNow;
            found.Priority = t.Priority;
            found.Note = t.Note;
            found.EmployeeParty = _db.Party.Find(t.EmployeePartyId) ?? throw new ApplicationException("Die Party Employee wurde leider nicht gefunden.");
            //found.Order = _db.Order.Find(t.OrderId) ?? throw new ApplicationException("Die Order konnte leider nicht gefunden werden.");
            found.Status = _db.Status.Find(t.StatusId) ?? throw new ApplicationException("Der Status konnte leider nicht gefunden werden.");

            await base.UpdateAsync<Ticket>(found);
        }

        /// <summary>
        /// Method to delet an order object from the db.
        /// </summary>
        public async Task Delete(Guid guid)
        {
            //Check if user can delete this Object here;

            var found = _db.Ticket.SingleOrDefault(e => e.Guid == guid) ?? throw new ApplicationException("Das Ticket konnte nicht gefunden werden. ");
            await base.DeleteAsync<Ticket>(found);
        }

        public decimal GetTicketTotal(Ticket ticket)
        {
            decimal total = 0;
            var found = base.GetTable<Ticket>().Include(e => e.Elements).Include(e => e.TimeRecordingElements).SingleOrDefault(e => e.Id == ticket.Id);
            if (found is null) throw new ApplicationException($"Order mit der Id '{ticket.Id}' wurde nicht gefunden.");
            foreach (var element in found.Elements)
            {
                if (Convert.ToDecimal(element.Ammount) != 0 && Convert.ToDecimal(element.PricePerItem) != 0)
                    total += Math.Round(Convert.ToDecimal(element.Ammount) * Convert.ToDecimal(element.PricePerItem), 2);
            }
            foreach (var timeElement in found.TimeRecordingElements)
            {
                if (Convert.ToDecimal(timeElement.PricePerHour) != 0 && Convert.ToDecimal(timeElement.Minutes) != 0)
                    total += Math.Round((Convert.ToDecimal(timeElement.PricePerHour) / 60) * Convert.ToDecimal(timeElement.Minutes), 2);
            }
            return total;
        }

        public async Task DeleteTagAsync(int id)
        {
            var found = GetTable<Tag>()?.FirstOrDefault(g => g.Id == id) ?? throw new ApplicationException($"Das Tag konnte nicht gefunden werden");
            await base.DeleteAsync(found);
        }

        public async Task AddTagAsync(int id, TagDto tagDto)
        {
            var Ticket = GetTable<Ticket>().FirstOrDefault(t => t.Id == id) ?? throw new ApplicationException($"Das Tag konnte nicht gefunden werden");
            Tag tag = new Tag()
            {
                Name = tagDto.Name
            };
            Ticket.Tags.Add(tag);
            tag.Ticket = Ticket;
            await base.AddAsync(tag);
            await base.UpdateAsync(Ticket);

        }
    }
}