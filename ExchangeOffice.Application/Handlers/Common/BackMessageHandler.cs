using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Handlers.Interfaces;
using ExchangeOffice.Application.Views;
using ExchangeOffice.Application.Views.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Handlers.Common {
	[TextMessageHandler(ReservationMenuTitles.BackToMainMenu)]
	public class BackMessageHandler : ITextHandler {
		#region Fields: Private

		private ITelegramBotClient _bot;

		#endregion

		#region Constructors: Public

		public BackMessageHandler(IManagerProvider provider) {
			_bot = provider.GetTelegramBotClient();
		}

		#endregion

		#region Methods: Public

		public async Task ExecuteAsync(Update request) {
			var chatId = request?.Message?.Chat.Id;
			if (chatId == null) {
				return;
			}

			await _bot.SendTextMessageAsync(chatId, text: "Головне меню:", replyMarkup: MainMenu.GetKeyboard());
		}

		#endregion
	}
}
