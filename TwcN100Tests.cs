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
        public async Task TwcN100_01To06()
        {
            await TwcN100_01();
            await TwcN100_02();
            await TwcN100_03();
            await TwcN100_04();
            await TwcN100_05();
            await TwcN100_06();
            await TwcN100_07();
            await TwcN100_08();
            await TwcN100_09();
            await TwcN100_10();
            await TwcN100_11();
            await TwcN100_12();
            await TwcN100_13();
            await TwcN100_14();
            await TwcN100_15();
            //await TwcN100_16();
            //await TwcN100_17();
        }
        public async Task TwcN100_01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip a[href='./logout'] i");
            That(logout.Text,Is.EqualTo("logout"));
        }
        public async Task TwcN100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/systemannouncement");
            That(TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo")!.Text, Is.EqualTo("共 0 筆"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='公告管理']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("公告管理"));
        }
        public async Task TwcN100_03()
        {
            var primaryButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(primaryButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='公告管理']")));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增公告']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增公告"));
        }
        public async Task TwcN100_04()
        {
            var name = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var nameInput = name.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("公告測試");

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("水籍系統將於12/20進行系統更新，造成不便敬請見諒");

            editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor h6")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].style.color = 'rgb(230, 0, 0)';", editorInput);

            var createButton = TestHelper.FindAndMoveElement(_driver, "button[form='create']");
            _actions.MoveToElement(createButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增公告']")));

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='name'] span")!.Text, Is.EqualTo("公告測試"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='marquee'] span")!.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));
        }
        public async Task TwcN100_05()
        {
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button")!.Text, Is.EqualTo("visibility"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(2)")!.Text, Is.EqualTo("drive_file_rename_outline"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(3)")!.Text, Is.EqualTo("delete"));
        }
        public async Task TwcN100_06()
        {
            var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog h6")));
            That(content.Text, Is.EqualTo("水籍系統將於12/20進行系統更新，造成不便敬請見諒"));
            That(content.GetAttribute("style"), Is.EqualTo("color: rgb(230, 0, 0);"));
        }
        public async Task TwcN100_07()
        {
            var primaryButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-stack div.rz-stack button")));
            _actions.MoveToElement(primaryButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack div.rz-stack button")));
        }
        public async Task TwcN100_08()
        {
            var editButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(2)");
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='公告管理']")));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='修改公告']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("修改公告"));
        }
        public async Task TwcN100_09()
        {
            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.Clear();

            editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("水籍系統將於12/30進行系統更新，造成不便敬請見諒");

            var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[form='create']")));
            _actions.MoveToElement(createButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='修改公告']")));
        }
        public async Task TwcN100_10()
        {
            var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-button:nth-child(3)");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content h5")));
            That(content.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcN100_11()
        {
            var primaryButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-stack div.rz-stack button")));
            _actions.MoveToElement(primaryButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack div.rz-stack button")));
            _wait.Until(driver => {
                try
                {
                    var pageInfo = TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo");
                    return pageInfo!.Text == "共 0 筆";
                }
                catch
                {
                    return false;
                }
            });
        }
        public async Task TwcN100_12()
        {
            await TwcN100_03();
        }
        public async Task TwcN100_13()
        {
            var name = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
            var nameInput = name.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("公告測試2");

            DateTime currentDateTime = DateTime.Now;

            var releaseDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='上架日期']")));
            var releaseDateInput = releaseDate.GetShadowRoot().FindElement(By.CssSelector("div input"));

            DateTime targetReleaseDate = currentDateTime.AddDays(-1);
            string formattedReleaseDate = targetReleaseDate.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedReleaseDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", releaseDateInput);

            var expiryDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期']")));
            var expiryDateInput = expiryDate.GetShadowRoot().FindElement(By.CssSelector("div input"));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            editorInput.SendKeys("測試公告自動下架");

            var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[form='create']")));
            _actions.MoveToElement(createButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增公告']")));

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='name'] span")!.Text, Is.EqualTo("公告測試2"));
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field ='marquee'] span")!.Text, Is.EqualTo("測試公告自動下架"));
        }
        public async Task TwcN100_14()
        {
            var logoutButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-tooltip a[href='./logout']")));
            _actions.MoveToElement(logoutButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='公告管理']")));
        }
        public async Task TwcN100_15()
        {
            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("0511");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
        }
        public async Task TwcN100_16()
        {
            //var headline = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='系統公告'] h6")));
            //That(headline.Text, Is.EqualTo("測試公告自動下架"));

            var logoutButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-tooltip a[href='./logout']")));
            _actions.MoveToElement(logoutButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='系統公告']")));
        }
        public async Task TwcN100_17()
        {
            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("0511");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var headline = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='系統公告']")));
            var systemannouncement = headline.GetShadowRoot().FindElement(By.CssSelector("slot"));
            That(systemannouncement.Text, Is.EqualTo(""));
        }
    }
}