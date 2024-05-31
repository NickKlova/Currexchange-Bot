using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Application.Models;
using ExchangeOffice.Core.Managers.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Application.Handlers.Abstractions {
	public abstract class BaseReservationCreator : BaseStepperHandler {
		#region Fields: Private

		private readonly ICacheManager _cacheManager;
		private readonly IRateManager _rateManager;
		private readonly string _tempReservationDataKey;

		#endregion

		#region Contructors: Public

		public BaseReservationCreator(IManagerProvider provider) : base(provider) {
			_cacheManager = provider.GetCacheManager();
			_rateManager = provider.GetRateManager();
			_tempReservationDataKey = "TempReservationData";
		}

		#endregion

		#region Methods: Private

		private string GetTempReservationDataKey(string key) {
			return _tempReservationDataKey + ":" + key;
		}

		#endregion

		#region Methods: Protected

		protected async Task SetContactIdValue(string key, string contactId) {
			var cacheKey = GetTempReservationDataKey(key);
			var data = new TempReservationData() {
				ContactId = new Guid(contactId)
			};

			var json = JsonConvert.SerializeObject(data, Formatting.Indented);
			await _cacheManager.SetAsync(cacheKey, json);
		}

		protected async Task SetCurrencyValue(string key, string currencyId) {
			var rate = await _rateManager.GetRateAsync(key, currencyId);
			if (rate == null) {
				throw new Exception();
			}

			var cacheKey = GetTempReservationDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<TempReservationData>(json);
			if (entity == null) {
				throw new Exception();
			}

			entity.Rate = rate;
			var modifiedJson = JsonConvert.SerializeObject(entity, Formatting.Indented);
			await _cacheManager.SetAsync(cacheKey, modifiedJson);
		}

		protected async Task SetOperationTypeValue(string key, string operationTypeId) {
			var cacheKey = GetTempReservationDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<TempReservationData>(json);
			if (entity == null) {
				throw new Exception();
			}

			entity.OperationId = new Guid(operationTypeId);
			var modifiedJson = JsonConvert.SerializeObject(entity, Formatting.Indented);
			await _cacheManager.SetAsync(cacheKey, modifiedJson);
		}

		protected async Task SetAmountValue(string key, string amount) {
			var cacheKey = GetTempReservationDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<TempReservationData>(json);
			if (entity == null) {
				throw new Exception();
			}

			var isParcedToDecimal = decimal.TryParse(amount, out var decimalValue);
			if(!isParcedToDecimal) {
				throw new Exception();
			}

			entity.Amount = decimalValue;
			var modifiedJson = JsonConvert.SerializeObject(entity, Formatting.Indented);
			await _cacheManager.SetAsync(cacheKey, modifiedJson);
		}

		protected async Task<TempReservationData> GetTempReservationDataAsync(string key) {
			var cacheKey = GetTempReservationDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<TempReservationData>(json);
			if (entity == null) {
				throw new Exception();
			}
			return entity;
		}

		protected async Task ClearTempReservationDataAsync(string key) {
			var cacheKey = GetTempReservationDataKey(key);
			await _cacheManager.DeleteAsync(cacheKey);
		}

		#endregion
	}
}
