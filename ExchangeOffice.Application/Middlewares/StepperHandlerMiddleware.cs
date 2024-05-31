using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Extensions.Providers.Interfaces;
using ExchangeOffice.Application.Models;
using ExchangeOffice.Cache.Clients.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;
using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Middlewares {
	public class StepperHandlerMiddleware {
		#region Fields: Private

		private readonly RequestDelegate _next;
		private readonly ICacheClient _cache;
		private readonly IManagerProvider _managerProvider;

		#endregion

		#region Constructors: Public

		public StepperHandlerMiddleware(RequestDelegate next, ICacheClient cacheClient, IManagerProvider managerProvider) {
			_next = next;
			_cache = cacheClient;
			_managerProvider = managerProvider;
		}

		#endregion

		#region Methods: Private

		private async Task ExecuteTextHandlerNextStep(Update request, StepperInfo info) {
			var stepperKey = GetUserStepper(request);
			if (string.IsNullOrEmpty(stepperKey))
				return;

			var stepperName = info.Name;
			if(string.IsNullOrEmpty(stepperName)) {
				return;
			}
			var types = GetTypesWithTextHandlerAttribute(stepperName);

			foreach (var type in types) {
				MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (var method in methods) {
					var attributes = method.GetCustomAttributes(typeof(TextStepperHandlerAttribute), false);
					foreach (var attribute in attributes) {
						var typedAttribute = (TextStepperHandlerAttribute)attribute;
						if (typedAttribute.Name == info.Name && typedAttribute.Step == info.CurrentStep) {
							var instance = Activator.CreateInstance(type, _managerProvider);
							await Task.FromResult(method.Invoke(instance, new[] { request }));
						}
					}
				}
			}
		}

		private IEnumerable<Type> GetTypesWithTextHandlerAttribute(string stepperName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			var types = assembly.GetTypes();

			return types.Where(t =>
				t.GetCustomAttributes(false).Any(attr =>
					(attr is TextMessageHandlerAttribute textAttr && textAttr.Text == stepperName) ||
					(attr is CallbackMessageHandlerAttribute callbackAttr && callbackAttr.Text == stepperName)));
		}

		private string? GetUserStepper(Update request) {
			return request?.Message?.Chat?.Id.ToString();
		}

		private async Task<Update?> GetUpdateFromRequest(Stream requestBody) {
			var body = await GetRequestBody(requestBody);
			return JsonConvert.DeserializeObject<Update>(body);
		}

		private async Task<string> GetRequestBody(Stream body) {
			var data = await new StreamReader(body, leaveOpen: true).ReadToEndAsync();
			if (body.CanSeek)
				body.Position = 0;
			return data;
		}

		#endregion

		#region Methods: Public

		public async Task InvokeAsync(HttpContext context) {
			var handlerExecutedHeader = context.Request.Headers.TryGetValue("handler-executed", out var isHandlerExecuted);
			if (!string.IsNullOrEmpty(isHandlerExecuted)) {
				await _next(context);
			}

			context.Request.EnableBuffering();
			var update = await GetUpdateFromRequest(context.Request.Body);
			if (update == null) {
				return;
			}

			string? userStepper = GetUserStepper(update);
			if (string.IsNullOrEmpty(userStepper)) {
				return;
			}

			var stepInfoJson = await _cache.GetAsync(userStepper);
			if (!string.IsNullOrEmpty(stepInfoJson)) {
				var stepInfo = JsonConvert.DeserializeObject<StepperInfo>(stepInfoJson);
				if (stepInfo == null) {
					return;
				}

				await ExecuteTextHandlerNextStep(update, stepInfo);
			}

			await _next(context);
		}

		#endregion
	}
}
