using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS101Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcS101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            _chromeDriverList = new List<ChromeDriver>();

            return Task.CompletedTask;
        }

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            foreach (ChromeDriver driver in _chromeDriverList)
            {
                driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        public async Task TwcS101_01() // 15次皆無錯誤。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);

            for (int i = 0; i < 15; i++)
            {
                TestHelper.AccessToken = await TestHelper.GetAccessToken();

                HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

                driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
                TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

                driver.SwitchTo().Frame(0);

                var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

                driver.SwitchTo().DefaultContent();

                var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
                var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
                var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
                var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
                var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

                Actions actions = new(driver);
                actions.MoveToElement(夾帶附件).Click().Perform();

                var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
                actions.MoveToElement(新增文件).Perform();
                新增文件.Click();

                wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

                IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
                var lastHiddenInput = hiddenInputs[^1];

                string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

                lastHiddenInput.SendKeys(filePath);

                var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
                actions.MoveToElement(上傳).Perform();
                上傳.Click();

                var stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
                var stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

                var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
                var spanElement = element.FindElement(By.CssSelector("span"));
                wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

                var 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
                actions.MoveToElement(用印或代送件只需夾帶附件).Perform();
                用印或代送件只需夾帶附件.Click();

                var 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
                actions.MoveToElement(確認受理).Perform();
                確認受理.Click();

                string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
                wait.Until(ExpectedConditions.UrlContains(targetUrl));
            }
            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormtable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormtable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS101_02() // 查詢後資料清單列表顯示有10筆，畫面下方顯示有第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 6, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(2)]
        public async Task TwcS101_03() // 顯示清單畫面切換為5筆，下方顯示第 11 至 5 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 4, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 11 至 5 筆，共 15 筆"));
        }

        [Test]
        [Order(3)]
        public async Task TwcS101_04() // 顯示清單畫面切換至第一頁10筆，下方顯示第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 5, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}