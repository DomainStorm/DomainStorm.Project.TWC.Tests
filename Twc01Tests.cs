using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System;
using System.Drawing;
using static NUnit.Framework.Assert;
using static System.Net.Mime.MediaTypeNames;

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

        private Task Login(string userId, string password)
        {
            _driver.Navigate().GoToUrl($@"{BaseUrl}");
            _driver.Manage().Window.Size = new Size(1200, 800);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            //if (IsRemote)
            //{
            //    var detailsButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#details-button")));
            //    detailsButton.Click();

            //    var proceedLink = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#proceed-link")));
            //    proceedLink.Click();

            //}

            var usernameElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));

            usernameElement.SendKeys(userId);
            passwordElement.SendKeys(password);

            var button = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            button.Click();

            Thread.Sleep(2000);

            return Task.CompletedTask;
        }
        private void ClickRow(string applyCaseNo)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var card = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = card.FindElement(By.CssSelector("storm-document-list-detail"));
;           var stormTable = stormDocumentListDetail.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var findElements = stormTable.GetShadowRoot()
                .FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            var element = findElements.FirstOrDefault(e => e.Text == applyCaseNo);

            var action = new Actions(_driver);
            action.MoveToElement(element).Perform();

            element!.Click();
        }

        [SetUp]
        public Task Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return Task.CompletedTask;
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
            if (_accessToken == null)
                await GetAccessToken();
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task Twc01_02()
        {
            var client = new RestClient($"{BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tpcweb_01_1啟用_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.PostAsync(request);
            That(response.IsSuccessful, Is.True);
        }

        [Test]
        [Order(2)]
        public async Task Twc01_03()
        {
            await Login("admin", "admin");

            _driver.Navigate().GoToUrl($@"{BaseUrl}/draft");

            ClickRow("111123");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var 受理編號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var 水號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var 受理日期 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));
            var 身份證號碼 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-id-no]")));
            var 營利事業統一編號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-uni-no]")));

            That(受理編號.Text, Is.EqualTo("111123"));
            That(水號.Text, Is.EqualTo("41101234568"));
            That(受理日期.Text, Is.EqualTo("41101234568"));
            That(身份證號碼.Text, Is.EqualTo("A123456789"));
            That(營利事業統一編號.Text, Is.EqualTo("16753217"));
        }

        [Test]
        [Order(3)]
        public async Task Twc01_04()
        {
            await Login("admin", "admin");

            _driver.Navigate().GoToUrl($@"{BaseUrl}/draft");

            ClickRow("111123");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var waterBuildLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-build-lic]")));
            var waterUseLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-use-lic]")));

            That(waterBuildLic.Text, Is.EqualTo("建築執照號碼109中都工建建字00322號\r\n建照發照日期111年12月13日"));
            That(waterUseLic.Text, Is.EqualTo("建築使用執照111中都工建使字000990號\r\nA01附啟用單使照發照日期111年06月28日"));
        }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
    }
}