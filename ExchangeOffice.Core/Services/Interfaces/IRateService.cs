using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Core.Services.Interfaces {
	public interface IRateService {
		public Task<IEnumerable<Rate>> GetRatesAsync();
	}
}
