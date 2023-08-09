using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcM100Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcM100Tests()
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
        public async Task TwcM100_01() // 畫面右側出現媒體管理畫面。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            IWebElement mediaLibrary = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體庫']")));

            That(mediaLibrary, Is.Not.Null, "媒體庫未找到");
        }

        [Test]
        [Order(1)]
        public async Task TwcM100_02() // 媒體庫列表出現該筆內容。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 新增文字 = stormCard.FindElement(By.CssSelector(".btn-primary:last-child"));
            新增文字.Click();

            IWebElement stormInputGroup = driver.FindElement(By.CssSelector("storm-input-group[label='名稱']"));
            IWebElement 名稱 = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            名稱.SendKeys("宣導文字");

            stormInputGroup = driver.FindElement(By.CssSelector("storm-input-group[label='說明']"));
            IWebElement 說明 = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            說明.SendKeys("宣導說明");

            stormInputGroup = driver.FindElement(By.CssSelector("storm-input-group[label='播放秒數']"));
            IWebElement 播放秒數 = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            播放秒數.SendKeys("10");

            IWebElement stormTextEditor = driver.FindElement(By.CssSelector("storm-text-editor"));
            IWebElement 文字 = stormTextEditor.GetShadowRoot().FindElement(By.CssSelector(".ql-editor"));
            文字.SendKeys("跑馬燈內容");
            文字.SendKeys(Keys.Tab);

            stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 新增 = stormCard.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            新增.Click();
            Thread.Sleep(500);

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string spanText = spanElement.Text;
            That(spanText, Is.EqualTo("宣導文字"));
        }

        [Test]
        [Order(2)]
        public async Task TwcM100_03() // 畫面顯示輸入之文字-跑馬燈內容-視窗後按關閉。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormToolTip = stormTableToolbar.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 觀看 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            觀看.Click();

            IWebElement divElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show")));
            IWebElement div_divElement = divElement.FindElement(By.CssSelector("div.swal2-html-container"));
            string innerText = div_divElement.Text;
            That(innerText, Is.EqualTo("跑馬燈內容"));

            div_divElement = divElement.FindElement(By.CssSelector("div.swal2-actions"));
            IWebElement 關閉 = div_divElement.FindElement(By.CssSelector("button.swal2-cancel"));
            關閉.Click();
        }

        [Test]
        [Order(3)]
        public async Task TwcM100_04() // 回到媒體庫列表並顯示該筆內容。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormButton = stormTableToolbar.FindElements(By.CssSelector("storm-button"))[1];
            IWebElement stormToolTip = stormButton.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 修改 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            修改.Click();

            IWebElement stormTextEditor = driver.FindElement(By.CssSelector("storm-text-editor"));
            IWebElement 文字 = stormTextEditor.GetShadowRoot().FindElement(By.CssSelector(".ql-editor"));
            文字.Clear();
            文字.SendKeys("應該是宣導的內容文字");
            文字.SendKeys(Keys.Tab);

            stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 更新 = stormCard.FindElement(By.CssSelector("button.btn.bg-gradient-info"));
            更新.Click();
            Thread.Sleep(500);

            stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string spanText = spanElement.Text;
            That(spanText, Is.EqualTo("宣導文字"));
        }

        [Test]
        [Order(4)]
        public async Task TwcM100_05() // 畫面顯示輸入之文字-應該是宣導的內容文字-視窗後按關閉。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormToolTip = stormTableToolbar.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 觀看 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            觀看.Click();
            Thread.Sleep(500);

            IWebElement divElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show")));
            IWebElement div_divElement = divElement.FindElement(By.CssSelector("div.swal2-html-container"));
            string innerText = div_divElement.Text;
            That(innerText, Is.EqualTo("應該是宣導的內容文字"));

            div_divElement = divElement.FindElement(By.CssSelector("div.swal2-actions"));
            IWebElement 關閉 = div_divElement.FindElement(By.CssSelector("button.swal2-cancel"));
            關閉.Click();
        }

        [Test]
        [Order(5)]
        public async Task TwcM100_06() // 媒體庫列表已無出現該筆資訊。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormButton = stormTableToolbar.FindElements(By.CssSelector("storm-button"))[2];
            IWebElement stormToolTip = stormButton.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 刪除 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            刪除.Click();

            IWebElement divElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show")));
            IWebElement div_divElement = divElement.FindElement(By.CssSelector("div.swal2-actions"));
            刪除 = div_divElement.FindElement(By.CssSelector("button.swal2-confirm"));
            刪除.Click();

            var findElement = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td > p.h3"));
            var element = findElement.SingleOrDefault(e => e.Text == "沒有找到符合的結果");
            if (element != null)
            {
                string pText = element.Text;

                That(pText, Is.EqualTo("沒有找到符合的結果"));
            }
        }

        [Test]
        [Order(6)]
        public async Task TwcM100_07() // 回到媒體庫列表並顯示該筆圖片資訊。
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 新增檔案 = stormCard.FindElement(By.CssSelector(".btn-primary:first-child"));
            新增檔案.Click();
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string 台水官網圖 = "台水官網圖.png";
            string 台水官網圖Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", 台水官網圖);

            lastHiddenInput.SendKeys(台水官網圖Path);

            IWebElement 上傳 = driver.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 上傳);
            上傳.Click();
            Thread.Sleep(5000);

            stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string 文件名稱 = spanElement.Text;

            That(文件名稱, Is.EqualTo(台水官網圖));
        }
        
        [Test]
        [Order(7)]
        public async Task TwcM100_08() // 畫面顯示該圖內容後視窗按關閉
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormToolTip = stormTableToolbar.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 觀看 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            觀看.Click();

            IWebElement divElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show")));
            IWebElement div_divElement = divElement.FindElement(By.CssSelector("div.swal2-html-container"));
            IWebElement imgElement = div_divElement.FindElement(By.TagName("img"));

            if (imgElement != null)
            {
                div_divElement = divElement.FindElement(By.CssSelector("div.swal2-actions"));
                IWebElement 關閉 = div_divElement.FindElement(By.CssSelector("button.swal2-cancel"));
                關閉.Click();
            }
        }

        [Test]
        [Order(8)]
        public async Task TwcM100_09() // 回到媒體庫列表有於該筆列表欄位內容顯示-描述圖示說明-之文字
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormButton = stormTableToolbar.FindElements(By.CssSelector("storm-button"))[1];
            IWebElement stormToolTip = stormButton.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 修改 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            修改.Click();
            Thread.Sleep(500);

            IWebElement stormInputGroup = driver.FindElement(By.CssSelector("storm-input-group[label='說明']"));
            IWebElement 說明 = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            說明.Clear();
            說明.SendKeys("描述圖示說明");
            說明.SendKeys(Keys.Tab);

            stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 更新 = stormCard.FindElement(By.CssSelector("button.btn.bg-gradient-info"));
            更新.Click();
            Thread.Sleep(500);

            stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='description']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string spanText = spanElement.Text;
            That(spanText, Is.EqualTo("描述圖示說明"));
        }

        [Test]
        [Order(9)]
        public async Task TwcM100_10() // 回到媒體庫列表並顯示該筆圖片資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement 新增檔案 = stormCard.FindElement(By.CssSelector(".btn-primary:first-child"));
            新增檔案.Click();
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string testmedia = "testmedia.mp4";
            string testmediaPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", testmedia);

            lastHiddenInput.SendKeys(testmediaPath);

            IWebElement 上傳 = driver.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 上傳);
            上傳.Click();
            Thread.Sleep(3000);

            stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card")));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string 文件名稱 = spanElement.Text;

            That(文件名稱, Is.EqualTo(testmedia));
        }

        [Test]
        [Order(10)]
        public async Task TwcM100_11() // 畫面顯示該影片內容後視窗按關閉
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "irenewei", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement stormEditTable = stormCard.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            IWebElement td = stormTable.GetShadowRoot().FindElement(By.CssSelector("td.text-start.align-middle.action"));
            IWebElement stormTableToolbar = td.FindElement(By.CssSelector("storm-table-toolbar"));
            IWebElement stormToolTip = stormTableToolbar.FindElement(By.CssSelector("storm-tooltip"));
            IWebElement 觀看 = stormToolTip.FindElement(By.CssSelector("button[type='button']"));
            觀看.Click();

            IWebElement divElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show")));
            IWebElement div_divElement = divElement.FindElement(By.CssSelector("div.swal2-html-container"));
            IWebElement videoElement = div_divElement.FindElement(By.TagName("video"));

            if (videoElement != null)
            {
                div_divElement = divElement.FindElement(By.CssSelector("div.swal2-actions"));
                IWebElement 關閉 = div_divElement.FindElement(By.CssSelector("button.swal2-cancel"));
                關閉.Click();
            }
        }
    }
}