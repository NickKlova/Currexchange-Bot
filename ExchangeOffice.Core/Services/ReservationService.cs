using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Abstractions;
using ExchangeOffice.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Core.Services {
	public class ReservationService : BaseService, IReservationService {
		public async Task<Reservation> CreateReservationAsync(InsertReservation data) {
			var json = await PostAsync("api/reservation/create", data);
			var entity = JsonConvert.DeserializeObject<Reservation>(json);
			if (entity == null) {
				throw new Exception();
			}
			return entity;
		}

		public async Task<IEnumerable<Reservation>> GetReservationsAsync(string contactId) {
			var json = await GetAsync("api/reservation/getall");
			var entities = JsonConvert.DeserializeObject<IEnumerable<Reservation>>(json);
			if (entities == null) { 
				throw new Exception(); 
			}
			var entitiesByContact = entities.Where(x=>x.Contact != null && x.Contact.Id == new Guid(contactId));
			return entitiesByContact;
		}
	}
}
