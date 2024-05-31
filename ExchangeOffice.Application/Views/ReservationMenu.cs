using ExchangeOffice.Application.Views.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExchangeOffice.Application.Views {
	public class ReservationMenu {
		public static ReplyKeyboardMarkup GetKeyboard() {
			return new(
				new[]
				{
					new KeyboardButton[] { ReservationMenuTitles.MyReservations },
					new KeyboardButton[] { ReservationMenuTitles.CreateReservation },
					new KeyboardButton[] { ReservationMenuTitles.BackToMainMenu }
				}) { ResizeKeyboard = true };
		}
	}
}
