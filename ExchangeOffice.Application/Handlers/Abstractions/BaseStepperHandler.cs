using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Models;
using ExchangeOffice.Core.Managers.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Application.Handlers.Abstractions {
	public abstract class BaseStepperHandler {
		#region Fields: Private

		private readonly ICacheManager _cacheManager;

		#endregion

		#region Contructors: Public

		public BaseStepperHandler(IManagerProvider provider) {
			_cacheManager = provider.GetCacheManager();
		}

		#endregion

		#region Methods: Protected

		protected async Task ConfigureStepperAsync(object key, StepperInfo config) {
			var stringKey = key.ToString();
			if (stringKey == null) {
				throw new Exception();
			}
			await NextOrFinishStepAsync(stringKey, config);
		}

		protected async Task DeleteStepperAsync(object key) {
			var stringKey = key.ToString();
			if (string.IsNullOrEmpty(stringKey)) {
				return;
			}
			await _cacheManager.DeleteAsync(stringKey);
		}

		protected async Task NextOrFinishStepAsync(object key, StepperInfo? value = null) {
			var stringKey = key.ToString();
			if (string.IsNullOrEmpty(stringKey)) {
				return;
			}
			if (value == null) {
				var valueJson = await _cacheManager.GetAsync(stringKey);
				if (valueJson == null) {
					return;
				}
				value = JsonConvert.DeserializeObject<StepperInfo>(valueJson);
				if (value == null) {
					return;
				}
			}
			value!.CurrentStep++;
			if (value.CurrentStep > value.StepsCount) {
				await _cacheManager.DeleteAsync(stringKey);
				return;
			}
			var jsonValue = JsonConvert.SerializeObject(value);
			await _cacheManager.SetAsync(stringKey, jsonValue);
		}

		#endregion
	}
}
