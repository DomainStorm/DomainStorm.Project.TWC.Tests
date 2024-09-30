using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcC101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcC101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcC101Tests).GetMethod(testMethod!);
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
        [NoBrowser]
        public Task TwcC101_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcC101_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C101_bmDisableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcC101_03To07()
        {
            TwcC101_03();
            TwcC101_04();
            TwcC101_05();
            TwcC101_06();
            TwcC101_07();

            return Task.CompletedTask;
        }
        public Task TwcC101_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _testHelper.WaitElementExists(By.CssSelector("span[sti-post-user-full-name='']"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("張博文"));

            return Task.CompletedTask;
        }
        public Task TwcC101_04()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcC101_05()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【夾帶附件】或【掃描拍照】未上傳')]")));
            That(hint.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            _testHelper.ElementClick(By.XPath("//button[text()='確定']"));

            return Task.CompletedTask;
        }
        public Task TwcC101_06()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='啟動掃描證件']"));
            _testHelper.WaitElementExists(By.CssSelector("img[alt='證件_005.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcC101_07()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcC101_08()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/unfinished", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.MoveAndCheck("#消費性用水服務契約");
            _testHelper.MoveAndCheck("#公司個人資料保護告知事項");
            _testHelper.MoveAndCheck("#公司營業章程");
            _testHelper.WaitElementExists(By.XPath("//storm-card[position()=6]//img"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcC101_09To11()
        {
            TwcC101_09();
            TwcC101_10();
            TwcC101_11();

            return Task.CompletedTask;
        }

        public Task TwcC101_09()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-06-03";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));

            var applyCaseNo = _wait.Until(_driver =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
                return applyCaseNoElements.FirstOrDefault(element => element.Text == TestHelper.ApplyCaseNo);
            });
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }
        public Task TwcC101_10()
        {
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//storm-card[position()=6]//img"));

            return Task.CompletedTask;
        }
        public Task TwcC101_11()
        {
            _testHelper.DownloadFileAndVerify("41101699338.pdf", "//button[text()='轉PDF']");

            return Task.CompletedTask;
        }
    }
}