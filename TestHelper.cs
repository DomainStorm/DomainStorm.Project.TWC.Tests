using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DomainStorm.Project.TWC.Tests;

public class TestHelper
{
    private static TestConfig _testConfig;
    public static TestConfig TestConfig
    {
        get
        {
            _testConfig ??= GetTestConfig();
            return _testConfig;
        }
    }
    private static TestConfig GetTestConfig()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .AddUserSecrets<TestHelper>()
            .AddEnvironmentVariables()
            .Build()
            .GetSection("TestConfig")
            .Get<TestConfig>();
    }
    private static string? _baseUrl;
    public static string? BaseUrl
    {
        get
        {
            _baseUrl ??= GetTestConfig().BaseUrl;
            return _baseUrl;
        }
    }
    private static string? _tokenUrl;
    public static string? TokenUrl
    {
        get
        {
            _tokenUrl ??= GetTestConfig().TokenUrl;
            return _tokenUrl;
        }
    }
    private static string? _loginUrl;
    public static string? LoginUrl
    {
        get
        {
            _loginUrl ??= GetTestConfig().LoginUrl;
            return _loginUrl;
        }
    }
    private static string? _accessToken;
    public static string AccessToken
    {
        get
        {
            _accessToken ??= GetTestConfig().AccessToken;
            return _accessToken;
        }
    }
    public class TokenResponse
    {
        public string Access_token { get; set; }
    }
    public static async Task<string> GetAccessToken()
    {
        var client = new RestClient(TokenUrl);
        var request = new RestRequest();

        const string clientId = "bmuser";
        const string clientSecret = "4xW8KpkKkeFc";
        var encodedData = Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(clientId + ":" + clientSecret));
        request.AddHeader("Authorization", $"Basic {encodedData}");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("scope", "template_read post_read");

        var response = await client.PostAsync<TokenResponse>(request);
        return response?.Access_token ?? throw new InvalidOperationException("Failed to get access token.");
    }

    public static Task Login(IWebDriver webDriver, string userId, string password)
    {

        webDriver.Navigate().GoToUrl(LoginUrl);
        webDriver.Manage().Window.Size = new Size(1200, 800);

        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var detailsButtonElements = webDriver.FindElements(By.CssSelector("#details-button"));
        if (detailsButtonElements.Count > 0)
        {
            var detailsButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#details-button")));
            detailsButton.Click();

            var proceedLink = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#proceed-link")));
            proceedLink.Click();
        }

        var usernameElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
        var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));

        usernameElement.SendKeys(userId);
        passwordElement.SendKeys(password);

        var button = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
        button.Click();

        Thread.Sleep(2000);

        return Task.CompletedTask;
    }

    public static void ClickRow(IWebDriver webDriver, string applyCaseNo)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));

        var card = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
        var stormDocumentListDetail = card.FindElement(By.CssSelector("storm-document-list-detail"));
        var stormTable = stormDocumentListDetail.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

        var findElements = stormTable.GetShadowRoot()
            .FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

        var element = findElements.FirstOrDefault(e => e.Text == applyCaseNo);

        var action = new Actions(webDriver);
        action.MoveToElement(element).Perform();

        element!.Click();
    }
}

public class Serialization
{
    public string applyCaseNo { get; set; }
    public string applyDate { get; set; }
    public string operatingArea { get; set; }
    public string waterNo { get; set; }
    public string typeChange { get; set; }
    public string userCode { get; set; }
    public string deviceLocation { get; set; }
    public string applicant { get; set; }
    public string idNo { get; set; }
    public string unino { get; set; }
    public string telNo { get; set; }
    public string mobileNo { get; set; }
    public string pipeDiameter { get; set; }
    public string waterType { get; set; }
    public string scoreSheet { get; set; }
    public string waterBuildLic { get; set; }
    public string waterUseLic { get; set; }
    public string billAddress { get; set; }
}


public class TestConfig
{
    public string? BaseUrl { get; set; }

    public string? TokenUrl { get; set; }

    public string? LoginUrl { get; set; }

    public string? AccessToken { get; set; }
}

