using ExchangeOffice.Core.Models;

namespace ExchangeOffice.Core.Services.Interfaces {
	public interface IContactService {
		public Task<Contact> CreateContactAsync(Contact contact);
	}
}
