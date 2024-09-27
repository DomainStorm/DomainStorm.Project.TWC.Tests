using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcB100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcB100Tests).GetMethod(testMethod!);
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
        public Task TwcB100_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }
        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcB100_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B100_bmRecoverApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }
        [Test]
        [Order(2)]
        public Task TwcB100_03To13()
        {
            TwcB100_03();
            TwcB100_04();
            TwcB100_05();
            TwcB100_06();
            TwcB100_07();
            TwcB100_08();
            TwcB100_09();
            TwcB100_10();
            TwcB100_11();
            TwcB100_12();
            TwcB100_13();

            return Task.CompletedTask;
        }
        public Task TwcB100_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));
            _testHelper.OpenNewWindowWithLastSegmentUrlAndVerify();

            return Task.CompletedTask;
        }
        public Task TwcB100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            _testHelper.InputSendkeys(By.XPath("//span[@sti-trustee-id-no]/input"), "A123456789" + Keys.Tab);
            var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));
            That(idElement.GetAttribute("value"), Is.EqualTo("A123456789"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _testHelper.WaitElementExists(By.XPath("//span[@id='身分證號碼']"));

            var check = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼']")));
            That(check.Text, Is.EqualTo("A123456789"));

            return Task.CompletedTask;
        }
        public Task TwcB100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _testHelper.InputSendkeys(By.XPath("//input[@id='繳費']"), Keys.Tab);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#繳費")));
            That(stiPay.Selected);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            That(stiPay.Selected);

            return Task.CompletedTask;
        }
        public Task TwcB100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _testHelper.WaitElementExists(By.CssSelector("span[sti-post-user-full-name='']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));

            return Task.CompletedTask;
        }
        public Task TwcB100_07()
        {
            _driver.SwitchTo().DefaultContent();
            _testHelper.SwitchWindowAndClick("//label[@for='消費性用水服務契約']");

            return Task.CompletedTask;
        }
        public Task TwcB100_08()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司個人資料保護告知事項']");

            return Task.CompletedTask;
        }
        public Task TwcB100_09()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司營業章程']");

            return Task.CompletedTask;
        }
        public Task TwcB100_10()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.ElementClick(By.XPath("//span[text()='簽名']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcB100_11()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='啟動掃描證件']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcB100_12()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf", "twcweb_01_1_夾帶附件2.pdf" }, "input.dz-hidden-input:nth-of-type(3)");
            _testHelper.WaitElementExists(By.CssSelector("storm-edit-table"));

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            return Task.CompletedTask;
        }
        public Task TwcB100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));
            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }
    }
}