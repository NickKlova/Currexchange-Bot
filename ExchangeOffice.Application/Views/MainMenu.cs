using ExchangeOffice.Application.Views.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExchangeOffice.Application.Views {
	public static class MainMenu {
		public static ReplyKeyboardMarkup GetKeyboard() {
			return new(
				new[]
				{
					new KeyboardButton[] { MainMenuTitles.MyContact },
					new KeyboardButton[] { MainMenuTitles.Reservations, MainMenuTitles.Rates },
					new KeyboardButton[] { MainMenuTitles.Faq }
				}) { ResizeKeyboard = true };
		}
	}
}
