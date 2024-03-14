using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcC101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcC101Tests()
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
        public async Task TwcC101_01To08()
        {
            await TwcC101_01();
            await TwcC101_02();
            await TwcC101_03();
            await TwcC101_04();
            await TwcC101_05();
            await TwcC101_06();
            await TwcC101_07();
            await TwcC101_08();
        }
        public async Task TwcC101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcC101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C101_bmDisableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcC101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcC101_04()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox!.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC101_05()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            var errorMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage!.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            var closeMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            closeMessage!.Click();
        }
        public async Task TwcC101_06()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));

            var scanButton = TestHelper.FindAndMoveToElement(_driver, "[headline='掃描拍照'] button:nth-child(2)");
            scanButton!.Click();

            var scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcC101_07()
        {
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
        public async Task TwcC101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(contract_1!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(contract_2!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(contract_3!.GetAttribute("checked"), Is.EqualTo("true"));

            var img = TestHelper.FindAndMoveToElement(_driver, "storm-card:nth-child(6) > img");
            That(img, Is.Not.Null);
        }

        [Test]
        [Order(1)]
        public async Task TwcC101_09To11()
        {
            await TwcC101_09();
            await TwcC101_10();
            await TwcC101_11();
        }
        public async Task TwcC101_09()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-06-03";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "[headline='綜合查詢'] button");
            search!.Click();

            That(TestHelper.FindShadowElement(_driver, "stormTable", "span"), Is.Not.Null);
        }
        public async Task TwcC101_10()
        {
            var applyCaseNo = TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();
            Thread.Sleep(1000);

            var img = TestHelper.FindAndMoveToElement(_driver, "storm-card:nth-child(6) > img");
            That(img, Is.Not.Null);
        }
        public async Task TwcC101_11()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41101699338.pdf", "storm-card[id='finished'] button"), Is.True);
        }
    }
}