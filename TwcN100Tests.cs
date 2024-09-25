using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;
using static NUnit.Framework.Assert;


namespace DomainStorm.Project.TWC.Tests
{
    public class TwcN100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
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
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcN100_01To05()
        {
            await TwcN100_01();
            await TwcN100_02();
            await TwcN100_03();
            await TwcN100_04();
            await TwcN100_05();
        }
        public async Task TwcN100_01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
        }
        public async Task TwcN100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='公告管理']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='公告管理']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("公告管理"));
        }
        public async Task TwcN100_03()
        {
            var addSystemannounceButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增公告')]")));
            _actions.MoveToElement(addSystemannounceButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增公告']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增公告']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("新增公告"));
        }
        public async Task TwcN100_04()
        {
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            stormInputGroupNameInput.SendKeys("公告測試");
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試"));


            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.SendKeys("水籍系統將於12/20進行系統更新，造成不便敬請見諒");
            That(stormTextEditorInput.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));

            stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor h6")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].style.color = 'rgb(230, 0, 0)';", stormTextEditorInput);

            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }
        public async Task TwcN100_05()
        {
            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            That(viewButton.Text, Is.EqualTo("visibility"));

            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            That(editButton.Text, Is.EqualTo("drive_file_rename_outline"));

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3)"));
            That(deleteButton.Text, Is.EqualTo("delete"));
        }

        [Test]
        [Order(1)]
        public async Task TwcN100_06To07()
        { 
            await TwcN100_06();
            await TwcN100_07();
        }
        public async Task TwcN100_06()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '水籍系統將於12/20進行系統更新，造成不便敬請見諒')]")));
            That(content.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));
            That(content.GetAttribute("style"), Is.EqualTo("color: rgb(230, 0, 0);"));
        }
        public async Task TwcN100_07()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), '確定')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }

        [Test]
        [Order(2)]
        public async Task TwcN100_08To09()
        {
            await TwcN100_08();
            await TwcN100_09();
        }
        public async Task TwcN100_08()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='修改公告']"));
                return stormCard != null;
            });
        }
        public async Task TwcN100_09()
        {
            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.Clear();
            await Task.Delay(1000);

            stormTextEditorInput.SendKeys("水籍系統將於12/30進行系統更新，造成不便敬請見諒");
            That(stormTextEditorInput.Text, Is.EqualTo("水籍系統將於12/30進行系統更新，造成不便敬請見諒"));

            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }

        [Test]
        [Order(3)]
        public async Task TwcN100_10To11()
        {
            await TwcN100_10();
            await TwcN100_11();
        }
        public async Task TwcN100_10()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3)"));
            _actions.MoveToElement(deleteButton).Click().Perform();;

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '是否確定刪除？')]")));
            That(hint.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcN100_11()
        {
            var submitButton = _driver.FindElement(By.XPath("//span[contains(text(), '刪除')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }

        [Test]
        [Order(4)]
        public async Task TwcN100_12To13()
        {
            await TwcN100_12();
            await TwcN100_13();
        }
        public async Task TwcN100_12()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='公告管理']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='公告管理']")));
            var systemannounceManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(systemannounceManage.Text, Is.EqualTo("公告管理"));

            await TwcN100_03();
        }
        public async Task TwcN100_13()
        {
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            stormInputGroupNameInput.SendKeys("公告測試2");
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試2"));

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.SendKeys("測試公告自動下架");
            That(stormTextEditorInput.Text, Is.EqualTo("測試公告自動下架"));

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確定')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }

        [Test]
        [Order(5)]
        public async Task TwcN100_14To16()
        {
            await TwcN100_14();
            await TwcN100_15();
            await TwcN100_16();
        }
        public async Task TwcN100_14()
        {
            await TwcN100_12();
        }
        public async Task TwcN100_15()
        {
            var stormInputGroupNameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            stormInputGroupNameInput.SendKeys("公告測試3");
            That(stormInputGroupNameInput.GetAttribute("value"), Is.EqualTo("公告測試3"));

            DateTime currentDateTime = DateTime.Now;

            var expiryDateInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期'] input")));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor div.ql-editor")));
            stormTextEditorInput.SendKeys("測試公告");
            That(stormTextEditorInput.Text, Is.EqualTo("測試公告"));

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確定')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

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
        }
        public async Task TwcN100_16()
        {
            var logout = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//i[text()='logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='登入']")));
        }

        [Test]
        [Order(6)]
        public async Task TwcN100_17To18()
        {
            await TwcN100_17();
            await TwcN100_18();
        }
        public async Task TwcN100_17()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
        }
        public async Task TwcN100_18()
        {
            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='系統公告']"));
                return stormCard != null;
            });

            var systemannouncement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[contains(text(), '測試公告自動下架')]")));
            That(systemannouncement.Text, Is.EqualTo("測試公告自動下架"));
        }
    }
}