using ExchangeOffice.Application.Attributes;
using ExchangeOffice.Application.Handlers.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;
using Telegram.Bot.Types;

namespace ExchangeOffice.Application.Middlewares {
	public class HandlerMiddleware {
		#region Fields: Private

		private readonly RequestDelegate _next;
		private readonly Dictionary<string, ITextHandler> _handlers;
		private readonly Dictionary<string, ICallbackHandler> _callbackHandlers;

		#endregion

		#region Constructors: Public

		public HandlerMiddleware(RequestDelegate next, IEnumerable<ITextHandler> handlers, IEnumerable<ICallbackHandler> cbHandlers) {
			_next = next;
			_handlers = handlers
				.Select(h => new { Handler = h, Attribute = h.GetType().GetCustomAttribute<TextMessageHandlerAttribute>() })
				.Where(x => x.Attribute != null)
				.ToDictionary(x => x.Attribute!.Text, x => x.Handler);
			_callbackHandlers = cbHandlers
				.Select(h => new { Handler = h, Attribute = h.GetType().GetCustomAttribute<CallbackMessageHandlerAttribute>() })
			.Where(x => x.Attribute != null)
				.ToDictionary(x => x.Attribute!.Text, x => x.Handler);
		}

		#endregion

		#region Methods: Private

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
			context.Request.EnableBuffering();
			var update = await GetUpdateFromRequest(context.Request.Body);

			if (update == null) {
				return;
			}

			var text = update?.Message?.Text;
			if (!string.IsNullOrEmpty(text) && _handlers.TryGetValue(text, out var handler)) {
				await handler.ExecuteAsync(update!);
				context.Request.Headers["handler-executed"] = "1";
			}

			var callback = update?.CallbackQuery?.Data;
			if (!string.IsNullOrEmpty(callback)) {
				var searcher = callback.Contains('|') ? callback.Substring(0, callback.IndexOf('|')) : callback;
				if (!string.IsNullOrEmpty(callback) && _callbackHandlers.TryGetValue(searcher, out var callbackHandler)) {
					await callbackHandler.ExecuteAsync(update!);
				}
			}

			await _next(context);
		}

		#endregion
	}
}
