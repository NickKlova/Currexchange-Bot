namespace ExchangeOffice.Application.Attributes {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TextMessageHandlerAttribute : Attribute {
		public string Text { get; set; }

		public TextMessageHandlerAttribute(string text) {
			Text = text;
		}
	}
}
