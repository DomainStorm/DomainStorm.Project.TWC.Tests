using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcE101Tests()
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
        public async Task TwcE101_01To02()
        {
            await TwcE101_01();
            await TwcE101_02();
        }
        public async Task TwcE101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcE101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E101_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(1)]
        public async Task TwcE101_03To12()
        {
            await TwcE101_03();
            await TwcE101_04();
            await TwcE101_05();
            await TwcE101_06();
            await TwcE101_07();
            await TwcE101_08();
            await TwcE101_09();
            await TwcE101_10();
            await TwcE101_11();
            await TwcE101_12();
        }
        public async Task TwcE101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            _wait.Until(driver =>
            {
                var checkbox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='中結']")));

                return checkbox.GetAttribute("checked") != null;
            });

            var checkStiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            That(checkStiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));

                return signElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }

        public async Task TwcE101_05()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindNavigationBySpan(_driver, "夾帶附件");
            _actions.MoveToElement(href).Click().Perform();
        }

        public async Task TwcE101_06()
        {
            var addAttachment = TestHelper.FindAndMoveElement(_driver, "//button[text()='新增文件']");
            _actions.MoveToElement(addAttachment).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf";
            });
        }

        public async Task TwcE101_07()
        {
            var upload = TestHelper.FindAndMoveElement(_driver, "//button[text()='上傳']");
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='上傳']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_08()
        {
            var checkBox = TestHelper.FindAndMoveElement(_driver, "//input[@id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkBox).Click().Perform();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_09()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "//button[text()='確認受理']");
            _actions.MoveToElement(submitButton).Click().Perform();

            var hintTitle = TestHelper.FindAndMoveElement(_driver, "//h5[text()='【聯絡電話】未填寫']");
            That(hintTitle.Text, Is.EqualTo("【聯絡電話】未填寫"));

            var confirmButton = TestHelper.FindAndMoveElement(_driver, "//button[text()='確定']");
            _actions.MoveToElement(confirmButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));
        }

        public async Task TwcE101_10()
        {
            _driver.SwitchTo().Frame(0);

            var phoneInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-email-tel-no]/input")));
            phoneInput.SendKeys("02-12345678" + Keys.Tab);

            _wait.Until(driver =>
            {
                var phoneElement = driver.FindElement(By.XPath("//span[@id='電子帳單聯絡電話']/input"));
                return phoneElement != null;
            });

            var phoneElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='電子帳單聯絡電話']/input")));
            That(phoneElement.GetAttribute("value"), Is.EqualTo("02-12345678"));

            _driver.SwitchTo().DefaultContent();
        }

        public async Task TwcE101_11()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "//button[text()='確認受理']");
            _actions.MoveToElement(submitButton).Click().Perform();


            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確認受理']")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveElement(_driver, "//span[@sti-apply-case-no]");
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        public async Task TwcE101_12()
        {
            var phone = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='電子帳單聯絡電話']")));
            That(phone.Text, Is.EqualTo("02-12345678"));

            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = TestHelper.FindAndMoveElement(_driver, "//input[@id='消費性用水服務契約']");
            That(waterServiceAgreement.GetAttribute("checked"), Is.EqualTo("true"));

            var dataProtectionNotice = TestHelper.FindAndMoveElement(_driver, "//input[@id='公司個人資料保護告知事項']");
            That(dataProtectionNotice.GetAttribute("checked"), Is.EqualTo("true"));

            var companyRegulation = TestHelper.FindAndMoveElement(_driver, "//input[@id='公司營業章程']");
            That(companyRegulation.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(2)]
        public async Task TwcE101_13To15()
        {
            await TwcE101_13();
            await TwcE101_14();
            await TwcE101_15();
        }
        public async Task TwcE101_13()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            string formattedApplyDateBegin = "2023-06-03";
            var applyDateBeginInput = TestHelper.FindAndMoveElement(_driver, "//storm-input-group[@label='受理日期起']//input");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveElement(_driver, "//button[text()='查詢']");
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span"), Is.Not.Null);
        }

        public async Task TwcE101_14()
        {
            var applyCaseNo = TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var href = TestHelper.FindNavigationBySpan(_driver, "夾帶附件");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveElement(_driver, "//a[text()='twcweb_01_1_夾帶附件1.pdf']");
            That(attachmentName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_15()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41881288118.pdf", "//button[text()='轉PDF']"), Is.True);
        }
    }
}