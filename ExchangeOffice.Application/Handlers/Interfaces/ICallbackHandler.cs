using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Handlers.Interfaces {
	public interface ICallbackHandler {
		public Task ExecuteAsync(Update request);
	}
}
