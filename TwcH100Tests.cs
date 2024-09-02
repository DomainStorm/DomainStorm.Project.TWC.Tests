using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Reflection;
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
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcH100Tests).GetMethod(testMethod);
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
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體管理']")));

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='媒體管理'] button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體管理'] button")));

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增檔案'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增檔案'] button")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 1 筆，共 1 筆"));
        }
        public async Task TwcH100_01_02()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='媒體管理'] button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體管理'] button")));

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "台水官網圖.png");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(2)");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

            var stormInputGroupDurationInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='播放秒數'] input")));
            stormInputGroupDurationInput.SendKeys("10");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增檔案'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增檔案'] button")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }

        public async Task TwcH100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");
            That(TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo")!.Text, Is.EqualTo("共 0 筆"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("節目單管理"));
        }
        public async Task TwcH100_03()
        {
            var addListButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='節目單管理'] button");
            _actions.MoveToElement(addListButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='節目單管理'] button")));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增節目單']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增節目單"));
        }
        public async Task TwcH100_04()
        {
            var addMediaButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增節目單'] button");
            _actions.MoveToElement(addMediaButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "台水官網圖.png";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = TestHelper.FindAndMoveElement(_driver, "span.rz-button-box");
            _actions.MoveToElement(addButton).Click().Perform();
        }
        public async Task TwcH100_05()
        {
            var name = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var nameInput = name.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("新增測試");

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("新增測試");

            var createButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增節目單'] button[form='create']");
            _actions.MoveToElement(createButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增節目單']")));

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='name'] span")!.Text, Is.EqualTo("新增測試"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='marquee'] span")!.Text, Is.EqualTo("<h6>新增測試</h6>"));
        }
        public async Task TwcH100_06()
        {
            var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var closeButton = TestHelper.FindAndMoveElement(_driver, "div.rz-stack button");
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-carousel")));
        }
        public async Task TwcH100_07()
        {
            var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(3)");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content h5")));
            That(content.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_08()
        {
            var deleteButton = TestHelper.FindAndMoveElement(_driver, "div.rz-stack button");
            _actions.MoveToElement(deleteButton).Click().Perform();

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
            var addMediaButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
            _actions.MoveToElement(addMediaButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell span"));
                return nameCell.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("span.rz-button-box")));
            _actions.MoveToElement(addButton).Click().Perform();
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

            var createButton = TestHelper.FindAndMoveElement(_driver, "button[form='create']");
            _actions.MoveToElement(createButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增節目單']")));

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='name'] span")!.Text, Is.EqualTo("節目單測試"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='marquee'] span")!.Text, Is.EqualTo("<h6>跑馬燈測試</h6>"));
        }
        public async Task TwcH100_13()
        {
            var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var stormCarousel = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-carousel")));
            var videoAutoPlay = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video[autoplay]"));
            That(videoAutoPlay, Is.Not.Null, "影片正在撥放");

            var closeButton = TestHelper.FindAndMoveElement(_driver, "div.rz-stack button");
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-carousel")));
        }
        public async Task TwcH100_14()
        {
            var editButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(2)");
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='修改節目單']")));

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[form='create']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button[form='create']")));
        }
        public async Task TwcH100_15()
        {
            var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(3)");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content h5")));
            That(content.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_16()
        {
            var deleteButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack button:nth-child(2)")));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button:nth-child(2)")));
        }
        public async Task TwcH100_17()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));
        }
        public async Task TwcH100_18()
        {
            var searchButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='節目單審核'] button");
            _actions.MoveToElement(searchButton).Click().Perform();
        }
        public async Task TwcH100_19()
        {
            var auditButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(3)");
            _actions.MoveToElement(auditButton).Click().Perform();

            var approvalButton = TestHelper.FindAndMoveElement(_driver, "div.rz-stack button");
            _actions.MoveToElement(approvalButton).Click().Perform();

            _wait.Until(driver => {
                try
                {
                    var statusElement = TestHelper.WaitStormTableUpload(_driver, "div.table-responsive td[data-field='playListStatus'] span");
                    return statusElement!.Text == "核准";
                }
                catch
                {
                    return false;
                }
            });
            That(TestHelper.WaitStormTableUpload(_driver, "div.table-responsive td[data-field='playListStatus'] span")!.Text, Is.EqualTo("核准"));
        }
    }
}