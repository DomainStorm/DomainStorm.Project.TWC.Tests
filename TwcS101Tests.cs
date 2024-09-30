using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcS101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _actions = new Actions(_driver);
            _testHelper = new TestHelper(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        [Repeat(15)]
        public Task CreateFormBy0511_Test()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

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

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));
            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

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
        [Order(1)]
        public Task TwcS101_01()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/unfinished", By.CssSelector("storm-table"));

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var content = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(content.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcS101_02To04()
        {
            TwcS101_02();
            TwcS101_03();
            TwcS101_04();

            return Task.CompletedTask;
        }
        public Task TwcS101_02()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-table"));

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
                return pageInfo.Text == "顯示第 1 至 10 筆，共 15 筆";
            });

            var pageInfoFinal = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfoFinal.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));

            return Task.CompletedTask;
        }
        public Task TwcS101_03()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            var nextPage = stormPagination.GetShadowRoot().FindElement(By.CssSelector("ul > li:nth-child(3) > a"));
            _actions.MoveToElement(nextPage).Click().Perform();

            _wait.Until(driver =>
            {
                var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
                return pageInfo.Text == "顯示第 11 至 15 筆，共 15 筆";
            });

            var pageInfoFinal = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfoFinal.Text, Is.EqualTo("顯示第 11 至 15 筆，共 15 筆"));

            return Task.CompletedTask;
        }
        public Task TwcS101_04()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            var backPage = stormPagination.GetShadowRoot().FindElement(By.CssSelector("ul > li > a"));
            _actions.MoveToElement(backPage).Click().Perform();

            _wait.Until(driver =>
            {
                var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
                return pageInfo.Text == "顯示第 1 至 10 筆，共 15 筆";
            });

            var pageInfoFinal = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfoFinal.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));

            return Task.CompletedTask;
        }
    }
}
