using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Abstractions;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Models;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Application.Views.Constants;
using ExchangeOffice.Core.Managers.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Handlers.Contact.Text {
	[TextMessageHandler(MainMenuTitles.MyContact)]
	public class ContactMessageHandler : BaseStepperHandler, ITextHandler {
		#region Fields: Private
		
		private readonly ITelegramBotClient _bot;
		private readonly IContactManager _contactManager;
		private readonly ICacheManager _cacheManager;
		private readonly string _contactTempDataKey;

		#endregion

		#region Constructors: Public

		public ContactMessageHandler(IManagerProvider managerProvider) : base(managerProvider) {
			_bot = managerProvider.GetTelegramBotClient();
			_contactManager = managerProvider.GetContactManager();
			_cacheManager = managerProvider.GetCacheManager();
			_contactTempDataKey = "ContactTempData";
		}

		#endregion

		#region Methods: Private

		private string GetContactMsg(Core.Models.Contact contact) {
			return "📸  📸  📸\n\n" + 
				$"Особистий ключ: {contact.Id}\n" +
				$"ПІБ: {contact.FullName}\n" +
				$"Номер телефону: {contact.PhoneNumber}\n" +
				$"Пошта: {contact.Email}\n";
		}

		#endregion

		#region Methods: Private

		private string GetTempContactDataKey(string key) {
			return _contactTempDataKey + ":" + key;
		}

		private async Task SaveFullNameAsync(string key, string value) {
			var cacheKey = GetTempContactDataKey(key);
			var contact = new Core.Models.Contact() {
				FullName = value
			};
			var json = JsonConvert.SerializeObject(contact);
			await _cacheManager.SetAsync(cacheKey, json);
		}

		private async Task SavePhoneNumberAsync(string key, string value) {
			var cacheKey = GetTempContactDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<Core.Models.Contact>(json);
			if (entity == null) {
				throw new Exception();
			}

			entity.PhoneNumber = value;
			var resultJson = JsonConvert.SerializeObject(entity);
			await _cacheManager.SetAsync(cacheKey, resultJson);
		}

		private async Task<Core.Models.Contact> GetSavedContactWithEmail(string key, string value) {
			var cacheKey = GetTempContactDataKey(key);
			var json = await _cacheManager.GetAsync(cacheKey);
			if (string.IsNullOrEmpty(json)) {
				throw new Exception();
			}

			var entity = JsonConvert.DeserializeObject<Core.Models.Contact>(json);
			if (entity == null) {
				throw new Exception();
			}
			entity.Email = value;

			await _cacheManager.DeleteAsync(cacheKey);

			return entity;
		}

		#endregion

		#region Methods: Public

		[TextStepperHandler(MainMenuTitles.MyContact, 1)]
		public async Task ExecuteAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}
			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}
			var contact = await _contactManager.GetContactAsync(key);
			if (contact != null) {
				var registeredContactMessage = GetContactMsg(contact);
				await _bot.SendTextMessageAsync(chatId, registeredContactMessage, replyMarkup: MainMenu.GetKeyboard());
				return;
			}

			var config = new StepperInfo() {
				CurrentStep = 1,
				StepsCount = 4,
				Name = MainMenuTitles.MyContact,
			};
			await ConfigureStepperAsync(chatId, config);

			await _bot.SendTextMessageAsync(chatId, "Введіть ваше ПІБ:", replyMarkup: MainMenu.GetKeyboard());
		}

		[TextStepperHandler(MainMenuTitles.MyContact, 2)]
		public async Task ProcessUsernameParameterAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}
			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}

			var text = request?.Message?.Text;
			if (string.IsNullOrEmpty(text)) {
				return;
			}
			await SaveFullNameAsync(key, text);

			await _bot.SendTextMessageAsync(chatId, "Введіть ваш контактний номер телефону:", replyMarkup: MainMenu.GetKeyboard());
			await NextOrFinishStepAsync(chatId);
		}

		[TextStepperHandler(MainMenuTitles.MyContact, 3)]
		public async Task ProcessPhoneNumberParameterAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}
			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}

			var text = request?.Message?.Text;
			if (string.IsNullOrEmpty(text)) {
				return;
			}
			await SavePhoneNumberAsync(key, text);

			await _bot.SendTextMessageAsync(chatId, "Введіть вашу пошту:", replyMarkup: MainMenu.GetKeyboard());
			await NextOrFinishStepAsync(chatId);
		}

		[TextStepperHandler(MainMenuTitles.MyContact, 4)]
		public async Task FillEmail(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}
			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}
			var msg = await _bot.SendTextMessageAsync(chatId, "Додаємо ваш контакт у систему 🤭", replyMarkup: MainMenu.GetKeyboard());
			var text = request?.Message?.Text;
			if (string.IsNullOrEmpty(text)) {
				return;
			}
			
			var contact = await GetSavedContactWithEmail(key, text);
			await _contactManager.CreateContactAsync(contact, key);
			await _bot.DeleteMessageAsync(chatId, msg.MessageId);
			await _bot.SendTextMessageAsync(chatId, "Ми успішно додали ваш контакт у систему!", replyMarkup: MainMenu.GetKeyboard());
			await NextOrFinishStepAsync(chatId);
		}

		#endregion
	}
}
