using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class Twc01_1Tests
    {
        private IWebDriver _driver = null!;
        private string? _accessToken;

        public Twc01_1Tests()
        {
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
            _accessToken ??= await TestHelper.GetAccessToken();
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task Twc01_02()
        {
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/tpcweb_01_1啟用_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.PostAsync(request);
            That(response.IsSuccessful, Is.True);
        }

        [Test]
        [Order(2)]
        public async Task Twc01_03()
        {
            await TestHelper.Login(_driver, "admin", "admin");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");

            TestHelper.ClickRow(_driver, "111123");

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
            await TestHelper.Login(_driver, "admin", "admin");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");

            TestHelper.ClickRow(_driver, "111123");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var waterBuildLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-build-lic]")));
            var waterUseLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-use-lic]")));

            That(waterBuildLic.Text, Is.EqualTo("建築執照號碼109中都工建建字00322號\r\n建照發照日期111年12月13日"));
            That(waterUseLic.Text, Is.EqualTo("建築使用執照111中都工建使字000990號\r\nA01附啟用單使照發照日期111年06月28日"));
        }


    }
}