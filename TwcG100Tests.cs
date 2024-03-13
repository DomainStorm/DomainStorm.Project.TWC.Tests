using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
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

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);
            Thread.Sleep(1000);

            idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var idNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no]")));
            That(idNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcG100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);

            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[value='撫卹令或撫卹金分領證書']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiIdentificationChoose);

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);
            Thread.Sleep(1000);

            stiIdentificationInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件'] > input")));
            stiIdentificationInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-over-apply]")));
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
        public async Task TwcG100_08()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNoteInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-note] > input")));
            stiNoteInput.SendKeys("備註內容" + Keys.Tab);
            Thread.Sleep(1000);

            stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-note] > input")));
            stiNoteInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-note]")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcG100_09()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_10()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcG100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "[id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().DefaultContent();

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_1);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            That(contract_1.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_12()
        {
            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "[id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_2);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            That(contract_2.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_13()
        {
            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "[id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_3);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            That(contract_3.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signature = TestHelper.FindAndMoveToElement(_driver, "[id='signature'] button:nth-child(2)");
            signature.Click();

            var img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");
            That(img, Is.Not.Null);
        }
        public async Task TwcG100_15()
        {
            var scanButton = TestHelper.FindAndMoveToElement(_driver, "[headline='掃描拍照'] button:nth-child(2)");
            scanButton.Click();

            var scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcG100_16()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab.Text, Is.EqualTo("新增文件"));

            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            var attachment1 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment1, "input.dz-hidden-input:nth-of-type(3)");

            var attachment2 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, attachment2, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });
        }
        public async Task TwcG100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            var errorMessage1 = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage1.Text, Is.EqualTo("【聯絡電話】未填寫"));

            var errorMessage2 = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5:nth-child(2)");
            That(errorMessage2.Text, Is.EqualTo("【Email】未填寫或不正確"));

            var closeMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            closeMessage.Click();
        }
        public async Task TwcG100_18()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));
            _driver.SwitchTo().Frame(0);

            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiEmail);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);
            Thread.Sleep(1000);

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEmail = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
        public async Task TwcG100_19()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG100_20()
        {
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