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
                That(TestHelper.AccessToken, Is.Not.Empty);

                HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
                That(statusCode, Is.EqualTo(HttpStatusCode.OK));

                _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
                _driver.SwitchTo().Frame(0);

                var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

                var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
                That(approver.Text, Is.EqualTo("張博文"));

                _driver.SwitchTo().DefaultContent();

                var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
                That(attachmentTab!.Text, Is.EqualTo("新增文件"));

                var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
                addAttachment!.Click();

                var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
                TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
                That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

                var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
                upload!.Click();

                _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
                That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

                var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
                checkBox!.Click();

                var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
                confirmButton!.Click();

                _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

                var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
                _wait.Until(ExpectedConditions.UrlContains(targetUrl));

                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
                _driver.SwitchTo().Frame(0);

                var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
                That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
            }

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-bottom > div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
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

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-03-06";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "[headline='綜合查詢'] button");
            search!.Click();

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
        public async Task TwcS101_03()
        {
            var nextPage = TestHelper.FindShadowElement(_driver, "stormPagination", "ul > li:nth-child(3) > a");
            _actions.MoveToElement(nextPage).Click().Perform();

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 5;
            });

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 11 至 15 筆，共 15 筆"));
        }
        public async Task TwcS101_04()
        {
            var backPage = TestHelper.FindShadowElement(_driver, "stormPagination", "ul > li > a");
            _actions.MoveToElement(backPage).Click().Perform();

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 10;
            });

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}