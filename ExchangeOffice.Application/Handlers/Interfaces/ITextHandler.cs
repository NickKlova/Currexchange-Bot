using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Handlers.Interfaces {
	public interface ITextHandler {
		public Task ExecuteAsync(Update request);
	}
}
