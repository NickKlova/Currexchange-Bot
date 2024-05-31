using Newtonsoft.Json;
using System.Text;

namespace ExchangeOffice.Core.Services.Abstractions {
	public abstract class BaseService {
		private readonly string _host = "http://localhost:5034/";

		public async Task<string> PostAsync(string endpoint, object data) {
			var url = _host + endpoint;
			var body = JsonConvert.SerializeObject(data);
			using (HttpClient client = new HttpClient()) {
				StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await client.PostAsync(url, content);

				if (!response.IsSuccessStatusCode) {
					throw new Exception();
				}
				string responseBody = await response.Content.ReadAsStringAsync();
				return responseBody;
			}
		}

		public async Task<string> GetAsync(string endpoint) {
			var url = _host + endpoint;
			using (HttpClient client = new HttpClient()) {
				var response = await client.GetAsync(url);
				if (!response.IsSuccessStatusCode) {
					throw new Exception();
				}
				string responseBody = await response.Content.ReadAsStringAsync();
				return responseBody;
			}
		}
	}
}
