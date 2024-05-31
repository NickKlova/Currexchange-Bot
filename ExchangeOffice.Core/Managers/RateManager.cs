using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Cache.Clients.Interfaces;
using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Application.Managers {
	public class RateManager : IRateManager {
		#region Fields: Private

		private readonly ICacheClient _cache;
		private readonly IRateService _service;
		private readonly string _tempRateUniqKey;

		#endregion

		#region Contstructors: Public

		public RateManager(ICacheClient cache, IRateService service) {
			_cache = cache;
			_service = service;
			_tempRateUniqKey = "RatesTempData:";
		}

		#endregion

		#region Methods: Private

		private string GetCacheAccessKey(string key) {
			return _tempRateUniqKey + key;
		}

		#endregion

		#region Methods: Public

		public async Task<IEnumerable<Rate>> GetRatesAsync(string key) {
			var rates = await _service.GetRatesAsync();

			var cacheKey = GetCacheAccessKey(key);
			var cachedJson = JsonConvert.SerializeObject(rates, Formatting.Indented);
			await _cache.SetAsync(cacheKey, cachedJson);

			return rates;
		}

		public async Task<Rate> GetRateAsync(string key, string currencyId) {
			var cacheKey = GetCacheAccessKey(key);
			var json = await _cache.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception("json must be filled!");
			}
			var entities = JsonConvert.DeserializeObject<IEnumerable<Rate>>(json);
			if (entities == null) {
				throw new Exception("if json filled entities can't be empty");
			}
			var entity = entities.Where(x => x.Currency != null && x.Currency.Id == new Guid(currencyId)).FirstOrDefault();
			return entity == null ? new Rate() : entity;
		}

		#endregion
	}
}
