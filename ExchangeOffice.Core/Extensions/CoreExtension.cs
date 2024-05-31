using ExchangeOffice.Core.Services.Interfaces;
using ExchangeOffice.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using ExchangeOffice.Cache.Extensions;
using ExchangeOffice.Core.Managers;
using ExchangeOffice.Core.Managers.Interfaces;
using ExchangeOffice.Application.Managers.Interfaces;
using ExchangeOffice.Application.Managers;

namespace ExchangeOffice.Core.Extensions
{
    public static class CoreExtension {
		public static void AddCoreLayer(this IServiceCollection services) {
			services.AddCacheLayer();
			services.AddTransient<ICacheManager, CacheManager>();
			services.AddSingleton<IRateService, RateService>();
			services.AddSingleton<IRateManager, RateManager>();
			services.AddSingleton<IContactService, ContactService>();
			services.AddSingleton<IContactManager, ContactManager>();
			services.AddSingleton<IReservationService, ReservationService>();
			services.AddSingleton<IReservationManager, ReservationManager>();
		}
	}
}
