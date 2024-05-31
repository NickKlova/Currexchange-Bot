using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using ExchangeOffice.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace ExchangeOffice.Core.Managers {
	public class ContactManager : IContactManager {
		#region Fields: Private

		private readonly ICacheManager _cacheManager;
		private readonly IContactService _contactService;
		private readonly string _prefixContactKey;
		#endregion

		#region Constructors: Public

		public ContactManager(ICacheManager cacheManager, IContactService contactService) {
			_cacheManager = cacheManager;
			_contactService = contactService;
			_prefixContactKey = "Contact";
		}

		#endregion

		#region Methods: Private

		private string GetPrefixContactKey(string key) {
			return _prefixContactKey + ":" + key;
		}

		#endregion

		#region Methods: Public

		public async Task<Contact> CreateContactAsync(Contact entity, string key) {
			var result = await _contactService.CreateContactAsync(entity);
			var json = JsonConvert.SerializeObject(result);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var cacheKey = GetPrefixContactKey(key);
			await _cacheManager.SetAsync(cacheKey, json);
			return result;
		}

		public async Task<Contact?> GetContactAsync(string key) {
			var cacheKey = GetPrefixContactKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				return null;
			}

			var entity = JsonConvert.DeserializeObject<Contact>(json);
			if (entity == null) {
				return null;
			}
			return entity;
		}
		
		#endregion
	}
}
