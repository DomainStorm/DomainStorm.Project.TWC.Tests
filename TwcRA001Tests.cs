using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using OfficeOpenXml;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcRA001Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcRA001Tests()
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
        public async Task TwcRA001_01()
        {
            //0511 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var submitButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info");
            _actions.MoveToElement(submitButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            //tw491 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));

            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("謝德威"));

            _driver.SwitchTo().DefaultContent();

            href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            submitButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info");
            _actions.MoveToElement(submitButton).Click().Perform();

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
        public async Task TwcRA001_02To09()
        {
            await TwcRA001_02();
            await TwcRA001_03();
            await TwcRA001_04();
            await TwcRA001_05();
            await TwcRA001_06();
            await TwcRA001_07();
            await TwcRA001_08();
            await TwcRA001_09();
        }
        public async Task TwcRA001_02()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcRA001_03()

        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA001_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task TwcRA001_04()
        {
            await TestHelper.Login(_driver, "ning53", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }

        public async Task TwcRA001_05()
        {
            var createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA001_06()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA001_07()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("陳宥甯"));
        }

        public async Task TwcRA001_08()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcRA001_09()
        {
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

        [Test]
        [Order(2)]
        public async Task TwcRA001_10To11()
        {
            await TwcRA001_10();
            await TwcRA001_11();
        }
        public async Task TwcRA001_10()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
        }

        public async Task TwcRA001_11()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/report/RA001");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
            _driver.SwitchTo().Frame(0);

            var 區處別 = TestHelper.FindAndMoveElement(_driver, "storm-card > form > div.mt-3 > storm-select >div.choices");
            _actions.MoveToElement(區處別).Click().Perform();

            var 第四區管理處 = TestHelper.FindAndMoveElement(_driver, "div.choices__list.choices__list--dropdown > div.choices__list > [data-id='2']");
            _actions.MoveToElement(第四區管理處).Click().Perform();
            
            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-03-06";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var applyDateEnd = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期迄']")));
            var applyDateEndInput = applyDateEnd.GetShadowRoot().FindElement(By.CssSelector("div input"));

            string formattedApplyDateEnd = "2023-04-06";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateEnd}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateEndInput);

            //var applyDateBegin = TestHelper.FindAndMoveElement(_driver, "[label='受理日期起']");
            //var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            //_actions.MoveToElement(input).Click().Perform();

            //var select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            //var applyMonthBegin = new SelectElement(select);
            //applyMonthBegin.SelectByText("三月");

            //var applyDayBegin = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']");
            //_actions.MoveToElement(applyDayBegin).Click().Perform();

            //var applyDateEnd = TestHelper.FindAndMoveElement(_driver, "[label='受理日期迄']");
            //input = applyDateEnd.GetShadowRoot().FindElement(By.CssSelector("input"));
            //_actions.MoveToElement(applyDateEnd).Click().Perform();

            //select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            //var applyMonthEnd = new SelectElement(select);
            //applyMonthEnd.SelectByText("四月");

            //var applyDatEnd = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='四月 6, 2023']");
            //_actions.MoveToElement(applyDatEnd).Click().Perform();

            var 檔案格式 = TestHelper.FindAndMoveElement(_driver, "[label='檔案格式'] div.choices");
            _actions.MoveToElement(檔案格式).Click().Perform();

            var xlsx = TestHelper.FindAndMoveElement(_driver, "div.choices__list [data-value='XLSX']");
            _actions.MoveToElement(xlsx).Click().Perform();

            That(TestHelper.DownloadFileAndVerify(_driver, "RA001.xlsx", "storm-card > form > div > button"), Is.True);

            var _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var filePath = Path.Combine(_downloadDirectory, "RA001.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            string E7Value = worksheet.Cells["E7"].Text;
            string E8Value = worksheet.Cells["E8"].Text;

            That(E7Value == "2" && E8Value == "1", Is.True);
        }
    }
}