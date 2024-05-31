using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Application.Views;
using Telegram.Bot.Types;
using Telegram.Bot;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Application.Constants;

namespace ExchangeOffice.Application.Handlers.Rate.Callback {
	[CallbackMessageHandler(Callbacks.RateCurrency)]
	public class RateCurrencyHandler : ICallbackHandler {
		#region Fields: Private

		private readonly ITelegramBotClient _bot;
		private readonly IRateManager _manager;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors: Public

		public RateCurrencyHandler(IManagerProvider provider) {
			_bot = provider.GetTelegramBotClient();
			_manager = provider.GetRateManager();
			_cacheManager = provider.GetCacheManager();
		}

		#endregion

		#region Methods: Private

		private string GetRateTextMessage(Core.Models.Rate data) {
			var currencyCode = data.Currency?.Code;
			var currencyDescription = data.Currency?.Description;
			var sellRate = data.SellRate;
			var buyRate = data.BuyRate;
			if (currencyCode == null || currencyDescription == null) {
				return "Щось пішло не так... 😢";
			}
			return $"💸\t💸\t💸\n\n\n" +
				$"Валюта: {currencyCode}\n" +
				$"Опис: {currencyDescription}\n\n" +
				$"🔺 Обмінник продає по: {sellRate}₴\n" +
				$"🔻 Обмінник купує по: {buyRate}₴";
		}

		#endregion

		#region Methods: Public

		public async Task ExecuteAsync(Update request) {
			var chatId = request?.CallbackQuery?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}

			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}

			var callback = request?.CallbackQuery?.Data;
			if (string.IsNullOrEmpty(callback)) {
				return;
			}

			var splittedCallback = callback.Split('|');
			if (splittedCallback.Length != 2) {
				return;
			}

			var currencyId = splittedCallback[1];
			var rate = await _manager.GetRateAsync(key, currencyId);
			var rateTextMsg = GetRateTextMessage(rate);

			var lastMessageId = await _cacheManager.GetLastMessage(key);
			await _bot.DeleteMessageAsync(chatId, messageId: Convert.ToInt32(lastMessageId));

			await _bot.SendTextMessageAsync(chatId, text: rateTextMsg, replyMarkup: MainMenu.GetKeyboard());
		}

		#endregion
	}
}
