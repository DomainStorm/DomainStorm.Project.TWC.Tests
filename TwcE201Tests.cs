using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE201Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcE201Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcE201Tests).GetMethod(testMethod!);
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
        public Task TwcE201_01To03()
        {
            TwcE201_01();
            TwcE201_02();
            TwcE201_03();

            return Task.CompletedTask;
        }
        public Task TwcE201_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        public Task TwcE201_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        public Task TwcE201_03()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground2.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task TwcE201_04To06()
        {
            TwcE201_04();
            TwcE201_05();
            TwcE201_06();

            return Task.CompletedTask;
        }
        public Task TwcE201_04()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/batch", By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var checkAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[class='form-check-input']"));
            _actions.MoveToElement(checkAll).Click().Perform();

            _testHelper.ElementClick(By.XPath("//button"));

            return Task.CompletedTask;
        }
        public Task TwcE201_05()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf", "twcweb_01_1_夾帶附件2.pdf" }, "input.dz-hidden-input:nth-of-type(2)");
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
        public Task TwcE201_06()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認夾帶']"));

            Thread.Sleep(1000);

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

            foreach (var row in rows)
            {
                var attachedElement = row.FindElement(By.CssSelector("td[data-field='attached'] i"));
                That(attachedElement.Text, Is.EqualTo("attach_file"));
            }

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcE201_07()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-06-30";
            var applyDateBeginSelect = _testHelper.WaitElementVisible(By.CssSelector("storm-input-group[label='受理日期起'] input"));
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
    }
}