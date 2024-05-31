namespace ExchangeOffice.Application.Attributes {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CallbackMessageHandlerAttribute : Attribute {
		public string Text { get; set; }
		public CallbackMessageHandlerAttribute(string text) {
			Text = text;
		}
	}
}
