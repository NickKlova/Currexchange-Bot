using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Core.Managers.Interfaces {
	public interface IReservationManager {
		public Task<Reservation> CreateReservationAsync(InsertReservation data);

		public Task<IEnumerable<Reservation>> GetReservationsAsync(string contactId);
	}
}
