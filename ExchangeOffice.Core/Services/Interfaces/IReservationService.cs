using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Core.Services.Interfaces {
	public interface IReservationService {
		public Task<Reservation> CreateReservationAsync(InsertReservation data);
		public Task<IEnumerable<Reservation>> GetReservationsAsync(string contactId);
	}
}
