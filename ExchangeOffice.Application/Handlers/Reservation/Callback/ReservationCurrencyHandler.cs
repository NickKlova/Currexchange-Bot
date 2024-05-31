using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Constants;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Abstractions;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExchangeOffice.Application.Handlers.Reservation.Callback {
	[CallbackMessageHandler(Callbacks.ReservationCurrency)]
	public class ReservationCurrencyHandler : BaseReservationCreator, ICallbackHandler {
		#region Fields: Private

		private readonly ITelegramBotClient _bot;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors: Public

		public ReservationCurrencyHandler(IManagerProvider provider) : base(provider) {
			_bot = provider.GetTelegramBotClient();
			_cacheManager = provider.GetCacheManager();
		}

		#endregion

		#region Methods: Private

		private InlineKeyboardMarkup GetOperationTypeKeyboard() {
			var buttons = new List<List<InlineKeyboardButton>>() {
				new List<InlineKeyboardButton>() {
					InlineKeyboardButton.WithCallbackData(text: "Продати", callbackData: $"{Callbacks.OperationType}|{Constants.OperationType.Buy}")
				},new List<InlineKeyboardButton>() {
					InlineKeyboardButton.WithCallbackData(text: "Купити", callbackData: $"{Callbacks.OperationType}|{Constants.OperationType.Sell}")
				}
			};
			return new InlineKeyboardMarkup(buttons);
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
			await SetCurrencyValue(key, currencyId);

			var lastMessageId = await _cacheManager.GetLastMessage(key);
			await _bot.DeleteMessageAsync(chatId, messageId: Convert.ToInt32(lastMessageId));

			var message = await _bot.SendTextMessageAsync(chatId, text: "Оберіть тип операції:", replyMarkup: GetOperationTypeKeyboard());
			await _cacheManager.SaveMessageAsync(key, message.MessageId);
		}

		#endregion
	}
}
