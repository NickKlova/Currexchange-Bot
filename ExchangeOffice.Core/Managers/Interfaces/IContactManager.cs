using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Core.Managers.Interfaces {
	public interface IContactManager {
		public Task<Contact> CreateContactAsync(Contact entity, string key);
		public Task<Contact?> GetContactAsync(string key);
	}
}