using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
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
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcE201Tests).GetMethod(testMethod);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public async Task TwcE201_01To03()
        {
            await TwcE201_01();
            await TwcE201_02();
            await TwcE201_03();
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

        [Test]
        [Order(1)]
        public async Task TwcE201_04To06()
        {
            await TwcE201_04();
            await TwcE201_05();
            await TwcE201_06();
        }
        public async Task TwcE201_04()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");

            _wait.Until(_ =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                return stormTable != null;
            });

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var firstWaterNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(1) td[data-field='waterNo'] span"));
            That(firstWaterNo.Text, Is.EqualTo("41101220338"));

            var secondWaterNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(2) td[data-field='waterNo'] span"));
            That(secondWaterNo.Text, Is.EqualTo("41101220339"));

            var checkAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
            _actions.MoveToElement(checkAll).Click().Perform();

            var applicantIdButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '申請者證件')]")));
            _actions.MoveToElement(applicantIdButton).Click().Perform();
        }
        public async Task TwcE201_05()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormcard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案']")));
                return stormcard != null;
            });

            var stormcard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案']")));
            var headline = stormcard.GetShadowRoot().FindElement(By.CssSelector("h5"));

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(2)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(2)");

            _wait.Until(driver =>
            {
                var input = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='名稱'] input")));
                return input.GetAttribute("value").Contains("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf");
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));

            var upload = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '上傳')]")));

            _wait.Until(_ =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                return stormEditTable != null;
            });

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var firstFileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(1) storm-table-cell span"));
            That(firstFileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var secondFileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(2) storm-table-cell span"));
            That(secondFileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public async Task TwcE201_06()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '確認夾帶')]")));
            _actions.MoveToElement(submitButton).Click().Perform();
            Thread.Sleep(1000);

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var firstFileSpan = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(1) storm-table-cell i"));
            That(firstFileSpan.Text, Is.EqualTo("attach_file"));

            var secondFileSpan = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-of-type(2) storm-table-cell i"));
            That(secondFileSpan.Text, Is.EqualTo("attach_file"));
        }
    }
}