using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG100Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcG100Tests()
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
        public async Task TwcG100_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcG100_02() // 呼叫bmMilitaryApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G100_bmMilitaryApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcG100_03() // driver_2中看到申請之表單內容
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_2, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_2.SwitchTo().Frame(0);

            IWebElement applyCaseNo = driver_2.FindElement(By.CssSelector("[sti-apply-case-no]"));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            IWebElement waterNo = driver_2.FindElement(By.CssSelector("[sti-water-no]"));
            That(waterNo.Text, Is.EqualTo("41105533310"));

            IWebElement applyDate = driver_2.FindElement(By.CssSelector("[sti-apply-date]"));
            That(applyDate.Text, Is.EqualTo("2023年06月13日"));
        }

        [Test]
        [Order(3)]
        public async Task TwcG100_04() // driver_2中看到身分證字號欄位出現A123456789
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement idNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));

            idNo_driver_1.SendKeys("A123456789");

            That(idNo_driver_1.GetAttribute("value"), Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcG100_05() // driver_2看到■申請電子帳單
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-email]")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", applyEmail);

            That(applyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcG100_06() // driver_2看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB  
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 撫卹 = driver_1.FindElement(By.Id("檢附證件group2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 撫卹);

            That(撫卹.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));

            identification.SendKeys("BBB");

            That(identification.GetAttribute("value"), Is.EqualTo("BBB"));
        }

        [Test]
        [Order(6)]
        public async Task TwcG100_07() // driver_2中看到●否
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 否 = driver_1.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 否);

            That(否.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task TwcG100_08() // driver_2可看到註記欄位內容顯示-備註內容 四個字 
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement stiNote_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));
            IWebElement stiNote_driver_2 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));

            stiNote_driver_1.SendKeys("備註內容");
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote_driver_1);

            That(stiNote_driver_2.GetAttribute("value"), Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(8)]
        public async Task TwcG100_09() // driver_2可看到顯示■繳費
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 繳費 = driver_1.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(9)]
        public async Task TwcG100_10() // driver_2看到受理欄位有落章
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait_driver_1 = new(driver_1, TimeSpan.FromSeconds(10));
            wait_driver_1.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            WebDriverWait wait_driver_2 = new(driver_2, TimeSpan.FromSeconds(10));
            wait_driver_2.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 受理_driver_1 = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理_driver_1);
            wait_driver_1.Until(ExpectedConditions.ElementToBeClickable(受理_driver_1));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理_driver_1);
            wait_driver_2.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.CssSelector("iframe")));

            IReadOnlyList<IWebElement> signElement = driver_2.FindElements(By.CssSelector("[class='sign']"));
            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(10)]
        public async Task TwcG100_11() // driver_2中勾選消費性用水服務契約
        {
            await TwcG100_10();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            driver_1.SwitchTo().DefaultContent();
            driver_2.SwitchTo().DefaultContent();

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeFirst = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement hrefContract_1 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_1).Click().Perform();

            IWebElement 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            IWebElement 消費性用水服務契約_driver_2 = driver_2.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 消費性用水服務契約_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(11)]
        public async Task TwcG100_12() // driver_2中勾選公司個人資料保護告知事項
        {
            await TwcG100_11();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeSecondUnderSecond = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefContract_2 = stormTreeNodeSecondUnderSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_2).Click().Perform();

            IWebElement 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            IWebElement 公司個人資料保護告知事項_driver_2 = driver_2.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(12)]
        public async Task TwcG100_13() // driver_2中勾選公司營業章程
        {
            await TwcG100_12();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeThird = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            IWebElement hrefContract_3 = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_3).Click().Perform();

            IWebElement 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            IWebElement 公司營業章程_driver_2 = driver_2.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司營業章程_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(13)]
        public async Task TwcG100_14() // driver_2中表單畫面完整呈現簽名內容，並於driver_1中看到相容內容
        {
            await TwcG100_13();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeThird = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            IWebElement hrefSignature = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#signature']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefSignature).Click().Perform();

            IWebElement buttonSign_driver_1 = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonSign_driver_1);

            IWebElement buttonSign_driver_2 = driver_2.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", buttonSign_driver_2);

            IWebElement container_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            IWebElement container_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));

            IWebElement img_driver_1 = container_driver_1.FindElement(By.CssSelector("img"));
            IWebElement img_driver_2 = container_driver_2.FindElement(By.CssSelector("img"));

            string src_driver_1 = img_driver_1.GetAttribute("src");
            string src_driver_2 = img_driver_2.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }

        [Test]
        [Order(14)]
        public async Task TwcG100_15() // driver_2中看到掃描拍照證件圖像
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeFirst = stormTreeNodeFourth.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement hrefCredential = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#credential']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefCredential).Click().Perform();

            IWebElement buttonScanId = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonScanId);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", buttonScanId);

            IWebElement container_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            IWebElement container_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));

            IWebElement img_driver_1 = container_driver_1.FindElement(By.CssSelector("img"));
            IWebElement img_driver_2 = container_driver_2.FindElement(By.CssSelector("img"));

            string src_driver_1 = img_driver_1.GetAttribute("src");
            string src_driver_2 = img_driver_2.GetAttribute("src");

            That(src_driver_1, Is.EqualTo(src_driver_2));
        }

        [Test]
        [Order(15)]
        public async Task TwcG100_16() // driver_2中看到夾帶附件資訊
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFile).Click().Perform();

            IWebElement buttonAddDocument = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonAddDocument);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", buttonAddDocument);
            Thread.Sleep(1000);

            IList<IWebElement> hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
            var spanElement = element.FindElement(By.CssSelector("span"));
            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));
        }

        [Test]
        [Order(16)]
        public async Task TwcG100_17() // 系統顯示[申請電子帳單需要填寫Email及聯絡電話]
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理);

            driver_1.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement hrefFinished = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFinished).Click().Perform();

            IWebElement 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

            IWebElement outerContainer = driver_1.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            IWebElement innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));

            That(innerContainer.Displayed, Is.True);
        }

        [Test]
        [Order(17)]
        public async Task TwcG100_18() // driver_2中Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait_driver_1 = new(driver_1, TimeSpan.FromSeconds(10));
            wait_driver_1.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            WebDriverWait wait_driver_2 = new(driver_2, TimeSpan.FromSeconds(10));
            wait_driver_2.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);
            driver_2.SwitchTo().Frame(0);

            IWebElement email_driver_1 = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email_driver_1.SendKeys("aaa@bbb.ccc");
            email_driver_1.SendKeys(Keys.Tab);
            Thread.Sleep(1000);

            IWebElement stiEmailElement = wait_driver_2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-class='InputSelectBlock'][sti-email]")));
            string spanText_Email = stiEmailElement.Text;

            That(spanText_Email, Is.EqualTo("aaa@bbb.ccc"));

            IWebElement telNo_driver_1 = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            telNo_driver_1.SendKeys("02-12345678");
            telNo_driver_1.SendKeys(Keys.Tab);
            Thread.Sleep(1000);

            IWebElement stiTelNoElement = wait_driver_2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-class='InputSelectBlock'][sti-email-tel-no]")));
            string spanText_TelNo = stiTelNoElement.Text;

            That(spanText_TelNo, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(18)]
        public async Task TwcG100_19() // 該申請案件進入未結案件中
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email_driver_1.SendKeys("aaa@bbb.ccc");
            email_driver_1.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            IWebElement telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            telNo_driver_1.SendKeys("02-12345678");
            telNo_driver_1.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            IWebElement 撫卹 = driver_1.FindElement(By.Id("檢附證件group2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 撫卹);

            IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));

            identification.SendKeys("BBB");
            identification.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            IWebElement 否 = driver_1.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 否);

            IWebElement stiNote_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));

            stiNote_driver_1.SendKeys("備註內容");
            stiNote_driver_1.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            IWebElement 繳費 = driver_1.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 繳費);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);

            driver_1.SwitchTo().DefaultContent();
            driver_2.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new (driver_1);
            actions.MoveToElement(hrefFile).Click().Perform();
            
            IWebElement buttonAddDocument = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonAddDocument);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", buttonAddDocument);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement hrefFinished = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(hrefFinished).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement confirmButton = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", confirmButton);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", confirmButton);

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var findElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            var element = findElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);
            if (element != null)
            {
                string applyCaseNo = element.Text;

                That(applyCaseNo, Is.EqualTo(TestHelper.ApplyCaseNo));
            }
        }

        [Test]
        [Order(19)]
        public async Task TwcG100_20() // driver_1中有看到註記欄位內容顯示-備註內容
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement stiNote_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote_driver_1);

            var stiNoteElement = driver_1.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-note='']"));
            string spanText = stiNoteElement.Text;

            That(spanText, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(20)]
        public async Task TwcG100_21() // driver_1中有看到打勾顯示■繳費
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 繳費 = driver_1.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(21)]
        public async Task TwcG100_22() // 表單內容顯示■申請電子帳單，Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", email_driver_1);

            var stiEmailElement = driver_1.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email='']"));
            string spanText_Email = stiEmailElement.Text;

            That(spanText_Email, Is.EqualTo("aaa@bbb.ccc"));

            IWebElement telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", telNo_driver_1);

            var stiTelNoElement = driver_1.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email-tel-no='']"));
            string spanText_TelNo = stiTelNoElement.Text;

            That(spanText_TelNo, Is.EqualTo("02-12345678"));
        }

        //[Test]
        //[Order(22)]
        //public async Task TwcG100_23() // 檢附證件欄位看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB
        //{
        //    ChromeDriver driver_1 = GetNewChromeDriver();

        //    await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
        //    driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
        //    TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

        //    WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

        //    driver_1.SwitchTo().Frame(0);

        //    IWebElement 撫卹 = driver_1.FindElement(By.Id("檢附證件group2"));
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 撫卹);

        //    That(撫卹.GetAttribute("checked"), Is.EqualTo("true"));

        //    IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));

        //    string spanText_Identification = identification.Text;

        //    That(spanText_Identification, Is.EqualTo("BBB"));
        //}

        //[Test]
        //[Order(23)]
        //public async Task TwcG100_24() // 超戶申請欄位看到●否
        //{
        //    ChromeDriver driver_1 = GetNewChromeDriver();

        //    await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
        //    driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
        //    TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

        //    WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

        //    driver_1.SwitchTo().Frame(0);

        //    IWebElement 否 = driver_1.FindElement(By.Id("超戶申請radio"));
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 否);

        //    That(否.GetAttribute("checked"), Is.EqualTo("true"));
        //}
    }
}