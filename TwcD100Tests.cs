using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcD100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcD100Tests()
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
        public async Task TwcD100_01To16()
        {
            await TwcD100_01();
            await TwcD100_02();
            await TwcD100_03();
            await TwcD100_04();
            await TwcD100_05();
            await TwcD100_06();
            await TwcD100_07();
            await TwcD100_08();
            await TwcD100_09();
            await TwcD100_10();
            await TwcD100_11();
            await TwcD100_12();
            await TwcD100_13();
            await TwcD100_14();
            await TwcD100_15();
            await TwcD100_16();
        }
        public async Task TwcD100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcD100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmAolishedApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-D100_bmAolishedApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcD100_03()
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
        public async Task TwcD100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);
            await Task.Delay(1000);

            idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            var idNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no]")));
            That(idNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcD100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-note] > input")));
            stiNoteInput.SendKeys("備註內容" + Keys.Tab);
            await Task.Delay(1000);

            stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-note] > input")));
            stiNoteInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-note]")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcD100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcD100_08()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = TestHelper.FindAndMoveElement(_driver, "input[id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().DefaultContent();

            waterServiceAgreement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", waterServiceAgreement);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            waterServiceAgreement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));
            That(waterServiceAgreement.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD100_09()
        {
            var dataProtectionNotice = TestHelper.FindAndMoveElement(_driver, "input[id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            dataProtectionNotice = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", dataProtectionNotice);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            dataProtectionNotice = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            That(dataProtectionNotice.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD100_10()
        {
            var companyRegulation = TestHelper.FindAndMoveElement(_driver, "input[id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            companyRegulation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", companyRegulation);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            companyRegulation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));
            That(companyRegulation.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] button.btn-primary")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", signButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", signButton);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dz-image > img[alt='簽名_001.tiff']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signImg = TestHelper.FindAndMoveElement(_driver, "div.dz-image > img[alt='簽名_001.tiff']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dz-image > img[alt='簽名_001.tiff']")));
            That(signImg, Is.Not.Null);
        }
        public async Task TwcD100_12()
        {
            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark");
            That(scanSuccess, Is.Not.Null, "未上傳");

            var scanImg = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcD100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var attachmentButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(attachmentButton).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));

            var attachmentName = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));

            var fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
            That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
            That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }
        public async Task TwcD100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button[type='submit']")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button[type='submit']")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcD100_15()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='註記']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='註記']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcD100_16()
        {
            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
    }
}