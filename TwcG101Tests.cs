using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcG101Tests()
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
        public async Task TwcG101_01To16()
        {
            await TwcG101_01();
            await TwcG101_02();
            await TwcG101_03();
            await TwcG101_04();
            await TwcG101_05();
            await TwcG101_06();
            await TwcG101_07();
            await TwcG101_08();
            await TwcG101_09();
            await TwcG101_10();
            await TwcG101_11();
            await TwcG101_12();
            await TwcG101_13();
            await TwcG101_14();
            await TwcG101_15();
            await TwcG101_16();
        }
        public async Task TwcG101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcG101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G101_bmMilitaryApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcG101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcG101_05()
        {
            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            Thread.Sleep(500);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='申請電子帳單勾選']")));
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_06()
        {
            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='檢附證件group3']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
            Thread.Sleep(500);

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件'] > input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);
            Thread.Sleep(500);

            stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            That(stiIdentificationChoose.GetAttribute("checked"), Is.EqualTo("true"));

            stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
            That(stiIdentificationInput.GetAttribute("value"), Is.EqualTo("BBB"));
        }
        public async Task TwcG101_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='超戶申請radio'] > input[id='超戶申請group2']")));
            That(stiOverApply.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_09()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info");
            _actions.MoveToElement(submitButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcG101_10()
        {
            var confirmButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-popup > div.swal2-actions > button.swal2-confirm");
            _actions.MoveToElement(confirmButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#person']");
            _actions.MoveToElement(href).Click().Perform();
            _driver.SwitchTo().Frame(0);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] > input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);
            Thread.Sleep(500);

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] > input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);
            Thread.Sleep(500);

            stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] > input")));
            That(stiEmailInput.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] > input")));
            That(stiEmailTelNoInput.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }
        public async Task TwcG101_11()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }
        public async Task TwcG101_12()
        {
            var createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_13()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_14()
        {
            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info");
            _actions.MoveToElement(submitButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG101_15()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_1'] > div.d-flex > div.form-check > input")));
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_2'] > div.d-flex > div.form-check > input")));
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_3'] > div.d-flex > div.form-check > input")));
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var attachmentName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(attachmentName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='申請電子帳單勾選']")));
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", chceckSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");
        }
        public async Task TwcG101_16()
        {
            var stiIdentificationTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件']")));
            That(stiIdentificationTitle.Text, Is.EqualTo("BBB"));

            var stiOverApplyTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='超戶申請radio']")));
            That(stiOverApplyTitle.Text, Is.EqualTo("否"));
        }

        [Test]
        [Order(1)]
        public async Task TwcG101_17To19()
        {
            await TwcG101_17();
            await TwcG101_18();
            await TwcG101_19();
        }
        public async Task TwcG101_17()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var applyDateBegin = TestHelper.FindAndMoveElement(_driver, "[label='受理日期起']");
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("六月");

            var applyDayBegin = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='六月 17, 2023']");
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = TestHelper.FindAndMoveElement(_driver, "storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button");
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell span"), Is.Not.Null);
        }
        public async Task TwcG101_18()
        {
            var applyCaseNo = TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(attachmentName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_19()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41105536610.pdf", "storm-card[id='finished'] > div.float-end > div:nth-child(3) > button"), Is.True);
        }
    }
}