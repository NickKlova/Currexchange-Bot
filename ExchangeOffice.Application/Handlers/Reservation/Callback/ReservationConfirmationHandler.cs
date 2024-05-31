using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Constants;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Abstractions;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Handlers.Reservation.Callback {
	[CallbackMessageHandler(Callbacks.Approve)]
	public class ReservationApproveHandler : BaseReservationCreator, ICallbackHandler {
		#region Fields: Private

		private readonly ITelegramBotClient _bot;
		private readonly ICacheManager _cacheManager;
		private readonly IReservationManager _reservationManager;

		#endregion

		#region Constructors: Public

		public ReservationApproveHandler(IManagerProvider provider) : base(provider) {
			_bot = provider.GetTelegramBotClient();
			_cacheManager = provider.GetCacheManager();
			_reservationManager = provider.GetReservationManager();
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

			var lastMessageId = await _cacheManager.GetLastMessage(key);
			await _bot.DeleteMessageAsync(chatId, messageId: Convert.ToInt32(lastMessageId));

			var tempReservationData = await GetTempReservationDataAsync(key);
			var data = new InsertReservation() {
				ContactId = tempReservationData.ContactId,
				OperationId = tempReservationData.OperationId,
				RateId = tempReservationData.Rate.Id,
				StatusId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), // in progress
				Amount = tempReservationData.Amount
			};

			var msg = await _bot.SendStickerAsync(chatId, sticker: InputFile.FromFileId("CAACAgIAAxkBAAEMOZ1mWahnBXeULfsC9JtS6IlJuz98KwACaBEAAmZ7kUgsXv7fxzaNIDUE"));
			await _reservationManager.CreateReservationAsync(data);
			await _bot.DeleteMessageAsync(chatId, msg.MessageId);
			await _bot.SendTextMessageAsync(chatId, text: "Бронювання успішно створене!");
		}

		#endregion
	}

	[CallbackMessageHandler(Callbacks.Reject)]
	public class ReservationRejectHandler : BaseReservationCreator, ICallbackHandler {
		#region Fields: Private

		private readonly ITelegramBotClient _bot;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors: Public

		public ReservationRejectHandler(IManagerProvider provider) : base(provider) {
			_bot = provider.GetTelegramBotClient();
			_cacheManager = provider.GetCacheManager();
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

			var lastMessageId = await _cacheManager.GetLastMessage(key);
			await _bot.DeleteMessageAsync(chatId, messageId: Convert.ToInt32(lastMessageId));

			await _bot.SendTextMessageAsync(chatId, text: "Створення бронювання скасовано!");
			await ClearTempReservationDataAsync(key);
		}

		#endregion
	}
}
