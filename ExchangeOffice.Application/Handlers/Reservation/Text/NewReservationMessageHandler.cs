using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Constants;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Abstractions;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Application.Views.Constants;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExchangeOffice.Application.Handlers.Reservation.Text {
	[TextMessageHandler(ReservationMenuTitles.CreateReservation)]
	public class NewReservationMessageHandler : BaseReservationCreator, ITextHandler {
		#region Fields: Private

		private ITelegramBotClient _bot;
		private IRateManager _rateManager;
		private IContactManager _contactManager;
		private ICacheManager _cacheManager;
		private string _cbPrefixReservationCurrency;

		#endregion

		#region Constructors: Public

		public NewReservationMessageHandler(IManagerProvider provider) : base(provider) {
			_bot = provider.GetTelegramBotClient();
			_rateManager = provider.GetRateManager();
			_contactManager = provider.GetContactManager();
			_cacheManager = provider.GetCacheManager();
			_cbPrefixReservationCurrency = Callbacks.ReservationCurrency;
		}

		#endregion

		#region Methods: Private

		public static InlineKeyboardMarkup GetKeyboard(IDictionary<string, string> values) {
			var keyboard = new List<List<InlineKeyboardButton>>();
			foreach (var key in values.Keys) {
				var row = new List<InlineKeyboardButton>();
				var value = values[key];
				var button = InlineKeyboardButton.WithCallbackData(text: value, callbackData: key);
				row.Add(button);
				keyboard.Add(row);
			}
			return new InlineKeyboardMarkup(keyboard);
		}

		private InlineKeyboardMarkup GetAcceptedCurrenciesKeyboard(IEnumerable<Core.Models.Rate> rates) {
			var currencies = rates.Select(x => x.Currency).Where(x => x != null).Cast<Currency>();
			var dictionarifiedValues = GetDictionarifiedValues(currencies);
			return GetKeyboard(dictionarifiedValues);
		}

		private IDictionary<string, string> GetDictionarifiedValues(IEnumerable<Currency> currencies) {
			var dictionary = new Dictionary<string, string>();
			foreach (var currency in currencies) {
				var key = _cbPrefixReservationCurrency + "|" + currency.Id.ToString();
				var value = currency.Code == null ? "none" : currency.Code;
				dictionary.TryAdd(key, value);
			}
			return dictionary;
		}

		#endregion

		#region Methods: Public

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
			if (contact == null) {
				await _bot.SendTextMessageAsync(chatId, text: "Додайте контакт у систему, без нього неможливо створити бронювання 😥", replyMarkup: MainMenu.GetKeyboard());
				return;
			}
			await SetContactIdValue(key, contact.Id.ToString());

			var rates = await _rateManager.GetRatesAsync(key);
			if (rates.Count() == 0) {
				await _bot.SendTextMessageAsync(chatId, text: "Напишить пізніше, обмінник нічого не може вам запропонувати 😥", replyMarkup: ReservationMenu.GetKeyboard());
				return;
			}

			var keyboard = GetAcceptedCurrenciesKeyboard(rates);
			var msg = await _bot.SendTextMessageAsync(chatId, text: "Оберіть валюту з запропонованих:", replyMarkup: keyboard);
			await _cacheManager.SaveMessageAsync(key, msg.MessageId);
		}

		#endregion
	}
}
