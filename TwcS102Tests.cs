using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS102Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcS102Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcD100Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
                _testHelper = new TestHelper(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
        }

        [Test]
        [Order(0)]
        public Task TwcS102_01()
        {
            CreateFormBy_0511();
            CreateFormBy_tw491();
            CreateFormBy_ning53();

            return Task.CompletedTask;
        }
        public Task CreateFormBy_0511()
        {
            _testHelper.PrepareData("0511", $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", "Assets/twcweb-A101_bmEnableApply.json", "張博文");
            
            return Task.CompletedTask;
        }
        public Task CreateFormBy_tw491()
        {
            _testHelper.PrepareData("tw491", $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", "Assets/twcweb-S100_bmTransferApply.json", "謝德威");

            return Task.CompletedTask;
        }
        public Task CreateFormBy_ning53()
        {
            _testHelper.PrepareData("ning53", $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", "Assets/twcweb-RA001_bmTransferApply.json", "陳宥甯");

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcS102_03()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        [NoBrowser]
        public Task TwcS102_04()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S102_bmTransferApply_bground2.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcS102_05To06()
        {
            TwcS102_05();
            TwcS102_06();

            return Task.CompletedTask;
        }
        public Task TwcS102_05()
        {
            _testHelper.Login("live", TestHelper.Password!);

            return Task.CompletedTask;
        }

        public Task TwcS102_06()
        {
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-03-01";
            var applyDateBeginSelect = _testHelper.WaitElementVisible(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));

            _wait.Until(driver =>
            {
                var parentElement = _testHelper.WaitShadowElement("tbody");
                var rows = parentElement?.FindElements(By.CssSelector("tr"));

                return rows!.Count == 3;
            });

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcS102_07To08()
        {
            TwcS102_07();
            TwcS102_08();

            return Task.CompletedTask;
        }
        public Task TwcS102_07()
        {
            _testHelper.Login("alarmsue", TestHelper.Password!);

            return Task.CompletedTask;
        }

        public Task TwcS102_08()
        {
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-03-01";
            var applyDateBeginSelect = _testHelper.WaitElementVisible(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));

            _wait.Until(driver =>
            {
                var parentElement = _testHelper.WaitShadowElement("tbody");
                var rows = parentElement?.FindElements(By.CssSelector("tr"));

                return rows!.Count == 1;
            });

            return Task.CompletedTask;
        }
    }
}
