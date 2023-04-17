using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Runtime.CompilerServices;
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


            var obj = TestHelper.GetSerializationObject(json);
            obj.applyCaseNo = "111124";
           // obj.applyCaseNo = Guid.NewGuid().ToString();
            var updatedJson = JsonConvert.SerializeObject(obj);

            request.AddParameter("application/json", updatedJson, ParameterType.RequestBody);
            var response = await client.PostAsync(request);
            That(response.IsSuccessful, Is.True);
        }

        [Test]
        [Order(2)]
        public async Task Twc01_03()
        {
            await TestHelper.Login(_driver, "0511", "password");

            _driver.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(_driver, "111124");
            Thread.Sleep(1000);

            string[] segments = _driver.Url.Split('/');
            string id = segments[segments.Length - 1];

            _driver.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var 受理編號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var 水號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var 受理日期 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));

            That(受理編號.Text, Is.EqualTo("111124"));
            That(水號.Text, Is.EqualTo("41101202191"));
            That(受理日期.Text, Is.EqualTo("2023年03月06日"));
        }

        [Test]
        [Order(3)]
        public async Task Twc01_04()
        {
            await TestHelper.Login(_driver, "0511", "password");

            _driver.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(_driver, "111124");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var 身分證字號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));
            
            //var action = new Actions(_driver);

            //action.MoveToElement(身分證字號).Perform();
            身分證字號.SendKeys("A123456789");

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 身分證字號);

            ////((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = 'A123456789';", 身分證字號);

            That(身分證字號.GetAttribute("value"), Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task Twc01_05()
        {
            await TestHelper.Login(_driver, "0511", "password");

            _driver.Navigate().GoToUrl($"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(_driver, "111124");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);

            Actions actions = new Actions(_driver);
            actions.MoveToElement(受理).Click().Perform();

            Console.WriteLine();
        }

    }
}