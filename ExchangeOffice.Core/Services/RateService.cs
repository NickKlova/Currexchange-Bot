using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Abstractions;
using ExchangeOffice.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Core.Services {
	public class RateService : BaseService, IRateService {
		public async Task<IEnumerable<Rate>> GetRatesAsync() {
			var json = await GetAsync("api/rate/get/all");
			var entities = JsonConvert.DeserializeObject<IEnumerable<Rate>>(json);
			if (entities == null) {
				throw new Exception();
			}
			return entities;
		}
	}
}
