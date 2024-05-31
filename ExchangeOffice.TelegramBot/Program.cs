using Telegram.Bot;
using ExchangeOffice.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var client = new TelegramBotClient("6700447814:AAG-ynj_oHoZ9mEeW8kORRCSXDl0Aewf2i0");
client.SetWebhookAsync("https://69e3-176-39-34-13.ngrok-free.app/telegram/update/", dropPendingUpdates: true).Wait();
builder.Services.AddSingleton<ITelegramBotClient>(options => {

	return client;
});
builder.Services.AddApplicationLayer();


var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseApplicationMiddlewares();
app.UseAuthorization();

app.MapControllers();

app.Run();
