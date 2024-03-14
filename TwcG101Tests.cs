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
            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
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

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件'] input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);
            Thread.Sleep(500);

            stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            That(stiIdentificationChoose.GetAttribute("checked"), Is.EqualTo("true"));

            stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] input")));
            That(stiIdentificationInput.GetAttribute("value"), Is.EqualTo("BBB"));
        }
        public async Task TwcG101_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='超戶申請radio'] input[id='超戶申請group2']")));
            That(stiOverApply.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox!.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_09()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            var errorMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage!.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            var closeMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            closeMessage!.Click();
        }
        public async Task TwcG101_10()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));
            _driver.SwitchTo().Frame(0);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);
            Thread.Sleep(500);

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);
            Thread.Sleep(500);

            stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] input")));
            That(stiEmailInput.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] input")));
            That(stiEmailTelNoInput.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }
        public async Task TwcG101_11()
        {
            _driver.SwitchTo().DefaultContent();

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab!.Text, Is.EqualTo("新增文件"));
        }
        public async Task TwcG101_12()
        {
            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment!.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_13()
        {
            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_14()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveToElement(_driver, "[sti-apply-case-no]");
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG101_15()
        {
            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", chceckSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='申請電子帳單勾選']")));
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));

            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(contract_1!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(contract_2!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(contract_3!.GetAttribute("checked"), Is.EqualTo("true"));

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[type='submit']");

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName!.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_16()
        {
            _driver.SwitchTo().Frame(0);

            var stiIdentificationTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiIdentificationTitle);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件']")));
            That(stiIdentificationTitle.Text, Is.EqualTo("BBB"));

            var stiOverApplyTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiOverApplyTitle);
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

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-06-17";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "[headline='綜合查詢'] button");
            search!.Click();

            That(TestHelper.FindShadowElement(_driver, "stormTable", "span"), Is.Not.Null);
        }
        public async Task TwcG101_18()
        {
            var applyCaseNo = TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();
            Thread.Sleep(1000);

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName!.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_19()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41105536610.pdf", "storm-card[id='finished'] button"), Is.True);
        }
    }
}