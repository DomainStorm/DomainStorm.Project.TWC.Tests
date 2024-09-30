using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcB101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcB101Tests).GetMethod(testMethod!);
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
        public Task TwcB101_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcB101_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B101_bmRecoverApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcB101_03To08()
        {
            TwcB101_03();
            TwcB101_04();
            TwcB101_05();
            TwcB101_06();
            TwcB101_07();
            TwcB101_08();

            return Task.CompletedTask;
        }
        public Task TwcB101_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }
        public Task TwcB101_04()
        {
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

            var targetRow = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr")).First(row =>
            {
                var fileNameElement = row.FindElement(By.CssSelector("td[data-field='name'] storm-table-cell span span"));
                return fileNameElement.Text == "twcweb_01_1_夾帶附件1.pdf";
            });

            var deleteButton = targetRow.FindElement(By.CssSelector("td.action storm-table-toolbar storm-toolbar-item storm-button button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='是否刪除？']")));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '刪除')]"));

            _wait.Until(driver =>
            {
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 1;
            });

            var remainingFileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell span"));
            That(remainingFileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));

            return Task.CompletedTask;
        }
        public Task TwcB101_05()
        {
            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcB101_06()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='【受理】未核章']")));
            That(content.Text, Is.EqualTo("【受理】未核章"));

            _testHelper.ElementClick(By.XPath("//button[text()='確定']"));

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));

            return Task.CompletedTask;
        }
        public Task TwcB101_07()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("張博文"));

            return Task.CompletedTask;
        }
        public Task TwcB101_08()
        {
            _driver.SwitchTo().DefaultContent();

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

        public Task TwcB101_09To10()
        {
            TwcB101_09();
            TwcB101_10();

            return Task.CompletedTask;
        }
        public Task TwcB101_09()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/unfinished", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.MoveAndCheck("#消費性用水服務契約");
            _testHelper.MoveAndCheck("#公司個人資料保護告知事項");
            _testHelper.MoveAndCheck("#公司營業章程");

            return Task.CompletedTask;
        }

        public Task TwcB101_10()
        {
            _testHelper.CheckElementText("a[download='twcweb_01_1_夾帶附件2.pdf']", "twcweb_01_1_夾帶附件2.pdf");

            return Task.CompletedTask;
        }
    }
}