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
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);

            _driver.SwitchTo().DefaultContent();

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(3)");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
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

            chceckSign = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);

            _driver.SwitchTo().DefaultContent();

            addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(3)");

            fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

            infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
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

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
        }

        public async Task TwcRA001_05()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(3)");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA001_06()
        {
            var uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA001_07()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }

        public async Task TwcRA001_08()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcRA001_09()
        {
            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
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
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var 區處別 = TestHelper.FindAndMoveElement(_driver, "storm-card > form > div.mt-3 > storm-select >div.choices");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > form > div.mt-3 > storm-select >div.choices")));
            _actions.MoveToElement(區處別).Click().Perform();

            var 第四區管理處 = TestHelper.FindAndMoveElement(_driver, "div.choices__list.choices__list--dropdown > div.choices__list > [data-id='2']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list.choices__list--dropdown > div.choices__list > [data-id='2']")));
            _actions.MoveToElement(第四區管理處).Click().Perform();

            var applyDateBegin = TestHelper.FindAndMoveElement(_driver, "[label='受理日期起']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[label='受理日期起']")));
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("三月");

            var applyDayBegin = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='三月 6, 2023']")));
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var applyDateEnd = TestHelper.FindAndMoveElement(_driver, "[label='受理日期迄']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[label='受理日期迄']")));
            input = applyDateEnd.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(applyDateEnd).Click().Perform();

            select = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-current-month select");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            
            var applyMonthEnd = new SelectElement(select);
            applyMonthEnd.SelectByText("四月");

            var applyDatEnd = TestHelper.FindAndMoveElement(_driver, "div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='四月 6, 2023']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='四月 6, 2023']")));
            _actions.MoveToElement(applyDatEnd).Click().Perform();

            var 檔案格式 = TestHelper.FindAndMoveElement(_driver, "[label='檔案格式'] div.choices");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='檔案格式'] div.choices")));
            _actions.MoveToElement(檔案格式).Click().Perform();

            var xlsx = TestHelper.FindAndMoveElement(_driver, "div.choices__list [data-value='XLSX']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list [data-value='XLSX']")));
            _actions.MoveToElement(xlsx).Click().Perform();

            That(TestHelper.DownloadFileAndVerify(_driver, "RA001.xlsx", "storm-card > form > div > button"), Is.True);

            string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string filePath = Path.Combine(_downloadDirectory, "RA001.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            string E7Value = worksheet.Cells["E7"].Text;
            string E8Value = worksheet.Cells["E8"].Text;

            That(E7Value == "2" && E8Value == "1", Is.True);
        }
    }
}