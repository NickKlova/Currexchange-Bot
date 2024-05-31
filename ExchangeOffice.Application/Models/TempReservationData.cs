using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Application.Models {
	public class TempReservationData {
		public Guid ContactId { get; set; }
		public Rate Rate { get; set; }
		public Guid OperationId { get; set; }
		public Guid StatusId { get; set; }
		public decimal Amount { get; set; }
	}
}
