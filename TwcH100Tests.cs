//using OfficeOpenXml.Export.HtmlExport.Accessibility;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
//using static NUnit.Framework.Assert;

//namespace DomainStorm.Project.TWC.Tests
//{
//    public class TwcH100Tests
//    {
//        private IWebDriver _driver = null!;
//        private WebDriverWait _wait = null!;
//        private Actions _actions = null!;
//        public TwcH100Tests()
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
//        public async Task TwcH100_01()
//        {
//            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

//            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > div > button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > div > button")));
//            _actions.MoveToElement(addFileButton).Click().Perform();

//            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input")));
//            var 台水官網圖 = "台水官網圖.png";
//            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", 台水官網圖);
//            lastHiddenInput.SendKeys(filePath);

//            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
//            That(fileName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

//            var uploadButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button[type='submit']")));
//            _actions.MoveToElement(uploadButton).Click().Perform();
//            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("台水官網圖.png"));

//            addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card > div > button")));
//            _actions.MoveToElement(addFileButton).Click().Perform();

//            lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(2)")));
//            var testmedia = "testmedia.mp4";
//            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", testmedia);
//            lastHiddenInput.SendKeys(filePath);

//            fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
//            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

//            uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button[type='submit']")));
//            _actions.MoveToElement(uploadButton).Click().Perform();
//            That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
//        }

//        [Test]
//        [Order(1)]
//        public async Task TwcH100_02To13()
//        {
//            await TwcH100_02();
//            await TwcH100_03();
//            await TwcH100_04();
//            await TwcH100_05();
//            await TwcH100_06();
//            await TwcH100_07();
//            await TwcH100_08();
//            await TwcH100_09();
//            await TwcH100_10();
//            await TwcH100_11();
//            await TwcH100_12();
//            await TwcH100_13();
//        }

//        public async Task TwcH100_02()
//        {
//            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

//            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
//            That(logout, Is.Not.Null, "登入失敗");
//        }
//        public async Task TwcH100_03()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("節目單管理"));
//        }
//        public async Task TwcH100_04()
//        {
//            var addListButton = TestHelper.FindAndMoveElement(_driver, "button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
//            _actions.MoveToElement(addListButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增節目單']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("新增節目單"));
//        }
//        public async Task TwcH100_05()
//        {
//            var addMediaButton = TestHelper.FindAndMoveElement(_driver, "button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
//            _actions.MoveToElement(addMediaButton).Click().Perform();

//            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
//            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
//            var trList = tbody!.FindElements(By.CssSelector("tr"));
//            var selectedRows = trList.FirstOrDefault(tr =>
//            {
//                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell > span"));
//                return nameCell.Text == "testmedia.mp4";
//            });
//            _actions.MoveToElement(selectedRows).Click().Perform();

//            var addButton = TestHelper.FindAndMoveElement(_driver, "span.rz-button-box");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("span.rz-button-box")));
//            _actions.MoveToElement(addButton).Click().Perform();
//        }
//        public async Task TwcH100_06()
//        {
//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group")));
//            var stormInputGroupInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            stormInputGroupInput.SendKeys("節目單測試");

//            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor > div.ql-container > div.ql-editor")));
//            stormTextEditorInput.SendKeys("跑馬燈測試");

//            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.bg-gradient-info");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.bg-gradient-info")));
//            _actions.MoveToElement(submitButton).Click().Perform();

//            var stormTabelName = TestHelper.WaitStormTableUpload(_driver, "td[data-field='name'] > storm-table-cell > span");
//            That(stormTabelName!.Text, Is.EqualTo("節目單測試"));

//            var stormTableMarquee = TestHelper.WaitStormTableUpload(_driver, "td[data-field='marquee'] > storm-table-cell > span > h6");
//            That(stormTableMarquee!.Text, Is.EqualTo("跑馬燈測試"));
//        }
//        public async Task TwcH100_07()
//        {
//            var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_9'] > storm-table-cell > storm-table-toolbar > storm-button");
//            _actions.MoveToElement(viewButton).Click().Perform();

//            var stormCarousel = TestHelper.FindAndMoveElement(_driver, "storm-carousel");
//            var video = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video > source"));
//            That(video.GetAttribute("type"), Is.EqualTo("video/mp4"));

//            var closeButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper button");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-wrapper button")));
//            _actions.MoveToElement(closeButton).Click().Perform();
//        }
//        public async Task TwcH100_08()
//        {
//            var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_9'] > storm-table-cell > storm-table-toolbar > storm-button:nth-child(2)");
//            _actions.MoveToElement(viewButton).Click().Perform();
//        }
//        public async Task TwcH100_09()
//        {
//        }
//        public async Task TwcH100_10()
//        {
//        }
//        public async Task TwcH100_11()
//        {
//        }
//        public async Task TwcH100_12()
//        {
//        }
//        public async Task TwcH100_13()
//        {
//        }
//    }
//}