using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RestSharp;

namespace DomainStorm.Project.TWC.Tests
{
    public class Twc01Tests
    {
        private const string BaseUrl = "https://localhost:9003";
        private IWebDriver _driver = null!;
        private string? _accessToken;

        private async Task GetAccessToken()
        {
            var client = new RestClient("http://localhost:5050/connect/token");
            var request = new RestRequest();
            const string clientId = "bmuser";
            const string clientSecret = "4xW8KpkKkeFc";
            var encodedData = Convert.ToBase64String(
                System.Text.Encoding.GetEncoding("UTF-8")
                    .GetBytes(clientId + ":" + clientSecret)
            );
            request.AddHeader("Authorization", $"Basic {encodedData}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "template_read post_read");

            var restResponse = await client.PostAsync<TokenResponse>(request);
            _accessToken = restResponse?.access_token;
        }
        [SetUp]
        public async Task Setup()
        {
            if (_accessToken == null)
                await GetAccessToken();
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }
        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task Twc01_01()
        {
            var client = new RestClient($"{BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tpcweb_01_1±Ò¥Î_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.PostAsync(request);

            if (response.IsSuccessful)
            {

            }
            else
            {
         
            }
        }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
    }
}