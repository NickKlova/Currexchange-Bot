using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Application.Views.Constants;

namespace ExchangeOffice.Application.Handlers.Rate.Text {
	[TextMessageHandler(MainMenuTitles.Rates)]
	public class RateMessageHandler : ITextHandler {
		#region Fields: Private

		private ITelegramBotClient _bot;
		private IRateManager _rateManager;
		private ICacheManager _cacheManager;
		private string _cbCurrencyPrefix;

		#endregion

		#region Constructors: Public
		
		public RateMessageHandler(IManagerProvider provider) {
			_bot = provider.GetTelegramBotClient();
			_rateManager = provider.GetRateManager();
			_cacheManager = provider.GetCacheManager();
			_cbCurrencyPrefix = "RateCurrency";
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
				var key = _cbCurrencyPrefix + "|" + currency.Id.ToString();
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

			var loadingMessage = await _bot.SendStickerAsync(chatId, sticker: InputFile.FromFileId("CAACAgIAAxkBAAEMKDdmTIUGYME5y_0bBsx_6NRc8lsoVAACRF4AAi3haEi5O6MC2jMvlTUE"));
			var rates = await _rateManager.GetRatesAsync(key);
			await _bot.DeleteMessageAsync(chatId, loadingMessage.MessageId);

			if (rates.Count() == 0) {
				await _bot.SendTextMessageAsync(chatId, text: "test", replyMarkup: MainMenu.GetKeyboard());
				return;
			}

			var keyboard = GetAcceptedCurrenciesKeyboard(rates);
			var msg = await _bot.SendTextMessageAsync(chatId, text: "test", replyMarkup: keyboard);
			await _cacheManager.SaveMessageAsync(key, msg.MessageId);
		}

		#endregion
	}
}
