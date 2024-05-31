using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Core.Managers.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Views.Constants;

namespace ExchangeOffice.Application.Handlers.Reservation.Text {
	[TextMessageHandler(ReservationMenuTitles.MyReservations)]
	public class MyReservationsMessageHandler : ITextHandler {
		#region Fields: Private

		private ITelegramBotClient _bot;
		private IReservationManager _reservationManager;
		private IContactManager _contactManager;

		#endregion

		#region Constructors: Public

		public MyReservationsMessageHandler(IManagerProvider provider) {
			_bot = provider.GetTelegramBotClient();
			_reservationManager = provider.GetReservationManager();
			_contactManager = provider.GetContactManager();
		}

		#endregion

		#region Methods: Private

		private string GetReservationsText(IEnumerable<Core.Models.Reservation> reservations) {
			var resultText = "";
			foreach(var reservation in reservations) {
				var textRate = reservation.OperationType.Id == new Guid(Constants.OperationType.Buy) ? reservation.Rate.BuyRate : reservation.Rate.SellRate;
				var textOperationType = reservation.OperationType.Id == new Guid(Constants.OperationType.Buy) ? "покупку" : "продаж";

				resultText += "🤖    🤖    🤖\n" +
					$"Валюта: {reservation.Rate.Currency.Code}\n" +
					$"Опис: {reservation.Rate.Currency.Description}\n\n" +
					$"Бронювання на {textOperationType}.\n" +
					$"Курс: {textRate}\n" +
					$"Кількість: {reservation.Amount}\n" +
					$"Статус бронювання: {reservation.Status.Name}\n\n\n\n";
			}
			return resultText;
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

			var reservations = await _reservationManager.GetReservationsAsync(contact.Id.ToString());
			if (reservations.Count() == 0) {
				await _bot.SendTextMessageAsync(chatId, text: "У вас немає активних бронювань, створіть нове 😥", replyMarkup: ReservationMenu.GetKeyboard());
				return;
			}

			var msgText = GetReservationsText(reservations);
			await _bot.SendTextMessageAsync(chatId, text: msgText, replyMarkup: ReservationMenu.GetKeyboard());
		}

		#endregion
	}
}
