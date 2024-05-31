using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Application.Managers.Interfaces {
	public interface IRateManager {
		public Task<IEnumerable<Rate>> GetRatesAsync(string key);
		public Task<Rate> GetRateAsync(string key, string currencyId);
	}
}
