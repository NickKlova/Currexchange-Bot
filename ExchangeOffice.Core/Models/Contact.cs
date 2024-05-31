namespace ExchangeOffice.Core.Models {
	public class Contact {
		public Guid Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public bool IsBlacklist { get; set; }
	}
}
