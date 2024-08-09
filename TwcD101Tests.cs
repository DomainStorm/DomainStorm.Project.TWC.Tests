using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcD101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcD101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcD101_01To09()
        {
            await TwcD101_01();
            await TwcD101_02();
            await TwcD101_03();
            await TwcD101_04();
            await TwcD101_05();
            await TwcD101_06();
            await TwcD101_07();
            await TwcD101_08();
            await TwcD101_09();
        }
        public async Task TwcD101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcD101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmAolishedApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-D101_bmAolishedApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcD101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='中結']")));
            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcD101_05()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindNavigationBySpan(_driver, "受理登記");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD101_06()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='受理登記'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcD101_07()
        {
            var confirmButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions button");
            _actions.MoveToElement(confirmButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-actions button")));

            var href = TestHelper.FindNavigationBySpan(_driver, "掃描拍照");
            _actions.MoveToElement(href).Click().Perform();

            var scanButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='credential'] button:nth-child(2)");
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dz-success-mark")));
            That(scanSuccess, Is.Not.Null, "未上傳");

            var scanImg = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img");
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcD101_08()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='受理登記'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcD101_09()
        {
            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(waterServiceAgreement.GetAttribute("checked"), Is.EqualTo("true"));

            var dataProtectionNotice = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(dataProtectionNotice.GetAttribute("checked"), Is.EqualTo("true"));

            var companyRegulation = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(companyRegulation.GetAttribute("checked"), Is.EqualTo("true"));

            var img = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(6) > img");
            That(img, Is.Not.Null);
        }

        [Test]
        [Order(1)]
        public async Task TwcD101_10To12()
        {
            await TwcD101_10();
            await TwcD101_11();
            await TwcD101_12();
        }
        public async Task TwcD101_10()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBeginInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起'] input")));

            string formattedApplyDateBegin = "2023-06-03";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='綜合查詢'] button");
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span"), Is.Not.Null);
        }
        public async Task TwcD101_11()
        {
            var applyCaseNo = TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var href = TestHelper.FindNavigationBySpan(_driver, "掃描拍照");
            _actions.MoveToElement(href).Click().Perform();

            var img = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(6) > img");
            That(img, Is.Not.Null);
        }
        public async Task TwcD101_12()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41104433664.pdf", "storm-card[id='finished'] button"), Is.True);
        }
    }
}