using Microsoft.Extensions.DependencyInjection;
using ExchangeOffice.Core.Extensions;
using ExchangeOffice.Application.Attributes;
using System.Reflection;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Extensions.Providers;
using ExchangeOffice.Application.Handlers.Interfaces;
using Microsoft.AspNetCore.Builder;
using ExchangeOffice.Application.Middlewares;

namespace ExchangeOffice.Application.Extensions {
	public static class ApplicationExtension {
		public static void AddApplicationLayer(this IServiceCollection services) {
			services.AddCoreLayer();
			services.AddSingleton<IManagerProvider, ManagerProvider>();

			var assembly = Assembly.GetExecutingAssembly();
			var handlerTypes = assembly.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(TextMessageHandlerAttribute), true).Length > 0);

			foreach (var handlerType in handlerTypes) {
				var attribute = handlerType.GetCustomAttribute<TextMessageHandlerAttribute>();
				if (attribute != null) {
					services.AddSingleton(typeof(ITextHandler), handlerType);
				}
			}
			var callbackTypes = assembly.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(CallbackMessageHandlerAttribute), true).Length > 0);

			foreach (var handlerType in callbackTypes) {
				var attribute = handlerType.GetCustomAttribute<CallbackMessageHandlerAttribute>();
				if (attribute != null) {
					services.AddSingleton(typeof(ICallbackHandler), handlerType);
				}
			}
		}

		public static void UseApplicationMiddlewares(this IApplicationBuilder app) {
			app.UseMiddleware<HandlerMiddleware>();
			app.UseMiddleware<StepperHandlerMiddleware>();
		}
	}
}
