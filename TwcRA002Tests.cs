using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using OfficeOpenXml;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcRA002Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcRA002Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _actions = new Actions(_driver);
            _testHelper = new TestHelper(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public Task CreateFormBy0511_Test()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task CreateFormBytw491_Test()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            _testHelper.Login("tw491", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("謝德威"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");
            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task CreateFormByning53_Test()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA001_bmTransferApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            _testHelper.Login("ning53", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("陳宥甯"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");
            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcRA002_02To09()
        {
            TwcRA002_02();
            TwcRA002_03();
            TwcRA002_04();
            TwcRA002_05();
            TwcRA002_06();
            TwcRA002_07();
            TwcRA002_08();
            TwcRA002_09();

            return Task.CompletedTask;
        }
        public Task TwcRA002_02()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }
        public Task TwcRA002_03()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA002_bmTransferApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        public Task TwcRA002_04()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }

        public Task TwcRA002_05()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            return Task.CompletedTask;
        }

        public Task TwcRA002_06()
        {
            var content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            return Task.CompletedTask;
        }

        public Task TwcRA002_07()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("張博文"));

            return Task.CompletedTask;
        }

        public Task TwcRA002_08()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }

        public Task TwcRA002_09()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcRA002_10()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/report/RA002", By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);
            _testHelper.WaitElementExists(By.CssSelector("storm-card"));
            _testHelper.ElementClick(By.CssSelector("div.choices__item[data-id='1']"));
            _testHelper.ElementClick(By.CssSelector("div.choices__item[data-id='2']"));
            _testHelper.ElementClick(By.XPath("//storm-select[@label='選擇年份']/div[@class='choices']"));
            _testHelper.ElementClick(By.XPath("//div[text()='2023']"));
            _testHelper.ElementClick(By.XPath("//storm-select[@label='檔案格式']/div[@class='choices']"));
            _testHelper.ElementClick(By.XPath("//div[text()='XLSX']"));
            _testHelper.DownloadFileAndVerify("RA002.xlsx", "//button[text()='下載']");

            var downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var filePath = Path.Combine(downloadDirectory, "RA002.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            string D6Value = worksheet.Cells["D6"].Text;
            string F6Value = worksheet.Cells["F6"].Text;
            string E7Value = worksheet.Cells["E7"].Text;

            That(E7Value == "1" && D6Value == "2" && F6Value == "1", Is.True);

            return Task.CompletedTask;
        }
    }
}