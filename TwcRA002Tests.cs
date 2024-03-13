using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcRA002Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcRA002Tests()
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
        public async Task TwcRA002_01()
        {
            //0511 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

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

            _driver.SwitchTo().DefaultContent();

            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

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

            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            logout.Click();

            //tw491 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));

            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("tw491");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var login = TestHelper.FindAndMoveToElement(_driver, "button");
            login.Click();

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("謝德威"));

            _driver.SwitchTo().DefaultContent();

            addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

            confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            _driver.SwitchTo().DefaultContent();

            logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            logout.Click();

            //ning53 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA001_bmTransferApply.json"));

            usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("ning53");

            passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            login = TestHelper.FindAndMoveToElement(_driver, "button");
            login.Click();

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("陳宥甯"));

            _driver.SwitchTo().DefaultContent();

            addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

            confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));         
        }

        [Test]
        [Order(1)]
        public async Task TwcRA002_02To09()
        {
            await TwcRA002_02();
            await TwcRA002_03();
            await TwcRA002_04();
            await TwcRA002_05();
            await TwcRA002_06();
            await TwcRA002_07();
            await TwcRA002_08();
            await TwcRA002_09();
        }
        public async Task TwcRA002_02()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcRA002_03()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA002_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task TwcRA002_04()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);
            Thread.Sleep(1000);

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab.Text, Is.EqualTo("新增文件"));
        }

        public async Task TwcRA002_05()
        {
            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA002_06()
        {
            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA002_07()
        {
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }

        public async Task TwcRA002_08()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcRA002_09()
        {
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

        [Test]
        [Order(2)]
        public async Task TwcRA002_10()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/report/RA002");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
            _driver.SwitchTo().Frame(0);

            var 區處別 = TestHelper.FindAndMoveToElement(_driver, "storm-card storm-select div.choices");
            _actions.MoveToElement(區處別).Click().Perform();

            var 第四區管理處 = TestHelper.FindAndMoveToElement(_driver, "div.choices__list.choices__list--dropdown > div.choices__list > [data-id='2']");
            _actions.MoveToElement(第四區管理處).Click().Perform();

            var 選擇年份 = TestHelper.FindAndMoveToElement(_driver, "[label='選擇年份'] div.choices");
            _actions.MoveToElement(選擇年份).Click().Perform();

            var 年份 = TestHelper.FindAndMoveToElement(_driver, "div.choices__list [data-value='2023']");
            _actions.MoveToElement(年份).Click().Perform();

            var 檔案格式 = TestHelper.FindAndMoveToElement(_driver, "[label='檔案格式'] div.choices");
            _actions.MoveToElement(檔案格式).Click().Perform();

            var xlsx = TestHelper.FindAndMoveToElement(_driver, "div.choices__list [data-value='XLSX']");
            _actions.MoveToElement(xlsx).Click().Perform();

            That(TestHelper.DownloadFileAndVerify(_driver, "RA002.xlsx", "storm-card > form > div > button"), Is.True);

            var downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var filePath = Path.Combine(downloadDirectory, "RA002.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            string D6Value = worksheet.Cells["D6"].Text;
            string F6Value = worksheet.Cells["F6"].Text;
            string E7Value = worksheet.Cells["E7"].Text;

            That(E7Value == "1" && D6Value == "2" && F6Value == "1", Is.True);
        }
    }
}