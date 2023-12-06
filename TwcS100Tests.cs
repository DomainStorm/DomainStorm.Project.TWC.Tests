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
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
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

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcS100_02()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            var buttonTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            That(buttonTitle.Text, Is.EqualTo("登入"));
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

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcS100_06()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_07()
        {
            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_08()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }
        public async Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcS100_10()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
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

            var applyDateBegin = TestHelper.FindAndMoveElement(_driver, "[label='受理日期起']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[label='受理日期起']")));
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("三月");

            var applyDayBegin = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']")));
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell > span"), Is.Not.Null);

            var stormTable = TestHelper.FindAndMoveElement(_driver, "storm-card.bg-gradient-dark > storm-document-list-detail > div > storm-table");
            var userNameOne = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr > td[data-field='userName'] > storm-table-cell > span"));
            var userNameTwo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(2) > td[data-field='userName'] > storm-table-cell > span"));
            That(userNameOne.GetAttribute("innerText"), Is.EqualTo("張博文"));
            That(userNameTwo.GetAttribute("innerText"), Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_12()
        {
            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            await TestHelper.Login(_driver, "4e03", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var stormDropdown = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-sidenav > aside > ul > li > storm-dropdown > div > a > div > div > span")));
            That(stormDropdown.GetAttribute("innerText"), Is.EqualTo("草屯營運所業務股 - 業務員"));
        }
        public async Task TwcS100_13()
        {
            var applyDateBegin = TestHelper.FindAndMoveElement(_driver, "[label='受理日期起']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[label='受理日期起']")));
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));

            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("六月");

            var applyDayBegin = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='六月 17, 2023']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='六月 17, 2023']")));
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();
            That(TestHelper.WaitStormTableUpload(_driver, "td > p")!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}