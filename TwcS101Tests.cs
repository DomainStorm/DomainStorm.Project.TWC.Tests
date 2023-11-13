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
        public async Task TwcS101_01()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            for (int i = 0; i < 15; i++)
            {
                TestHelper.AccessToken = await TestHelper.GetAccessToken();
                HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
                _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
                _driver.SwitchTo().Frame(0);

                var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

                var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
                That(chceckSign!, Is.Not.Null);

                _driver.SwitchTo().DefaultContent();

                var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
                _actions.MoveToElement(addFileButton).Click().Perform();

                var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
                var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
                lastHiddenInput.SendKeys(filePath);

                var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
                That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

                var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
                _actions.MoveToElement(uploadButton).Click().Perform();
                That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

                var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
                _actions.MoveToElement(checkButton).Click().Perform();
                That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

                var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
                _actions.MoveToElement(infoButton).Click().Perform();

                var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
                _wait.Until(ExpectedConditions.UrlContains(targetUrl));
                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
                That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
            }

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            That(TestHelper.WaitStormTableUpload(_driver, "tr > td"), Is.Not.Null);

            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(1)]
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

            var 受理日期起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='受理日期起']")));
            var input = 受理日期起.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期起 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期起).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月起 = new SelectElement(select);
            受理月起.SelectByText("March");

            var 受理日起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='March 6, 2023']")));
            _actions.MoveToElement(受理日起).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell > span"), Is.Not.Null);

            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
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

            stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo.Text, Is.EqualTo("顯示第 11 至 15 筆，共 15 筆"));
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

            stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}