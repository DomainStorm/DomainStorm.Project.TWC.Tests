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
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        [Repeat(15)]
        public async Task CreateSingleFormTest()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);

            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);
            await Task.Delay(1000);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(_driver =>
            {
                var spanElement = _driver.FindElement(By.CssSelector("span[sti-post-user-full-name='']"));

                return spanElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//button[contains(text(), '新增文件')]"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var pdfFile = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, pdfFile, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var file = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(file.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var upload = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '上傳')]")));

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-edit-table"));

                return element;
            });

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            That(checkBox.Selected);

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(1)]
        public async Task TwcS101_01()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/unfinished");

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-table"));
                return element != null;
            });

            var pageInfo = TestHelper.WaitStormTableUpload(_driver, "div.table-bottom > div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(2)]
        public async Task TwcS101_02To04()
        {
            await TwcS101_02();
            await TwcS101_03();
            await TwcS101_04();
        }
        public async Task TwcS101_02()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card"));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));

                return pageInfo.Text == "顯示第 1 至 10 筆，共 15 筆";
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
        public async Task TwcS101_03()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            var nextPage = stormPagination.GetShadowRoot().FindElement(By.CssSelector("ul > li:nth-child(3) > a"));
            _actions.MoveToElement(nextPage).Click().Perform();

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 5;
            });

            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo!.Text, Is.EqualTo("顯示第 11 至 15 筆，共 15 筆"));
        }
        public async Task TwcS101_04()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            var backPage = stormPagination.GetShadowRoot().FindElement(By.CssSelector("ul > li > a"));
            _actions.MoveToElement(backPage).Click().Perform();

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 10;
            });

            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}