using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Core.Managers.Interfaces;
using Telegram.Bot;

namespace ExchangeOffice.Application.Extensions.Providers.Interfaces {
	public interface IManagerProvider {
		public ITelegramBotClient GetTelegramBotClient();
		public IRateManager GetRateManager();
		public IContactManager GetContactManager();
		public IReservationManager GetReservationManager();
		public ICacheManager GetCacheManager();
	}
}
