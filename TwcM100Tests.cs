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
        private TestHelper _testHelper = null!;
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
            _testHelper = new TestHelper(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public Task TwcM100_01To06()
        {
            TwcM100_01();
            TwcM100_02();
            TwcM100_03();
            TwcM100_04();
            TwcM100_05();
            TwcM100_06();

            return Task.CompletedTask;
        }
        public Task TwcM100_01()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);
            _testHelper.NavigateWait("/multimedia", By.CssSelector("storm-card[headline='媒體管理']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體管理']")));
            var mediaManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(mediaManage.Text, Is.EqualTo("媒體管理"));

            return Task.CompletedTask;
        }
        public Task TwcM100_02()
        {
            _testHelper.ElementClick(By.XPath("//button[contains(text(), '新增文字')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增文字']"));

            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='名稱'] input"), "宣導文字");
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("宣導文字"));

            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='說明'] input"), "宣導說明文字");
            var stormInputGroupDescInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='說明'] input")));
            That(stormInputGroupDescInput.GetAttribute("value"), Is.EqualTo("宣導說明文字"));

            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='播放秒數'] input"), "10");
            var stormInputGroupDurationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='播放秒數'] input")));
            That(stormInputGroupDurationInput.GetAttribute("value"), Is.EqualTo("10"));

            _testHelper.InputSendKeys(By.CssSelector("storm-text-editor div.ql-editor"), "跑馬燈內容");
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            That(stormTextEditorInput.Text, Is.EqualTo("跑馬燈內容"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '新增')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var textName = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span"));
            That(textName.Text, Is.EqualTo("宣導文字"));

            return Task.CompletedTask;
        }
        public Task TwcM100_03()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var container = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '跑馬燈內容')]")));
            That(container.Text, Is.EqualTo("跑馬燈內容"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '關閉')]"));

            return Task.CompletedTask;
        }
        public Task TwcM100_04()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(2000);

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='修改']"));

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor >div.ql-container > div.ql-editor")));
            stormTextEditorInput.Clear();
            stormTextEditorInput.SendKeys("應該是宣導的內容文字");

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '更新')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            return Task.CompletedTask;
        }
        public Task TwcM100_05()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(1000);

            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var container = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//p[contains(text(), '應該是宣導的內容文字')]")));
            That(container.Text, Is.EqualTo("應該是宣導的內容文字"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '關閉')]"));

            return Task.CompletedTask;
        }
        public Task TwcM100_06()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(1000);

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var confirmButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//h2[contains(text(), '是否刪除？')]")));
            That(confirmButton.Text, Is.EqualTo("是否刪除？"));
            Thread.Sleep(1000);

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '刪除')]"));

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
                return textElement.Text == "沒有找到符合的結果" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("沒有找到符合的結果"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task TwcM100_07To09()
        {
            TwcM100_07();
            TwcM100_08();
            TwcM100_09();

            return Task.CompletedTask;
        }
        public Task TwcM100_07()
        {
            TwcM100_01();

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '新增檔案')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "台水官網圖.png" }, "input.dz-hidden-input");
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("span span"));
                return textElement.Text == "台水官網圖.png" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("台水官網圖.png"));

            return Task.CompletedTask;
        }
        public Task TwcM100_08()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(1000);

            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();
            Thread.Sleep(1000);

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '關閉')]"));

            return Task.CompletedTask;
        }
        public Task TwcM100_09()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(1000);

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='修改']"));
            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='說明'] input"), "描述圖示說明");

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '更新')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("span span"));
                return textElement.Text == "台水官網圖.png" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("台水官網圖.png"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcM100_10To11()
        {
            TwcM100_10();
            TwcM100_11();

            return Task.CompletedTask;
        }
        public Task TwcM100_10()
        {
            TwcM100_01();

            _testHelper.ElementClick(By.XPath("//button[text()='新增檔案']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "testmedia.mp4" }, "input.dz-hidden-input");
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var textElement = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return textElement.Text == "testmedia.mp4";
            });

            var textElement = selectedRow!.FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(textElement!.Text, Is.EqualTo("testmedia.mp4"));

            return Task.CompletedTask;
        }
        public Task TwcM100_11()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var textElement = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return textElement.Text == "testmedia.mp4";
            });

            var stormToolbar = selectedRow!.FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            Thread.Sleep(1000);

            var viewButton = stormToolbar!.FindElement(By.CssSelector("storm-toolbar-item button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var videoElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("video")));
            That(videoElement, Is.Not.Null);

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '關閉')]"));

            return Task.CompletedTask;
        }
    }
}