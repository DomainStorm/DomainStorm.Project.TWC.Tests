using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS101Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcS101Tests()
        {
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            _chromeDriverList = new List<ChromeDriver>();

            return Task.CompletedTask;
        }
        private ChromeDriver GetNewChromeDriver()
        {
            var option = new ChromeOptions();
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-gpu");
            option.AddArgument("--enable-javascript");
            option.AddArgument("--allow-running-insecure-content");
            option.AddArgument("--ignore-urlfetcher-cert-requests");
            option.AddArgument("--disable-web-security");
            option.AddArgument("--ignore-certificate-errors");
            //option.AddArguments("--no-sandbox");

            if (TestHelper.GetChromeConfig().Headless)
                option.AddArgument("--headless");

            new DriverManager().SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig());
            var driver = new ChromeDriver(option);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            _chromeDriverList.Add(driver);

            return driver;
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
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);

            for (int i =0 ; i < 15 ; i++)
            {
                TestHelper.AccessToken = await TestHelper.GetAccessToken();

                HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

                driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
                TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

                driver.SwitchTo().Frame(0);

                IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

                driver.SwitchTo().DefaultContent();

                IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
                IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
                IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
                IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
                IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

                Actions actions = new(driver);
                actions.MoveToElement(夾帶附件).Click().Perform();

                IWebElement 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
                actions.MoveToElement(新增文件).Perform();
                新增文件.Click();
                Thread.Sleep(500);

                IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
                IWebElement lastHiddenInput = hiddenInputs[^1];

                string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

                lastHiddenInput.SendKeys(filePath);

                IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
                actions.MoveToElement(上傳).Perform();
                上傳.Click();

                IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
                IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
                IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

                IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
                IWebElement spanElement = element.FindElement(By.CssSelector("span"));
                wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

                IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
                actions.MoveToElement(用印或代送件只需夾帶附件).Perform();
                用印或代送件只需夾帶附件.Click();

                IWebElement 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
                actions.MoveToElement(確認受理).Perform();
                確認受理.Click();

                string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
                wait.Until(ExpectedConditions.UrlContains(targetUrl));
            }
            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormtable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            IWebElement pageInfo = stormtable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS101_02() // 查詢後資料清單列表顯示有10筆，畫面下方顯示有第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            IWebElement monthDropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("flatpickr-monthDropdown-months")));
            SelectElement selectMonth = new (monthDropdown);
            selectMonth.SelectByValue("2");

            IWebElement spanElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[aria-label='March 6, 2023']")));
            spanElement.Click();

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            
            查詢.Click();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            IWebElement pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            That(pageInfoText,Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
        
        [Test]
        [Order(2)]
        public async Task TwcS101_03() // 顯示清單畫面切換為5筆，下方顯示第 11 至 5 筆，共 15 筆。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            IWebElement monthDropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("flatpickr-monthDropdown-months")));
            SelectElement selectMonth = new(monthDropdown);
            selectMonth.SelectByValue("2");

            IWebElement spanElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[aria-label='March 6, 2023']")));
            spanElement.Click();

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            查詢.Click();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            IWebElement stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            IWebElement span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));
            Actions actions = new(driver);
            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            IWebElement pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            That(pageInfoText, Is.EqualTo("顯示第 11 至 5 筆，共 15 筆"));
        }

        [Test]
        [Order(3)]
        public async Task TwcS101_04() // 顯示清單畫面切換至第一頁10筆，下方顯示第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            IWebElement monthDropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("flatpickr-monthDropdown-months")));
            SelectElement selectMonth = new(monthDropdown);
            selectMonth.SelectByValue("2");

            IWebElement spanElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[aria-label='March 6, 2023']")));
            spanElement.Click();

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            查詢.Click();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            IWebElement stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            IWebElement span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));
            Actions actions = new(driver);
            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            IWebElement pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}