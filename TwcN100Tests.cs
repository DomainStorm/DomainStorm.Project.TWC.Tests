using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcN100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcN100Tests()
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
        public Task TwcN100_01To05()
        {
            TwcN100_01();
            TwcN100_02();
            TwcN100_03();
            TwcN100_04();
            TwcN100_05();

            return Task.CompletedTask;
        }
        public Task TwcN100_01()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("storm-sidenav"));

            return Task.CompletedTask;
        }
        public Task TwcN100_02()
        {
            _testHelper.NavigateWait("/systemannouncement", By.CssSelector("storm-card[headline='公告管理']"));
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='公告管理']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("公告管理"));

            return Task.CompletedTask;
        }
        public Task TwcN100_03()
        {
            _testHelper.ElementClick(By.XPath("//button[contains(text(), '新增公告')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增公告']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增公告']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("新增公告"));

            return Task.CompletedTask;
        }
        public Task TwcN100_04()
        {
            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='名稱'] input"), "公告測試");
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試"));

            _testHelper.InputSendKeys(By.CssSelector("storm-text-editor div.ql-editor"), "水籍系統將於12/20進行系統更新，造成不便敬請見諒");
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            That(stormTextEditorInput.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));

            stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor h6")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].style.color = 'rgb(230, 0, 0)';", stormTextEditorInput);

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '確定')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count >= 1;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(name.Text, Is.EqualTo("公告測試"));

            var marquee = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='marquee'] span span"));
            That(marquee.Text, Is.EqualTo("<h6 style=\"color: rgb(230, 0, 0);\">水籍系統將於12/20進行系統更新，造成不便敬請見諒</h6>"));

            return Task.CompletedTask;
        }
        public Task TwcN100_05()
        {
            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            That(viewButton.Text, Is.EqualTo("visibility"));

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            That(editButton.Text, Is.EqualTo("drive_file_rename_outline"));

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3)"));
            That(deleteButton.Text, Is.EqualTo("delete"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task TwcN100_06To07()
        {
            TwcN100_06();
            TwcN100_07();

            return Task.CompletedTask;
        }
        public Task TwcN100_06()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/systemannouncement", By.CssSelector("storm-table"));

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '水籍系統將於12/20進行系統更新，造成不便敬請見諒')]")));
            That(content.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));
            That(content.GetAttribute("style"), Is.EqualTo("color: rgb(230, 0, 0);"));

            return Task.CompletedTask;
        }
        public Task TwcN100_07()
        {
            _testHelper.ElementClick(By.XPath("//span[contains(text(), '確定')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count == 1;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(result.Text, Is.EqualTo("公告測試"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcN100_08To09()
        {
            TwcN100_08();
            TwcN100_09();

            return Task.CompletedTask;
        }
        public Task TwcN100_08()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/systemannouncement", By.CssSelector("storm-table"));

            //var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            //var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));

            var editButton = _testHelper.WaitShadowElement("storm-toolbar-item:nth-of-type(2)");
            _actions.MoveToElement(editButton).Click().Perform();



            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='修改公告']"));

            return Task.CompletedTask;
        }
        public Task TwcN100_09()
        {
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.Clear();
            stormTextEditorInput.SendKeys("水籍系統將於12/30進行系統更新，造成不便敬請見諒");
            That(stormTextEditorInput.Text, Is.EqualTo("水籍系統將於12/30進行系統更新，造成不便敬請見諒"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '確定')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count == 1;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var marquee = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='marquee'] span span"));
            That(marquee.Text, Is.EqualTo("<p>水籍系統將於12/30進行系統更新，造成不便敬請見諒</p>"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcN100_10To11()
        {
            TwcN100_10();
            TwcN100_11();

            return Task.CompletedTask;
        }
        public Task TwcN100_10()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/systemannouncement", By.CssSelector("storm-table"));

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3)"));
            _actions.MoveToElement(deleteButton).Click().Perform(); ;

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '是否確定刪除？')]")));
            That(hint.Text, Is.EqualTo("是否確定刪除？"));

            return Task.CompletedTask;
        }
        public Task TwcN100_11()
        {
            _testHelper.ElementClick(By.XPath("//span[contains(text(), '刪除')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count == 1;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
            That(result.Text, Is.EqualTo("沒有找到符合的結果"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcN100_12To13()
        {
            TwcN100_12();
            TwcN100_13();

            return Task.CompletedTask;
        }
        public Task TwcN100_12()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/systemannouncement", By.CssSelector("storm-card[headline='公告管理']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='公告管理']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("公告管理"));

            TwcN100_03();

            return Task.CompletedTask;
        }
        public Task TwcN100_13()
        {
            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='名稱'] input"), "公告測試2");
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試2"));

            _testHelper.InputSendKeys(By.CssSelector("storm-text-editor div.ql-editor"), "測試公告自動下架");
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            That(stormTextEditorInput.Text, Is.EqualTo("測試公告自動下架"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '確定')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count >= 1;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(name.Text, Is.EqualTo("公告測試2"));

            var marquee = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='marquee'] span span"));
            That(marquee.Text, Is.EqualTo("<h6>測試公告自動下架</h6>"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(5)]
        public Task TwcN100_14To16()
        {
            TwcN100_14();
            TwcN100_15();
            TwcN100_16();

            return Task.CompletedTask;
        }
        public Task TwcN100_14()
        {
            TwcN100_12();

            return Task.CompletedTask;
        }
        public Task TwcN100_15()
        {
            _testHelper.InputSendKeys(By.CssSelector("storm-input-group[label='名稱'] input"), "公告測試3");
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試3"));

            DateTime currentDateTime = DateTime.Now;

            var expiryDateInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期'] input")));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            _testHelper.InputSendKeys(By.CssSelector("storm-text-editor div.ql-editor"), "測試公告");
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            That(stormTextEditorInput.Text, Is.EqualTo("測試公告"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '確定')]"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/systemannouncement"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count >= 1;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

            var systemannouncementName = rows.Any(row =>
            {
                var systemannouncementName = row.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return systemannouncementName.Text == "公告測試3";
            });

            var systemannouncementMarquee = rows.Any(row =>
            {
                var systemannouncementMarquee = row.FindElement(By.CssSelector("td[data-field='marquee'] span span"));
                return systemannouncementMarquee.Text == "<h6>測試公告</h6>";
            });

            return Task.CompletedTask;
        }
        public Task TwcN100_16()
        {
            _testHelper.ElementClick(By.XPath("//i[text()='logout']"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='登入']"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(6)]
        public Task TwcN100_17To18()
        {
            TwcN100_17();
            TwcN100_18();

            return Task.CompletedTask;
        }
        public Task TwcN100_17()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("storm-sidenav"));

            return Task.CompletedTask;
        }
        public Task TwcN100_18()
        {
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='系統公告']"));

            var systemannouncement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '測試公告自動下架')]")));
            That(systemannouncement.Text, Is.EqualTo("測試公告自動下架"));

            return Task.CompletedTask;
        }
    }
}