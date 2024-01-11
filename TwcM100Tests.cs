using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcM100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcM100Tests()
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
        public async Task TwcM100_01To06()
        {
            await TwcM100_01();
            await TwcM100_02();
            await TwcM100_03();
            await TwcM100_04();
            await TwcM100_05();
            await TwcM100_06();
        }
        public async Task TwcM100_01()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體庫']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("媒體庫"));
        }
        public async Task TwcM100_02()
        {
            var addTextButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='媒體庫'] button:nth-child(2)");
            _actions.MoveToElement(addTextButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體庫'] button")));

            var stormInputGroupName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var stormInputGroupNameInput = stormInputGroupName.GetShadowRoot().FindElement(By.CssSelector("div input"));
            stormInputGroupNameInput.SendKeys("宣導文字");

            var stormInputGroupDesc = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='說明']")));
            var stormInputGroupDescInput = stormInputGroupDesc.GetShadowRoot().FindElement(By.CssSelector("div input"));
            stormInputGroupDescInput.SendKeys("宣導說明文字");

            var stormInputGroupDuration = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='播放秒數']")));
            var stormInputGroupDurationInput = stormInputGroupDuration.GetShadowRoot().FindElement(By.CssSelector("div input"));
            stormInputGroupDurationInput.SendKeys("10");

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.SendKeys("跑馬燈內容");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增文字'] button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增文字'] button[type='submit']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "td[data-field='name'] span")!.Text, Is.EqualTo("宣導文字"));
        }
        public async Task TwcM100_03()
        {
            var viewButton = TestHelper.WaitStormEditTableUpload(_driver, "storm-button > storm-tooltip > div > button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var viewText = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container h6")));
            That(viewText.Text, Is.EqualTo("跑馬燈內容"));

            var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-cancel");
            _actions.MoveToElement(cancelButton).Click().Perform();
        }
        public async Task TwcM100_04()
        {
            var editButton = TestHelper.WaitStormEditTableUpload(_driver, "storm-button:nth-child(2) > storm-tooltip > div > button");
            _actions.MoveToElement(editButton).Click().Perform();

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor >div.ql-container > div.ql-editor")));
            stormTextEditorInput.Clear();

            stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor >div.ql-container > div.ql-editor")));
            stormTextEditorInput.SendKeys("應該是宣導的內容文字");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();
        }
        public async Task TwcM100_05()
        {
            var viewButton = TestHelper.WaitStormEditTableUpload(_driver, "storm-button > storm-tooltip > div > button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var viewText = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > p")));
            That(viewText.Text, Is.EqualTo("應該是宣導的內容文字"));

            var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-cancel");
            _actions.MoveToElement(cancelButton).Click().Perform();
        }
        public async Task TwcM100_06()
        {
            var deleteButton = TestHelper.WaitStormEditTableUpload(_driver, "storm-button:nth-child(3) > storm-tooltip > div > button");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var deleteConfirm = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-confirm");
            _actions.MoveToElement(deleteConfirm).Click().Perform();

           _wait.Until(driver => {
                try
                {
                    var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
                    return pageInfo!.Text == "共 0 筆";
                }
                catch
                {
                    return false;
                }
            });
        }

        [Test]
        [Order(1)]
        public async Task TwcM100_07To11()
        {
            await TwcM100_07();
            await TwcM100_08();
            await TwcM100_09();
            await TwcM100_10();
            await TwcM100_11();
        }
        public async Task TwcM100_07()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > div > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "台水官網圖.png");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();

            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("台水官網圖.png"));
        }
        public async Task TwcM100_08()
        {
            var tbody = TestHelper.WaitStormEditTableUpload(_driver, "tbody");
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell span"));
                return nameCell.Text == "台水官網圖.png";
            });

            var viewButton = selectedRows!.FindElement(By.CssSelector("td.action > storm-table-cell > storm-table-toolbar > storm-button > storm-tooltip > div > button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var viewImg = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > img")));
            That(viewImg, Is.Not.Null);

            var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-cancel");
            _actions.MoveToElement(cancelButton).Click().Perform();
        }
        public async Task TwcM100_09()
        {
            var editButton = TestHelper.WaitStormEditTableUpload(_driver, "storm-button:nth-child(2) > storm-tooltip > div > button");
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-button:nth-child(2) > storm-tooltip > div > button")));

            var stormInputGroupDesc = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='說明']")));
            var stormInputGroupDescInput = stormInputGroupDesc.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            stormInputGroupDescInput.SendKeys("描述圖示說明");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();

            That(TestHelper.WaitStormEditTableUpload(_driver, "td[data-field='description'] > storm-table-cell span")!.Text, Is.EqualTo("描述圖示說明"));
        }
        public async Task TwcM100_10()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > div > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(2)");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();

            That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }
        public async Task TwcM100_11()
        {
            var tbody = TestHelper.WaitStormEditTableUpload(_driver, "tbody");
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell span"));
                return nameCell.Text == "testmedia.mp4";
            });

            var viewButton = selectedRows!.FindElement(By.CssSelector("td.action > storm-table-cell > storm-table-toolbar > storm-button > storm-tooltip > div > button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var viewVideo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > video")));
            That(viewVideo, Is.Not.Null);

            var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-cancel");
            _actions.MoveToElement(cancelButton).Click().Perform();
        }
    }
}