using ExchangeOffice.Cache.Clients.Interfaces;
using ExchangeOffice.Core.Managers.Interfaces;

namespace ExchangeOffice.Core.Managers {
	public class CacheManager : ICacheManager {
		#region Fields: Private

		private readonly ICacheClient _client;
		private readonly string _lastMsgPrefix;

		#endregion

		#region Constructors: Public

		public CacheManager(ICacheClient client) {
			_client = client;
			_lastMsgPrefix = "LastBotMessage";
		}

		#endregion

		#region Methods: Private

		private string GetLastMsgKey(string key) {
			return $"{_lastMsgPrefix}:{key}";
		}

		#endregion

		#region Methods: Public

		public async Task DeleteAsync(object key) {
			await _client.DeleteAsync(key);
		}

		public async Task<string?> GetAsync(object key) {
			return await _client.GetAsync(key);
		}

		public async Task<object> GetLastMessage(object key) {
			var stringifiedKey = key.ToString();
			if (string.IsNullOrEmpty(stringifiedKey)) {
				throw new Exception();
			}

			var cacheKey = GetLastMsgKey(stringifiedKey);
			var msgId = await GetAsync(cacheKey);
			return msgId == null ? new object() : msgId;
		}

		public async Task SaveMessageAsync(object key, object messageId) {
			var stringifiedKey = key.ToString();
			if (string.IsNullOrEmpty(stringifiedKey)) {
				throw new Exception();
			}

			var cacheKey = GetLastMsgKey(stringifiedKey);
			await SetAsync(cacheKey, messageId);
		}

		public async Task SetAsync(object key, object value) {
			await _client.SetAsync(key, value);
		}

		#endregion
	}
}
