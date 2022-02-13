using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service
{
    /// <summary>
    /// Class that services Order services to the front end.
    /// </summary>
    public class OrderService : BaseService
    {

        private readonly AuthService _authService;
        private readonly TicketService _ticketService;
        private readonly EMailService _eMailService;
        private readonly LinkGenerator _linkGenerator;

        /// <summary>
        /// Constructor that initializes a new Service object.
        /// </summary>
        /// <param name="db"></param>
        public OrderService(Database db, AuthService authService, TicketService ticketService, EMailService eMailService, LinkGenerator linkGenerator) : base(db)
        {
            _authService = authService;
            _ticketService = ticketService;
            _eMailService = eMailService;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Method to add an order object to the db.
        /// </summary>
        public async Task AddAsync(OrderDto o, Guid partyId)
        {
            if (!_authService.IsEmployee) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            var party = _db.Party?.FirstOrDefault(i => i.Guid == partyId) ?? throw new ApplicationException("Der User mit der Guid '' wurde nicht gefunden!");
            if (!(party.Role == Role.Customer || party.Role == Role.Company)) throw new ApplicationException("Es dürfen nur Privat Personen und Firmen Kunden sein!");
            var order = new Order
            {
                Guid = Guid.NewGuid(),
                Name = o.Name,
                Description = o.Description,
                Note = o.Note,
                DeliveryAddress = party.Address.Where(x => x.Id == o.DeliveryAddressId).FirstOrDefault() ?? throw new ApplicationException("Die Lieferaddresse wurde nicht gefunden!"),
                BillingAddress = party.Address.Where(x => x.Id == o.BillingAddressId).FirstOrDefault() ?? throw new ApplicationException("Die Rechnungsaddresse wurde nicht gefunden!"),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                Customer = party
            };
            await AddAsync(order);

        }

        public async Task AddAsync(Order order)
        {
            if (!_authService.IsEmployee) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            if (order.Customer is null) throw new ApplicationException("Eine Order ohne Party kann nicht hinzugefügt werden.");
            await base.AddAsync<Order>(order);
        }

        public async Task UpdateAsync(Order order)
        {
            if (!(_authService.IsEmployee || _authService.IsAdmin)) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            await base.UpdateAsync<Order>(order);
        }

        public void Update(Order order)
        {
            if (!(_authService.IsEmployee || _authService.IsAdmin)) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            base.Update<Order>(order);
        }

        /// <summary>
        /// Method to Update an order object in the db.
        /// </summary>
        public async Task UpdateAsync(Guid guid, OrderDto orderDto)
        {
            if (!_authService.IsEmployee) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");

            var found = _db.Order.SingleOrDefault(e => e.Guid == guid) ?? throw new ApplicationException("Die Order konte nicht aktualisiert werden!");
            if (found.OrderStatus != OrderStatus.Open) throw new ApplicationException("Die Order muss Open sein um diese zu bearbeiten");

            found.LastUpdated = DateTime.UtcNow;
            found.Name = orderDto.Name;
            found.Description = orderDto.Description;
            found.Note = orderDto.Note;
            //TODO: Check if the user has the address
            found.BillingAddress = _db?.Address?.Find(orderDto.BillingAddressId) ?? throw new ApplicationException("Die Addresse existiert nicht.");

            await base.UpdateAsync(found);
        }

        internal void SetOrderPayed(Guid orderGuid)
        {
            var found = base.GetTable<Order>()?.Include(e => e.Customer)?.ThenInclude(e => e.User).FirstOrDefault(e => e.Guid == orderGuid) ?? throw new ApplicationException($"Order mit guid {orderGuid} existiert kein oder mehr als einmal!");
            found.OrderStatus = OrderStatus.Closed;
            var link = _linkGenerator.GetPathByPage("/Order/Index", null, new { guid = orderGuid });
            _eMailService.SendEmail(found.Customer.User, $"Auftrag Nummer '{found.Id}'", $"Sehr geehrte Damen und Herren!\n\nHiermit informieren wir Sie,  dass der Auftrag mit der Nummer {found.Id} bezahlt wurde.\n\nIhre Rechnung finden Sie unter folgendem Link: {PageHelper.GetPageURL()}{link}#Dokumente");
            base.Update<Order>(found);
        }

        /// <summary>
        /// Method to delet an order object from the db.
        /// </summary>
        public async Task Delete(int orderId)
        {
            if (!_authService.IsEmployee) throw new ApplicationException("Die Operation konnte aufgrund von unzureichenden Rechten nicht ausgeführt werden.");
            var order = _db.Order.Include(i => i.Tickets).SingleOrDefault(e => e.Id == orderId) ?? throw new ApplicationException("Die Order konnte nicht gefunden werden.");

            if (order.Tickets.Count > 0) throw new ApplicationException("Eine Order die Tickets hat kann nicht gelöscht werden!");

            await base.DeleteAsync<Order>(order);
        }

        /// <summary>
        /// Method calculates the total for a given order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public (List<(decimal ticketTotal, Ticket ticket)> listOfTickets, decimal orderTotal) GetInvoiceSummary(Order order)
        {
            (List<(decimal ticketTotal, Ticket ticket)> listOfTickets, decimal orderTotal) val;
            val.listOfTickets = new();
            var found = base.GetTable<Order>().Include(e => e.Tickets).SingleOrDefault(e => e.Id == order.Id);
            if (found is null) throw new ApplicationException($"Order mit der Id '{order.Id}' wurde nicht gefunden.");
            val.orderTotal = 0;

            foreach (var ticket in found.Tickets)
            {
                decimal ticketTotal = _ticketService.GetTicketTotal(ticket);
                val.listOfTickets.Add(new() { ticket = ticket, ticketTotal = ticketTotal });
                val.orderTotal += ticketTotal;
            }
            return val;
        }

        /// <summary>
        /// Wrapper function to only return the total for a order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public decimal GetInvoiceTotal(Order order) => (GetInvoiceSummary(order)).orderTotal;

    }
}
