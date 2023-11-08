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
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcE101_01To06()
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

            That(TestHelper.WaitStormTableUpload, Is.Not.Null);
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var checkAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
            _actions.MoveToElement(checkAll).Click().Perform();

            var stormToolbar = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-toolbar")));
            var button = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("button"));
            _actions.MoveToElement(button).Click().Perform();

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcE201_05()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > storm-card > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(2)")));
            var firstFile = "twcweb_01_1_夾帶附件1.pdf";
            var firstFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", firstFile);
            var secondFile = "twcweb_01_1_夾帶附件2.pdf";
            var secondFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", secondFile);
            lastHiddenInput.SendKeys(firstFilePath);
            _wait.Until(_ =>
            {
                try
                {
                    lastHiddenInput.SendKeys(secondFilePath);
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(2)")));
                    return false;
                }
            });

            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            var spanOne = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr > td > storm-table-cell > span"));
            var spanTwo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(2) > td > storm-table-cell > span"));
            That(spanOne.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
            That(spanTwo.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public async Task TwcE201_06()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > storm-card > div.position-absolute > button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var attachFileOneTitle = TestHelper.WaitStormTableUpload(_driver, "tr > td[data-field='attached'] > storm-table-cell > span > i");
            var attachFileTwoTitle = TestHelper.WaitStormTableUpload(_driver, "tr:nth-child(2) > td[data-field='attached'] > storm-table-cell > span > i");
            That(attachFileOneTitle!.Text, Is.EqualTo("attach_file"));
            That(attachFileTwoTitle!.Text, Is.EqualTo("attach_file"));
        }
    }
}