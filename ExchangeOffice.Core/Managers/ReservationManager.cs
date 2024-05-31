using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Interfaces;

namespace ExchangeOffice.Core.Managers {
	public class ReservationManager : IReservationManager {
		#region Fields: Private

		private readonly IReservationService _reservationService;

		#endregion

		#region Constructors: Public

		public ReservationManager(IReservationService reservationService) {
			_reservationService = reservationService;
		}

		#endregion

		#region Methods: Public

		public async Task<Reservation> CreateReservationAsync(InsertReservation data) {
			return await _reservationService.CreateReservationAsync(data);
		}

		public async Task<IEnumerable<Reservation>> GetReservationsAsync(string contactId) {
			return await _reservationService.GetReservationsAsync(contactId);
		}

		#endregion
	}
}
