using OfficeOpenXml.Export.HtmlExport.Accessibility;
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

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體管理']")));
            var mediaManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(mediaManage.Text, Is.EqualTo("媒體管理"));
        }
        public async Task TwcM100_02()
        {
            var addTextButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文字')]")));
            _actions.MoveToElement(addTextButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增文字']"));
                return stormCard != null;
            });

            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            stormInputGroupNameInput.SendKeys("宣導文字");
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("宣導文字"));

            var stormInputGroupDescInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='說明'] input")));
            stormInputGroupDescInput.SendKeys("宣導說明文字");
            That(stormInputGroupDescInput.GetAttribute("value"), Is.EqualTo("宣導說明文字"));

            var stormInputGroupDurationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='播放秒數'] input")));
            stormInputGroupDurationInput.SendKeys("10");
            That(stormInputGroupDurationInput.GetAttribute("value"), Is.EqualTo("10"));

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.SendKeys("跑馬燈內容");
            That(stormTextEditorInput.Text, Is.EqualTo("跑馬燈內容"));

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '新增')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var textName = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span"));
            That(textName.Text, Is.EqualTo("宣導文字"));
        }
        public async Task TwcM100_03()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var container = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '跑馬燈內容')]")));
            That(container.Text, Is.EqualTo("跑馬燈內容"));

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '關閉')]")));
            _actions.MoveToElement(closeButton).Click().Perform();
        }
        public async Task TwcM100_04()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            await Task.Delay(1000);

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='修改']"));
                return stormCard != null;
            });

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor >div.ql-container > div.ql-editor")));
            stormTextEditorInput.Clear();
            stormTextEditorInput.SendKeys("應該是宣導的內容文字");

            var updateButton = _driver.FindElement(By.XPath("//button[contains(text(), '更新')]"));
            _actions.MoveToElement(updateButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });
        }
        public async Task TwcM100_05()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            await Task.Delay(1000);

            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var container = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//p[contains(text(), '應該是宣導的內容文字')]")));
            That(container.Text, Is.EqualTo("應該是宣導的內容文字"));

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '關閉')]")));
            _actions.MoveToElement(closeButton).Click().Perform();
        }
        public async Task TwcM100_06()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            await Task.Delay(1000);

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var confirmButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//h2[contains(text(), '是否刪除？')]")));
            That(confirmButton.Text, Is.EqualTo("是否刪除？"));
            await Task.Delay(1000);

            var confirmDeleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '刪除')]")));
            _actions.MoveToElement(confirmDeleteButton).Click().Perform();

            var element = _wait.Until(driver => 
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
                return textElement.Text == "沒有找到符合的結果" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("沒有找到符合的結果"));
        }

        [Test]
        [Order(1)]
        public async Task TwcM100_07To09()
        {
            await TwcM100_07();
            await TwcM100_08();
            await TwcM100_09();
        }
        public async Task TwcM100_07()
        {
            await TwcM100_01();

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增檔案')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));
                return stormCard != null;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "台水官網圖.png");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

            var stormInputGroupDurationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='播放秒數'] input")));
            stormInputGroupDurationInput.SendKeys("10");

            var uploadButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("span span"));
                return textElement.Text == "台水官網圖.png" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("台水官網圖.png"));
        }
        public async Task TwcM100_08()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            await Task.Delay(1000);

            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();
            await Task.Delay(1000);

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '關閉')]")));
            _actions.MoveToElement(closeButton).Click().Perform();
        }
        public async Task TwcM100_09()
        {
            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var stormToolbar = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar"));
            _actions.MoveToElement(stormToolbar).Perform();
            await Task.Delay(1000);

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='修改']"));
                return stormCard != null;
            });

            var stormInputGroupDescInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='說明'] input")));
            stormInputGroupDescInput.SendKeys("描述圖示說明");

            var updateButton = _driver.FindElement(By.XPath("//button[contains(text(), '更新')]"));
            _actions.MoveToElement(updateButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("span span"));
                return textElement.Text == "台水官網圖.png" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("台水官網圖.png"));
        }

        [Test]
        [Order(2)]
        public async Task TwcM100_10To11()
        {
            await TwcM100_10();
            await TwcM100_11();
        }
        public async Task TwcM100_10()
        {
            await TwcM100_01();

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增檔案')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));
                return stormCard != null;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var uploadButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

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
        }
        public async Task TwcM100_11()
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
            await Task.Delay(1000);

            var viewButton = stormToolbar!.FindElement(By.CssSelector("storm-toolbar-item button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var videoElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("video")));
            That(videoElement, Is.Not.Null);

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '關閉')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            //var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            //var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            //var stormToolbar = element!.FindElement(By.CssSelector("storm-toolbar"));
            //_actions.MoveToElement(stormToolbar).Perform();
            //await Task.Delay(1000);

            //var viewButton = element!.FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            //_actions.MoveToElement(viewButton).Click().Perform();
            //var tbody = TestHelper.WaitStormEditTableUpload(_driver, "tbody");
            //var trList = tbody!.FindElements(By.CssSelector("tr"));
            //var selectedRows = trList.FirstOrDefault(tr =>
            //{
            //    var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] > storm-table-cell span"));
            //    return nameCell.Text == "testmedia.mp4";
            //});

            //var viewButton = selectedRows!.FindElement(By.CssSelector("storm-toolbar-item > storm-button > button"));
            //_actions.MoveToElement(viewButton).Click().Perform();

            //var viewVideo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > video")));
            //That(viewVideo, Is.Not.Null);

            //var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-cancel");
            //_actions.MoveToElement(cancelButton).Click().Perform();
        }
    }
}