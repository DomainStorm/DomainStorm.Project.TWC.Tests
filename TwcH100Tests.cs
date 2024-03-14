using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcH100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcH100Tests()
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
        public async Task TwcH100_01To19()
        {
            await TwcH100_01();
            await TwcH100_02();
            await TwcH100_03();
            await TwcH100_04();
            await TwcH100_05();
            await TwcH100_06();
            await TwcH100_07();
            await TwcH100_08();
            await TwcH100_09();
            await TwcH100_10();
            await TwcH100_11();
            await TwcH100_12();
            await TwcH100_13();
            await TwcH100_14();
            await TwcH100_15();
            await TwcH100_16();
            await TwcH100_17();
            await TwcH100_18();
            await TwcH100_19();
        }
        public async Task TwcH100_01()
        {
            await TwcH100_01_01();
            await TwcH100_01_02();
        }
        public async Task TwcH100_01_01()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            var addFileButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='媒體管理'] button");
            addFileButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體管理'] button")));

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("testmedia.mp4"));
        }
        public async Task TwcH100_01_02()
        {
            var addFileButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='媒體管理'] button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體管理'] button")));

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "台水官網圖.png");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(2)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

            var stormInputGroupDuration = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='播放秒數']")));
            var stormInputGroupDurationInput = stormInputGroupDuration.GetShadowRoot().FindElement(By.CssSelector("div input"));
            stormInputGroupDurationInput.SendKeys("10");

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "台水官網圖.png";
            });
        }

        public async Task TwcH100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='節目單管理']");
            That(stormCard.Text, Is.EqualTo("節目單管理"));
        }
        public async Task TwcH100_03()
        {
            var addListButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='節目單管理'] button");
            addListButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='節目單管理'] button")));

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增節目單']");
            That(stormCard.Text, Is.EqualTo("新增節目單"));
        }
        public async Task TwcH100_04()
        {
            var addMediaButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增節目單'] button");
            addMediaButton!.Click();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "台水官網圖.png";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            addButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));
        }
        public async Task TwcH100_05()
        {
            var name = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var nameInput = name.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("新增測試");

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("新增測試");

            var createButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增節目單'] button[form='create']");
            createButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增節目單']")));

            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field ='name'] span").Text, Is.EqualTo("新增測試"));
            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field ='marquee'] span").Text, Is.EqualTo("<h6>新增測試</h6>"));
        }
        public async Task TwcH100_06()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var viewButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var closeButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            closeButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-carousel")));
        }
        public async Task TwcH100_07()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var deleteButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(3)"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var content = TestHelper.FindAndMoveToElement(_driver, "div.rz-dialog-content h5");
            That(content!.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_08()
        {
            var deleteButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            deleteButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));
        }
        public async Task TwcH100_09()
        {
            await TwcH100_03();
        }
        public async Task TwcH100_10()
        {
            await TwcH100_04();
        }
        public async Task TwcH100_11()
        {
            var addMediaButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增節目單'] button");
            addMediaButton!.Click();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell span"));
                return nameCell.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            addButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));
        }
        public async Task TwcH100_12()
        {
            var name = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var nameInput = name.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("節目單測試");

            DateTime currentDateTime = DateTime.Now;

            var releaseDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='上架日期']")));
            var releaseDateInput = releaseDate.GetShadowRoot().FindElement(By.CssSelector("div input"));

            string formattedReleaseDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedReleaseDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", releaseDateInput);

            var expiryDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期']")));
            var expiryDateInput = expiryDate.GetShadowRoot().FindElement(By.CssSelector("div input"));

            DateTime targetExpiryDate = currentDateTime.AddDays(1);
            string formattedExpiryDate = targetExpiryDate.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);


            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("跑馬燈測試");

            var createButton = TestHelper.FindAndMoveToElement(_driver, "button[form='create']");
            createButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增節目單']")));

            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field ='name'] span").Text, Is.EqualTo("節目單測試"));
            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field ='marquee'] span").Text, Is.EqualTo("<h6>跑馬燈測試</h6>"));
        }
        public async Task TwcH100_13()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var viewButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var stormCarousel = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-carousel")));
            var videoAutoPlay = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video[autoplay]"));
            That(videoAutoPlay, Is.Not.Null, "影片正在撥放");

            var closeButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            closeButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-carousel")));
        }
        public async Task TwcH100_14()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var editButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(2)"));
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='修改節目單']")));

            var submitButton = TestHelper.FindAndMoveToElement(_driver, "button[form='create']");
            submitButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button[form='create']")));
        }
        public async Task TwcH100_15()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var deleteButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(3)"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var content = TestHelper.FindAndMoveToElement(_driver, "div.rz-dialog-content h5");
            That(content!.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_16()
        {
            var deleteButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button:nth-child(2)");
            deleteButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button:nth-child(2)")));
        }
        public async Task TwcH100_17()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));
        }
        public async Task TwcH100_18()
        {
            var searchButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='節目單審核'] button");
            searchButton!.Click();
        }
        public async Task TwcH100_19()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var auditButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(3)"));
            _actions.MoveToElement(auditButton).Click().Perform();

            var approvalButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            approvalButton!.Click();

            _wait.Until(driver => {
                    var statusElement = TestHelper.WaitStormTableUpload(_driver, "div.table-responsive td[data-field='playListStatus'] span");
                    return statusElement!.Text == "核准";
            });
            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='playListStatus'] span")!.Text, Is.EqualTo("核准"));
        }
    }
}