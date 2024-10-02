using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcE100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcE100Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
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
        public Task TwcE100_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcE100_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E100_bmTransferApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcE100_03To16()
        {
            TwcE100_03();
            TwcE100_05();
            TwcE100_06();
            TwcE100_07();
            TwcE100_08();
            TwcE100_09();
            TwcE100_10();
            TwcE100_11();
            TwcE100_12();
            TwcE100_13();
            TwcE100_14();
            TwcE100_15();
            TwcE100_16();

            return Task.CompletedTask;
        }
        public Task TwcE100_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));
            _testHelper.OpenNewWindowWithLastSegmentUrlAndVerify();

            return Task.CompletedTask;
        }
        public Task TwcE100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            _testHelper.InputSendKeys(By.XPath("//span[@sti-trustee-id-no]/input"), "A123456789" + Keys.Tab);
            //var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));
            //That(idElement.GetAttribute("value"), Is.EqualTo("A123456789"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _testHelper.WaitElementExists(By.XPath("//span[@id='身分證號碼']"));

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼'][text()='A123456789']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcE100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            _testHelper.InputSendKeys(By.XPath("//span[@sti-note]/input"), "備註內容" + Keys.Tab);
            //var stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-note]/input")));
            //That(stiNote.GetAttribute("value"), Is.EqualTo("備註內容"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _testHelper.WaitElementExists(By.XPath("//span[@sti-note]"));

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note][text()='備註內容']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcE100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            _testHelper.InputSendKeys(By.XPath("//input[@id='中結']"), Keys.Tab);

            That(_wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#中結"))));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            That(_wait.Until(ExpectedConditions.ElementToBeSelected(By.XPath("//input[@id='中結']"))));

            return Task.CompletedTask;
        }
        public Task TwcE100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _testHelper.WaitElementExists(By.CssSelector("span[sti-post-user-full-name='']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-post-user-full-name][text()='張博文']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcE100_08()
        {
            _driver.SwitchTo().DefaultContent();
            _testHelper.SwitchWindowAndClick("//label[@for='消費性用水服務契約']");

            return Task.CompletedTask;
        }
        public Task TwcE100_09()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司個人資料保護告知事項']");

            return Task.CompletedTask;
        }
        public Task TwcE100_10()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司營業章程']");

            return Task.CompletedTask;
        }
        public Task TwcE100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.ElementClick(By.XPath("//span[text()='簽名']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcE100_12()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='啟動掃描證件']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcE100_13()
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
        public Task TwcE100_14()
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
        public Task TwcE100_15()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='備註內容']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));

            return Task.CompletedTask;
        }
        public Task TwcE100_16()
        {
            var checkStiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            That(checkStiEnd.Selected);

            return Task.CompletedTask;
        }
    }
}