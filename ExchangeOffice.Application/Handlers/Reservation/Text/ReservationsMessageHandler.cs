using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Views;
using Telegram.Bot.Types;
using Telegram.Bot;
using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Views.Constants;
using ExchangeOffice.Application.Handlers.Interfaces;

namespace ExchangeOffice.Application.Handlers.Reservation.Text {
	[TextMessageHandler(MainMenuTitles.Reservations)]
	public class ReservationsMessageHandler : ITextHandler {
		#region Fields: Private

		private ITelegramBotClient _bot;

		#endregion

		#region Constructors: Public

		public ReservationsMessageHandler(IManagerProvider provider) {
			_bot = provider.GetTelegramBotClient();
		}

		#endregion

		#region Methods: Public

		public async Task ExecuteAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}

			await _bot.SendTextMessageAsync(chatId, text: "Бронювання:", replyMarkup: ReservationMenu.GetKeyboard());
		}

		#endregion
	}
}
