using AngleSharp.Dom;
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
    public class TwcD101Tests
    {
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        private List<ChromeDriver> _chromeDriverList;
        public TwcD101Tests()
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
            option.AddUserProfilePreference("download.default_directory", _downloadDirectory);
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
        public async Task TwcD101_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcD101_02() // 呼叫bmAolishedApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmAolishedApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-D101_bmAolishedApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcD101_03() // 看到表單內容並於表單受理欄位中看到有■中結
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 中結 = driver.FindElement(By.Id("中結"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 中結);

            That(中結.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(3)]
        public async Task TwcD101_04() // 顯示臨櫃人員核章職稱姓名等資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));

            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(4)]
        public async Task TwcD101_05() // 看到■用印或代送件只需夾帶附件已打勾
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement 受理登記 = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver);
            actions.MoveToElement(受理登記).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcD101_06() // 系統跳出【尚未夾帶附件】訊息
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement 受理登記 = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver);
            actions.MoveToElement(受理登記).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            IWebElement divElement = driver.FindElement(By.Id("swal2-html-container"));
            IWebElement h5Element = divElement.FindElement(By.TagName("h5"));
            string 提示訊息 = h5Element.Text;

            That(提示訊息, Is.EqualTo("尚未夾帶附件"));
        }

        [Test]
        [Order(6)]
        public async Task TwcD101_07() // 看到申請之表單內容跳至夾帶附件區塊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var findElement = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td > p.h3"));
            var element = findElement.SingleOrDefault(e => e.Text == "沒有找到符合的結果");
            if (element != null)
            {
                string filename = element.Text;

                That(filename, Is.EqualTo("沒有找到符合的結果"));
            }
        }

        [Test]
        [Order(7)]
        public async Task TwcD101_08() // 看到檔案上傳
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            IWebElement 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 新增文件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 新增文件);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            IWebElement stormInputGroup = driver.FindElement(By.CssSelector("body storm-main-content main div div div div storm-card form storm-input-group"));
            string value = stormInputGroup.GetAttribute("value");

            That(value, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(8)]
        public async Task TwcD101_09() // 看到夾帶附件視窗顯示有一筆附件清單資料，檔名為twcweb_01_1_夾帶附件1.pdf
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            IWebElement 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 新增文件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 新增文件);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(9)]
        public async Task TwcD101_10() // 確認完成畫面進入未結案件中
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));

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
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 新增文件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 新增文件);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement 受理登記 = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(受理登記).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);

            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(10)]
        public async Task TwcD101_11() // 看到夾帶附件區塊顯示該檔案。已勾選■已詳閱貴公司消費性用水服務契約、公司個人資料保護法、貴公司營業章程
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeFirst = stormTreeNode.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement stormTreeNodeSecondUnderSecond = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeThird = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];

            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            IWebElement linkElement = driver.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string download = linkElement.GetAttribute("download");

            That(download, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            IWebElement 消費性用水服務契約 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            actions.MoveToElement(消費性用水服務契約).Click().Perform();

            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);

            wait.Until(driver =>
            {
                IWebElement 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
                return 消費性用水服務契約.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement 公司個人資料保護告知事項 = stormTreeNodeSecondUnderSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

            actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);

            wait.Until(driver =>
            {
                IWebElement 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
                return 公司個人資料保護告知事項.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement 公司營業章程 = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            actions.MoveToElement(公司營業章程).Click().Perform();

            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);

            wait.Until(driver =>
            {
                IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
                return 公司營業章程.GetAttribute("checked") == "true";
            });

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(11)]
        public async Task TwcD101_12() // 查詢出來該件，列在下方
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            IWebElement monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            Thread.Sleep(750);
            selectMonth.SelectByText("June");

            IWebElement spanElement = driver.FindElement(By.CssSelector("span[aria-label='June 1, 2023']"));
            spanElement.Click();

            IWebElement divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            spanElement = element.FindElement(By.CssSelector("span"));
            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(12)]
        public async Task TwcD101_13() // 確認附件有一件twcweb_01_1_夾帶附件1.pdf
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            IWebElement monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            Thread.Sleep(750);
            selectMonth.SelectByText("June");

            IWebElement spanElement = driver.FindElement(By.CssSelector("span[aria-label='June 1, 2023']"));
            spanElement.Click();

            IWebElement divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            actions.MoveToElement(element).Click().Perform();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(夾帶附件).Click().Perform();

            IWebElement linkElement = driver.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string downloadValue = linkElement.GetAttribute("download");

            That(downloadValue, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(13)]
        public async Task TwcD101_14() // PDF檔產製成功自動下載於下載區
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            IWebElement stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            IWebElement stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            IWebElement monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            Thread.Sleep(750);
            selectMonth.SelectByText("June");

            IWebElement spanElement = driver.FindElement(By.CssSelector("span[aria-label='June 1, 2023']"));
            spanElement.Click();

            IWebElement divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            actions.MoveToElement(element).Click().Perform();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(夾帶附件).Click().Perform();

            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            IWebElement 下載PDF = driver.FindElement(By.CssSelector("button.btn.bg-gradient-warning.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 下載PDF);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 下載PDF);

            string filePath = Path.Combine(_downloadDirectory, "41104433664.pdf");

            That(Directory.Exists(_downloadDirectory), Is.True);

            Console.WriteLine($"-----{_downloadDirectory} GetFiles-----");

            foreach (var fn in Directory.GetFiles(_downloadDirectory))
            {
                Console.WriteLine($"-----filename: {fn}-----");
            }

            Console.WriteLine($"-----{_downloadDirectory} GetFiles end-----");

            Console.WriteLine($"-----檢查檔案完整路徑: {filePath}-----");

            wait.Until(webDriver => File.Exists(filePath));

            That(File.Exists(filePath), Is.True);
        }
    }
}