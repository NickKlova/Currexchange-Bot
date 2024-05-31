namespace ExchangeOffice.Application.Attributes {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TextStepperHandlerAttribute : Attribute {
		public string? Name { get; set; }
		public int Step { get; set; }
		public TextStepperHandlerAttribute(string sessionName, int stepNumber) {
			Name = sessionName;
			Step = stepNumber;
		}
	}
}
