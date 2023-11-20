using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcG100Tests()
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
        public async Task TwcG100_01To24()
        {
            await TwcG100_01();
            await TwcG100_02();
            await TwcG100_03();
            await TwcG100_04();
            await TwcG100_05();
            await TwcG100_06();
            await TwcG100_07();
            await TwcG100_08();
            await TwcG100_09();
            await TwcG100_10();
            await TwcG100_11();
            await TwcG100_12();
            await TwcG100_13();
            await TwcG100_14();
            await TwcG100_15();
            await TwcG100_16();
            await TwcG100_17();
            await TwcG100_18();
            await TwcG100_19();
            await TwcG100_20();
            await TwcG100_21();
            await TwcG100_22();
            await TwcG100_23();
            await TwcG100_24();
        }
        public async Task TwcG100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcG100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G100_bmMilitaryApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcG100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiApplyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(stiApplyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            var stiWaterNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            That(stiWaterNo.Text, Is.EqualTo("41105533310"));

            var stiApplyDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));
            That(stiApplyDate.Text, Is.EqualTo("2023年06月13日"));
        }
        public async Task TwcG100_04()
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
        public async Task TwcG100_05()
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
        public async Task TwcG100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
            stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            That(stiIdentificationChoose.GetAttribute("checked"), Is.EqualTo("true"));

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiIdentification);
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-over-apply]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiOverApply);
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
        public async Task TwcG100_08() 
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
        public async Task TwcG100_09()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiPay);
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_10()
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
        public async Task TwcG100_11()
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
        public async Task TwcG100_12()
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
        public async Task TwcG100_13()
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
        public async Task TwcG100_14()
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
        public async Task TwcG100_15()
        {
            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark")));
            That(scanSuccess, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var scanImg = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='credential'] storm-upload > div > div > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcG100_16()
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
        public async Task TwcG100_17()
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
        public async Task TwcG100_18()
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
        public async Task TwcG100_19()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG100_20()
        {
            _driver.SwitchTo().Frame(0);

            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-note]")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcG100_21()
        {
            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_22()
        {
            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
        public async Task TwcG100_23()
        {
            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification-choose]")));
            That(stiIdentificationChoose.Text, Is.EqualTo("撫卹令或撫卹金分領證書"));

            var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification]")));
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_24()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-over-apply]")));
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
    }
}