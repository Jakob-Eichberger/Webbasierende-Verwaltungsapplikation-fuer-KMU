using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class AddressService : BaseService
    {

        /// <summary>
        /// Constructor that initializes a new Service object.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public AddressService(Database db) : base(db)
        {
        }

        /// <summary>
        /// Method to add an order object to the db.
        /// </summary>
        public async Task AddAsync(AddressDto dto, int id)
        {
            var address = new Address
            {
                Country = dto.Country,
                State = dto.State,
                ZipCode = dto.ZipCode,
                City = dto.City,
                Street = !string.IsNullOrEmpty(dto.Street) ? dto.Street : null,
                StairCase = !string.IsNullOrEmpty(dto.StairCase) ? dto.StairCase : null,
                HouseNumber = dto.HouseNumber,
                DoorNumber = !string.IsNullOrEmpty(dto.DoorNumber) ? dto.DoorNumber : null,
            };
            if (dto.IsPrimary)
            {
                var foundP = await GetTable<Party>().Where(p => p.Id == id).Include(p => p.Address).SingleOrDefaultAsync();
                if (foundP != null)
                {
                    foundP.Address.ForEach(a => a.IsPrimary = false);
                    await base.UpdateAsync(foundP);
                    address.IsPrimary = true;
                }
            }
            await base.AddAsync<Address>(address);
            var found = await GetTable<Party>().SingleOrDefaultAsync(p => p.Id == id);
            found.Address.Add(address);
            await base.UpdateAsync(found);
        }

        /// <summary>
        /// Method to Update an order object in the db.
        /// </summary>
        public async Task UpdateAsync(Guid guid, AddressDto dto)
        {
            var found = _db.Address.SingleOrDefault(e => e.Guid == guid) ?? throw new ApplicationException("Die Addresse konnte nicht gefunden werden.");
            found.Country = dto.Country;
            found.State = dto.State;
            found.ZipCode = dto.ZipCode;
            found.City = dto.City;
            found.Street = !string.IsNullOrEmpty(dto.Street) ? dto.Street : null;
            found.StairCase = !string.IsNullOrEmpty(dto.StairCase) ? dto.StairCase : null;
            found.HouseNumber = dto.HouseNumber;
            found.DoorNumber = !string.IsNullOrEmpty(dto.DoorNumber) ? dto.DoorNumber : null;
            if (dto.IsPrimary)
            {
                var foundP = await GetTable<Party>().Where(p => p.Id == found.PartyId).Include(p => p.Address).SingleOrDefaultAsync();
                if (foundP != null)
                {
                    foundP.Address.ForEach(a => a.IsPrimary = false);
                    await base.UpdateAsync(foundP);
                    found.IsPrimary = true;
                }
            }
            await base.UpdateAsync<Address>(found);
        }

        /// <summary>
        /// Method to delete an order object from the db.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var found = _db.Address.FirstOrDefault(e => e.Id == id) ?? throw new ApplicationException("Die Adresse existiert nicht.");
            if (found.IsPrimary) throw new ApplicationException("Die Primäre Adresse darf nicht gelöscht werden.");
            await base.DeleteAsync<Address>(found);
        }
    }
}
