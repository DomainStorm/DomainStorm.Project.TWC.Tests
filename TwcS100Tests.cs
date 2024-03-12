using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcS100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcS100_01To02()
        {
            await TwcS100_01();
            await TwcS100_02();
        }
        public async Task TwcS100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);

            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
            
            var createAttachmentButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var submitButton = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

            submitButton = TestHelper.FindAndMoveToElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcS100_02()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            That(button.Text, Is.EqualTo("登入"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS100_03To10()
        {
            await TwcS100_03();
            await TwcS100_04();
            await TwcS100_05();
            await TwcS100_06();
            await TwcS100_07();
            await TwcS100_08();
            await TwcS100_09();
            await TwcS100_10();
        }
        public async Task TwcS100_03()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcS100_04()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcS100_05()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }
        public async Task TwcS100_06()
        {
            var createAttachmentButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_07()
        {
            var submitButton = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_08()
        {
            var href = TestHelper.FindShadowRootElement(_driver, "[href='#person']");
            _actions.MoveToElement(href).Click().Perform();

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcS100_10()
        {
            var submitButton = TestHelper.FindAndMoveToElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(2)]
        public async Task TwcS100_11To13()
        {
            await TwcS100_11();
            await TwcS100_12();
            await TwcS100_13();
        }
        public async Task TwcS100_11()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var applyDateBegin = TestHelper.FindAndMoveToElement(_driver, "[label='受理日期起']");
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveToElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("三月");

            var applyDayBegin = TestHelper.FindAndMoveToElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']");
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell span"), Is.Not.Null);

            var stormTable = TestHelper.FindAndMoveToElement(_driver, "storm-card.bg-gradient-dark > storm-document-list-detail > div > storm-table");
            var userNameOne = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr > td[data-field='userName'] > storm-table-cell span"));
            That(userNameOne.GetAttribute("innerText"), Is.EqualTo("張博文"));

            var userNameTwo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(2) > td[data-field='userName'] > storm-table-cell span"));
            That(userNameTwo.GetAttribute("innerText"), Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_12()
        {
            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            await TestHelper.Login(_driver, "4e03", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var stormDropdown = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-sidenav > aside > ul > li > storm-dropdown > div > a > div > div > span")));
            That(stormDropdown.GetAttribute("innerText"), Is.EqualTo("草屯營運所業務股 - 業務員"));
        }
        public async Task TwcS100_13()
        {
            var applyDateBegin = TestHelper.FindAndMoveToElement(_driver, "[label='受理日期起']");
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveToElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("六月");

            var applyDayBegin = TestHelper.FindAndMoveToElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='六月 17, 2023']");
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td > p")!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}