using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Abstractions;
using ExchangeOffice.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Core.Services {
	public class ContactService : BaseService, IContactService {
		public async Task<Contact> CreateContactAsync(Contact contact) {
			var response = await PostAsync("api/contact/create", contact);
			if (string.IsNullOrEmpty(response)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<Contact>(response);
			if (entity == null) {
				throw new Exception();
			}

			return entity;
		}
	}
}
