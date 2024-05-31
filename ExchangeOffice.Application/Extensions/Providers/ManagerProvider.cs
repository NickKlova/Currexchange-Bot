using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Core.Managers.Interfaces;
using Telegram.Bot;

namespace ExchangeOffice.Application.Extensions.Providers {
	public class ManagerProvider : IManagerProvider {
		private readonly IServiceProvider _serviceProvider;
		public ManagerProvider(IServiceProvider serviceProvider) {
			_serviceProvider = serviceProvider;
		}

		public ITelegramBotClient GetTelegramBotClient() {
			var manager = _serviceProvider.GetService(typeof(ITelegramBotClient));
			if (manager == null) {
				throw new Exception();
			}

			var typedManager = (ITelegramBotClient)manager;
			return typedManager;
		}

		public IRateManager GetRateManager() {
			var manager = _serviceProvider.GetService(typeof(IRateManager));
			if (manager == null) {
				throw new Exception();
			}

			var typedManager = (IRateManager)manager;
			return typedManager;
		}

		public IContactManager GetContactManager() {
			var manager = _serviceProvider.GetService(typeof(IContactManager));
			if (manager == null) {
				throw new Exception();
			}

			var typedManager = (IContactManager)manager;
			return typedManager;
		}

		public IReservationManager GetReservationManager() {
			var manager = _serviceProvider.GetService(typeof(IReservationManager));
			if (manager == null) {
				throw new Exception();
			}

			var typedManager = (IReservationManager)manager;
			return typedManager;
		}

		public ICacheManager GetCacheManager() {
			var manager = _serviceProvider.GetService(typeof(ICacheManager));
			if (manager == null) {
				throw new Exception();
			}

			var typedManager = (ICacheManager)manager;
			return typedManager;
		}
	}
}
