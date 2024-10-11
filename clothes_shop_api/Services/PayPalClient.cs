using clothes_shop_api.Helpers;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace clothes_shop_api.Services
{
    public class PayPalClient
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;
        private readonly HttpClient _httpClient;

        public string BaseUrl = "https://api-m.sandbox.paypal.com";

        public PayPalClient(IConfiguration config)
        {
            _clientId = config["PayPalSettings:ClientId"];
            _clientSecret = config["PayPalSettings:ClientSecret"];
            _mode = config["PayPalSettings:Mode"];
            _httpClient = new HttpClient();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var content = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials")
            };
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/v1/oauth2/token"),
                Method = System.Net.Http.HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Basic {authToken}" }
                },
                Content = new FormUrlEncodedContent(content)
            };

            
            var httpResponse = await _httpClient.SendAsync(request);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

            return response?.access_token;
        }

        public async Task<PayPalOrderResponse> CreatePayPalOrderAsync(decimal amount)
        {
            var httpClient = new HttpClient();
            var accessToken = await GetAccessTokenAsync();
            var request = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new {
                        amount = new {
                            currency_code = "USD",
                            value = amount.ToString("F2")
                        }
                    }
                }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var httpResponse = await httpClient.PostAsJsonAsync($"{BaseUrl}/v2/checkout/orders", request);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PayPalOrderResponse>(jsonResponse);


            return response;
        }

        public async Task<PayPalOrderResponse> CaptureOrderAsync(string id)
        {
            var accessToken = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");

            var httpContent = new StringContent("", Encoding.Default, "application/json");

            var httpResponse = await _httpClient.PostAsync($"{BaseUrl}/v2/checkout/orders/{id}/capture", httpContent);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PayPalOrderResponse>(jsonResponse);

            return response;
        }


        public sealed class AuthResponse
    {
        public string scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
        public string nonce { get; set; }
    }



        public class PayPalOrderResponse
        {
            public string id { get; set; }
            public string status { get; set; }
            public PayPalLink[] links { get; set; }
        }

        public class PayPalLink
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        //public sealed class PayPalCaptureOrderResponse
        //{
        //    public string id { get; set; }
        //    public string status { get; set; }

        //}


    }
}
