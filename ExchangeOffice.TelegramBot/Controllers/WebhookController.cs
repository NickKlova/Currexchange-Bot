using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ExchangeOffice.TelegramBot.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class WebhookController : ControllerBase {
		[HttpPost("/telegram/update")]
		public async Task<IActionResult> ProcessUpdateAsync(Update request) {
			return Ok();
		}
	}
}
