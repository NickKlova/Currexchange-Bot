namespace ExchangeOffice.Core.Managers.Interfaces {
	public interface ICacheManager {
		public Task<string?> GetAsync(object key);
		public Task SetAsync(object key, object value);
		public Task DeleteAsync(object key);
		public Task SaveMessageAsync(object key, object messageId);
		public Task<object> GetLastMessage(object key);
	}
}
