using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE201Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcE201Tests()
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
        public async Task TwcE201_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcE201_02() // 呼叫bmTransferApply/confirmbground，沒錯誤則取得http 200回應

        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcE201_03() // 呼叫bmTransferApply/confirmbground，沒錯誤則取得http 200回應
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirmbground", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E201_bmTransferApply_bground2.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(3)]
        public async Task TwcE201_04() // 帳號0511進入批次過戶區，點選【+申請者證件】按鈕後右邊畫面直接顯示夾帶附件畫面。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            var card = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = card.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var searchInput = stormTable.GetShadowRoot().FindElement(By.Id("search"));
            searchInput.SendKeys(TestHelper.ApplyCaseNo!);

            var CheckAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
            CheckAll.Click();
            Thread.Sleep(500);

            var stormToolbar = stormDocumentListDetail.FindElement(By.CssSelector("storm-toolbar"));
            var stormButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-button button"));
            That(stormButton, Is.Not.Null, "不存在");
        }

        [Test]
        [Order(4)]
        public async Task TwcE201_05() // 看到夾帶附件資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            Actions actions = new(driver);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            
            var searchInput = stormTable.GetShadowRoot().FindElement(By.Id("search"));
            searchInput.SendKeys(TestHelper.ApplyCaseNo!);

            var CheckAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
            CheckAll.Click();
            Thread.Sleep(500);

            var stormToolbar = stormDocumentListDetail.FindElement(By.CssSelector("storm-toolbar"));
            var stormButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-button button"));
            stormButton.Click();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            上傳.Click();

            stormCard = stormDocumentListDetail.FindElement(By.CssSelector("storm-card"));
            var stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            var fileName1 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(1) > td[data-field='name']"));
            var spanName1 = fileName1.FindElement(By.CssSelector("span"));
            string spanText1 = spanName1.Text;

            var fileName2 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(2) > td[data-field='name']"));
            var spanName2 = fileName2.FindElement(By.CssSelector("span"));
            string spanText2 = spanName2.Text;

            That(spanText1, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
            That(spanText2, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }

        [Test]
        [Order(5)]
        public async Task TwcE201_06() // 該申請案件於批次過戶清單中之附件欄位有迴紋針圖示即表示該筆資料已完成夾帶檔動作。有夾帶附件之資料等待後續排程資料寫入結案日期後於批次過戶資料夾中消失。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/batch");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            Actions actions = new(driver);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var searchInput = stormTable.GetShadowRoot().FindElement(By.Id("search"));
            searchInput.SendKeys(TestHelper.ApplyCaseNo!);

            var CheckAll = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[aria-label='Check All']"));
            CheckAll.Click();
            Thread.Sleep(500);

            var stormToolbar = stormDocumentListDetail.FindElement(By.CssSelector("storm-toolbar"));
            var stormButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-button button"));
            stormButton.Click();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            上傳.Click();

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormCard = stormDocumentListDetail.FindElement(By.CssSelector("storm-card"));
            stormCard = stormCard.FindElement(By.CssSelector("storm-card"));
            var 確認夾帶 = stormCard.FindElement(By.CssSelector("button.btn.bg-gradient-info"));

            actions.MoveToElement(確認夾帶).Click().Perform();

            var attached = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='attached']"));
            var icon = attached.FindElement(By.CssSelector("i"));
            string attach_file = icon.Text;

            That(attach_file, Is.EqualTo("attach_file"));
        }
    }
}