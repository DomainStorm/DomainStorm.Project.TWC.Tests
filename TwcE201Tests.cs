using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE201Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcE201Tests()
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
        public async Task TwcE201_01To06()
        {
            await TwcE201_01();
            await TwcE201_02();
            await TwcE201_03();
            await TwcE201_04();
            await TwcE201_05();
            await TwcE201_06();
        }
        public async Task TwcE201_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcE201_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcE201_03()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground2.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcE201_04()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");

            var pageInfo = TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));

            var checkAll = TestHelper.WaitStormTableUpload(_driver, "input[aria-label='Check All']");
            _actions.MoveToElement(checkAll).Click().Perform();

            var button = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button")));
            _actions.MoveToElement(button).Click().Perform();

            pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }
        public async Task TwcE201_05()
        {
            var attachmentButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='夾帶附件'] button")));
            _actions.MoveToElement(attachmentButton).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(2)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(2)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案'] button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增檔案'] button")));

            var attachmentName = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));

            _wait.Until(driver =>
            {
                var target = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });
        }
        public async Task TwcE201_06()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button[type='submit']")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var attachFileOneTitle = TestHelper.WaitStormTableUpload(_driver, "tr > td[data-field='attached'] > storm-table-cell i");
            That(attachFileOneTitle!.Text, Is.EqualTo("attach_file"));

            var attachFileTwoTitle = TestHelper.WaitStormTableUpload(_driver, "tr:nth-child(2) > td[data-field='attached'] > storm-table-cell i");
            That(attachFileTwoTitle!.Text, Is.EqualTo("attach_file"));
        }
    }
}