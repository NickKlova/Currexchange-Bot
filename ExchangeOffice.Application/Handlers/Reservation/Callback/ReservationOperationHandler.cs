using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Constants;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Abstractions;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Models;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Application.Views.Constants;
using ExchangeOffice.Core.Managers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExchangeOffice.Application.Handlers.Reservation.Callback {
	[CallbackMessageHandler(Callbacks.OperationType)]
	public class ReservationOperationHandler : BaseReservationCreator, ICallbackHandler {
		#region Fields: Private

		private readonly ITelegramBotClient _bot;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors: Public

		public ReservationOperationHandler(IManagerProvider provider) : base(provider) {
			_bot = provider.GetTelegramBotClient();
			_cacheManager = provider.GetCacheManager();
		}

		#endregion

		#region Methods: Private

		private async Task<string> GetConfirmationInfoText(string key) {
			var entity = await GetTempReservationDataAsync(key);
			if (entity.Rate == null || entity.Rate.Currency == null) {
				throw new Exception();
			}
			var textRate = entity.OperationId == new Guid(OperationType.Buy) ? entity.Rate.BuyRate : entity.Rate.SellRate;
			var textOperationType = entity.OperationId == new Guid(OperationType.Buy) ? "Покупка" : "Продаж";
			return "⁉️   ⁉️   ⁉️\n\n" +
				$"Валюта: {entity.Rate.Currency.Code}\n" +
				$"Опис: {entity.Rate.Currency.Description}\n\n" +
				$"Курс: {textRate}\n" +
				$"Тип операції: {textOperationType}";
		}

		private InlineKeyboardMarkup GetConfirmationKeyboard() {
			var buttons = new List<List<InlineKeyboardButton>>() {
				new List<InlineKeyboardButton>() {
					InlineKeyboardButton.WithCallbackData(text: "👍", callbackData: Callbacks.Approve),
					InlineKeyboardButton.WithCallbackData(text: "👎", callbackData: Callbacks.Reject)
				}
			};
			return new InlineKeyboardMarkup(buttons);
		}

		#endregion

		#region Methods: Public

		[TextStepperHandler(Callbacks.OperationType, 1)]
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

			var operationId = splittedCallback[1];
			await SetOperationTypeValue(key, operationId);

			var lastMessageId = await _cacheManager.GetLastMessage(key);
			await _bot.DeleteMessageAsync(chatId, messageId: Convert.ToInt32(lastMessageId));

			var message = await _bot.SendTextMessageAsync(chatId, text: "Введіть кількість валюти:");
			await _cacheManager.SaveMessageAsync(key, message.MessageId);

			var config = new StepperInfo() {
				CurrentStep = 1,
				StepsCount = 2,
				Name = Callbacks.OperationType,
			};
			await ConfigureStepperAsync(chatId, config);
		}

		[TextStepperHandler(Callbacks.OperationType, 2)]
		public async Task ConfirmationAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}

			var key = chatId.ToString();
			if (string.IsNullOrEmpty(key)) {
				return;
			}

			var amount = request?.Message?.Text;
			if (string.IsNullOrEmpty(amount)) {
				throw new Exception();
			}
			await SetAmountValue(key, amount);

			var text = await GetConfirmationInfoText(key);
			var message = await _bot.SendTextMessageAsync(chatId, text: text, replyMarkup: GetConfirmationKeyboard());
			await _cacheManager.SaveMessageAsync(key, message.MessageId);
			await NextOrFinishStepAsync(chatId);
		}

		#endregion
	}
}
