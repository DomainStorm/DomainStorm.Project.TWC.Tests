using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcE100Tests()
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
        public async Task TwcE100_01To20()
        {
            await TwcE100_01();
            await TwcE100_02();
            await TwcE100_03();
            await TwcE100_04();
            await TwcE100_05();
            await TwcE100_06();
            await TwcE100_07();
            await TwcE100_08();
            await TwcE100_09();
            await TwcE100_10();
            await TwcE100_11();
            await TwcE100_12();
            await TwcE100_13();
            await TwcE100_14();
            await TwcE100_15();
            await TwcE100_16();
            await TwcE100_17();
            await TwcE100_18();
            await TwcE100_19();
            await TwcE100_20();
        }
        public async Task TwcE100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcE100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E100_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcE100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);

            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcE100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            stiTrusteeIdNoInput.SendKeys("A123456789" + Keys.Tab);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiTrusteeIdNo);
            That(stiTrusteeIdNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcE100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiApplyEmail = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='申請電子帳單勾選']")));
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            stiNote.SendKeys("備註內容" + Keys.Tab);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcE100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            Thread.Sleep(750);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiEnd);
            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_08()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }
        public async Task TwcE100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 消費性用水服務契約);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 消費性用水服務契約);
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_10()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司個人資料保護告知事項);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司個人資料保護告知事項);
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司營業章程);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司營業章程);
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_12()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div > div > button.btn-primary")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", signButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", signButton);

            var signSuccess = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div:nth-child(2) > storm-upload > div > div > div:nth-child(2) > div.dz-success-mark")));
            That(signSuccess, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signImg = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div:nth-child(2) > storm-upload > div > div > div:nth-child(2) > div.dz-image")));
            That(signImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcE100_13()
        {
            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark")));
            That(scanSuccess, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var scanImg = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='credential'] storm-upload > div > div > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcE100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var firstFile = "twcweb_01_1_夾帶附件1.pdf";
            var firstFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", firstFile);
            lastHiddenInput.SendKeys(firstFilePath);

            lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var secondFile = "twcweb_01_1_夾帶附件2.pdf";
            var secondFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", secondFile);
            lastHiddenInput.SendKeys(secondFilePath);

            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "tr:nth-child(2) > td[data-field='name'] > storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            That(TestHelper.WaitStormEditTableUpload(_driver, "tr:nth-child(2) > td[data-field='name'] > storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));

        }
        public async Task TwcE100_15()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitleOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > h5")));
            var hintTitleTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > h5:nth-child(2)")));
            That(hintTitleOne.Text, Is.EqualTo("【聯絡電話】未填寫"));
            That(hintTitleTwo.Text, Is.EqualTo("【Email】未填寫或不正確"));

            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(closeButton).Click().Perform();
        }
        public async Task TwcE100_16()
        {
            _driver.SwitchTo().Frame(0);

            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiEmail);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email] > input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);
            Thread.Sleep(500);

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email-tel-no] > input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
        public async Task TwcE100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

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
        public async Task TwcE100_18()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-note]")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcE100_19()
        {
            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='中結']")));
            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcE100_20()
        {
            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
    }
}