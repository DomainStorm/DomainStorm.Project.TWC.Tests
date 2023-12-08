//using OpenQA.Selenium;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
//using System.Net;
//using static NUnit.Framework.Assert;

//namespace DomainStorm.Project.TWC.Tests
//{
//    public class TwcE201Tests
//    {
//        private IWebDriver _driver = null!;
//        private WebDriverWait _wait = null!;
//        private Actions _actions = null!;
//        public TwcE201Tests()
//        {
//            TestHelper.CleanDb();
//        }

//        [SetUp]
//        public void Setup()
//        {
//            _driver = TestHelper.GetNewChromeDriver();
//            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
//            _actions = new Actions(_driver);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _driver.Quit();
//        }

//        [Test]
//        [Order(0)]
//        public async Task TwcE101_01To06()
//        {
//            await TwcE201_01();
//            await TwcE201_02();
//            await TwcE201_03();
//            await TwcE201_04();
//            await TwcE201_05();
//            await TwcE201_06();
//        }
//        public async Task TwcE201_01()
//        {
//            TestHelper.AccessToken = await TestHelper.GetAccessToken();
//            That(TestHelper.AccessToken, Is.Not.Empty);
//        }
//        public async Task TwcE201_02()
//        {
//            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground.json"));
//            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
//        }
//        public async Task TwcE201_03()
//        {
//            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground2.json"));
//            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
//        }
//        public async Task TwcE201_04()
//        {
//            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");
//            That(TestHelper.WaitStormTableUpload, Is.Not.Null);

//            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
//            var checkAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
//            _actions.MoveToElement(checkAll).Click().Perform();

//            var stormToolbar = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-toolbar")));
//            var button = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("button"));
//            _actions.MoveToElement(button).Click().Perform();

//            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
//            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
//        }
//        public async Task TwcE201_05()
//        {
//            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > storm-card > div.float-end > button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > storm-card > div.float-end > button")));
//            _actions.MoveToElement(addFileButton).Click().Perform();

//            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
//            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(2)");

//            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
//            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(2)");

//            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
//            That(fileName.GetAttribute("value"), Is.Not.Null);

//            var uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
//            _actions.MoveToElement(uploadButton).Click().Perform();

//            _wait.Until(driver =>
//            {
//                var target = TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo");
//                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
//            });
//        }
//        public async Task TwcE201_06()
//        {
//            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card > storm-card > div.position-absolute > button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > storm-card > div.position-absolute > button")));
//            _actions.MoveToElement(submitButton).Click().Perform();

//            var attachFileOneTitle = TestHelper.WaitStormTableUpload(_driver, "tr > td[data-field='attached'] > storm-table-cell > span > i");
//            That(attachFileOneTitle!.Text, Is.EqualTo("attach_file"));

//            var attachFileTwoTitle = TestHelper.WaitStormTableUpload(_driver, "tr:nth-child(2) > td[data-field='attached'] > storm-table-cell > span > i");
//            That(attachFileTwoTitle!.Text, Is.EqualTo("attach_file"));
//        }
//    }
//}