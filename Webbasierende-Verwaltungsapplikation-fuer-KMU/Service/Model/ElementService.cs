using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class ElementService : BaseService
    {
        public AuthService _authService;
        public ElementService(Database db, AuthService authService) : base(db)
        {
            _authService = authService;
        }

        public async Task AddAsync(Dto.ElementDto elementDto, Guid ticketGuid)
        {
            //if (!_authService.IsEmployee) throw new ApplicationException();
            var element = new Element
            {
                Description = elementDto.Description,
                Ammount = decimal.TryParse(elementDto.Ammount, out decimal ammountValue) ? ammountValue.ToString() : throw new ApplicationException("Anzahl muss eine Zahl sein."),
                PricePerItem = elementDto.Price,
                Created = DateTime.UtcNow,
                Ticket = base.GetTable<Ticket>().FirstOrDefault(i => i.Guid == ticketGuid) ?? throw new ApplicationException("Ticket existiert nicht.")
            };

            await base.AddAsync<Element>(element);
        }

        public async Task UpdateAsync(ElementDto elementDto, Guid elementGuid)
        {
            var found = base.GetTable<Element>().FirstOrDefault(i => i.Guid == elementGuid) ?? throw new ApplicationException("Element nicht gefunden.");
            found.Description = elementDto.Description;
            found.Ammount = elementDto.Ammount;
            found.PricePerItem = elementDto.Price;

            await base.UpdateAsync<Element>(found);
        }
        public async Task DeleteAsync(int Id)
        {
            var found = base.GetTable<Element>().FirstOrDefault(i => i.Id == Id) ?? throw new ApplicationException("Element nicht gefunden.");
            await base.DeleteAsync(found);
        }

    }
}
