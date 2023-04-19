using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
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

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/tpcweb_01_1�ҥ�_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();


            var guid = TestHelper.GetSerializationObject(json);
            guid.applyCaseNo = "111124";
            var updatedJson = JsonConvert.SerializeObject(guid);

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

            var ���z�s�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var ���� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var ���z��� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));

            That(���z�s��.Text, Is.EqualTo("111124"));
            That(����.Text, Is.EqualTo("41101202191"));
            That(���z���.Text, Is.EqualTo("2023�~03��06��"));
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

            var �����Ҧr�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));

            �����Ҧr��.SendKeys("A123456789");

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", �����Ҧr��);

            That(�����Ҧr��.GetAttribute("value"), Is.EqualTo("A123456789"));
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

            var ���z = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#���z")));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", ���z);

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", ���z);
        }

        [Test]
        [Order(5)]
        public async Task Twc01_06()
        {
            await TestHelper.Login(_driver, "0511", "password");

            _driver.Navigate().GoToUrl($"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(_driver, "111124");

            //var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            //var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            //var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            //var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            //var stormTreeNodes = stormTreeNode[1];

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='sync'])[1]/following::storm-vertical-navigation[1]")));

            Actions actions = new Actions(_driver);
            actions.MoveToElement(element).Click().Perform();

            _driver.FindElement(By.Id("���O�ʥΤ��A�ȫ���")).Click();

        }
    }
}