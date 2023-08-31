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
    public class TwcE100Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcE100Tests()
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
        public async Task TwcE100_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcE100_02() // 呼叫bmTransferApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E100_bmTransferApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcE100_03() // driver_2中看到申請之表單內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiApplyCaseNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]"))); ;
            string 受理編號 = stiApplyCaseNo.Text;
            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));

            var stiWaterNo = driver.FindElement(By.CssSelector("[sti-water-no]"));
            string 水號 = stiWaterNo.Text;
            That(水號, Is.EqualTo("41881288338"));

            var stiApplyDate = driver.FindElement(By.CssSelector("[sti-apply-date]"));
            string 申請日期 = stiApplyDate.Text;
            That(申請日期, Is.EqualTo("2023年06月03日"));
        }

        [Test]
        [Order(3)]
        public async Task TwcE100_04() // driver_2中看到身分證字號欄位出現A123456789
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiTrusteeIdNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiTrusteeIdNo);
            var stiTrusteeIdNoInput = stiTrusteeIdNo.FindElement(By.TagName("input"));

            stiTrusteeIdNoInput.SendKeys("A123456789");
            stiTrusteeIdNoInput.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiTrusteeIdNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiTrusteeIdNo);

            string 身分證號碼 = stiTrusteeIdNo.Text;

            That(身分證號碼, Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcE100_05() // 看到■申請電子帳單 
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);
            申請電子帳單勾選.SendKeys(Keys.Tab);

            Thread.Sleep(500);
            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));

            //lambda一行可省略{}，{}內也省略return
            wait.Until(driver =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成";
            });

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選value = driver.FindElement(By.Id("申請電子帳單勾選value"));
            string 申請電子帳單 = 申請電子帳單勾選value.GetAttribute("textContent");

            That(申請電子帳單, Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcE100_06() // 看到註記欄位內容顯示-備註內容 四個字
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            var stiNoteInput = stiNote.FindElement(By.TagName("Input"));

            stiNoteInput.SendKeys("備註內容");
            stiNoteInput.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("註記")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);

            string 註記 = stiNote.Text;

            That(註記, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(6)]
        public async Task TwcE100_07() // 看到顯示■中結
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var 中結 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("中結")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 中結);

            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            中結 = driver.FindElement(By.Id("中結"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);

            That(中結.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task TwcE100_08() // 看到受理欄位有落章
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(driver =>
            {
                try
                {
                    IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
            IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));
            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(8)]
        public async Task TwcE100_09() // driver_2中勾選消費性用水服務契約
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            Actions actions = new(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Second = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var stormTreeNode = stormTreeNode_Second.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            var 消費性用水服務契約 = stormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            actions.MoveToElement(消費性用水服務契約).Click().Perform();

            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            wait.Until(driver =>
            {
                var 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
                return 消費性用水服務契約.GetAttribute("checked") == "true";
            });
            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(9)]
        public async Task TwcE100_10() // driver_2中勾選公司個人資料保護告知事項
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            Actions actions = new(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Second = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var stormTreeNode_Second_Second = stormTreeNode_Second.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var 公司個人資料保護告知事項 = stormTreeNode_Second_Second.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

            actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            wait.Until(driver =>
            {
                var 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
                return 公司個人資料保護告知事項.GetAttribute("checked") == "true";
            });
            公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(10)]
        public async Task TwcE100_11() // driver_2中勾選公司營業章程
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            Actions actions = new(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Second = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var stormTreeNode_Third = stormTreeNode_Second.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            var 公司營業章程 = stormTreeNode_Third.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            actions.MoveToElement(公司營業章程).Click();


            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            wait.Until(driver =>
            {
                var 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
                return 公司營業章程.GetAttribute("checked") == "true";
            });
            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(11)]
        public async Task TwcE100_12() // driver_2中表單畫面完整呈現簽名內容，並於driver_1中看到相容內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            var 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            var 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            var 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            var 簽名 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 簽名);

            string src_driver_2 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            var imgElement = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container img")));
            wait.Until(driver =>
            {
                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", imgElement);
                    string src = imgElement.GetAttribute("src");
                    return src != null;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });

            string src_driver_1 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            That(src_driver_1, Is.EqualTo(src_driver_2));
        }

        [Test]
        [Order(12)]
        public async Task TwcE100_13() // driver_2中看到掃描拍照證件圖像
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            Actions actions = new(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Fourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var stormTreeNode = stormTreeNode_Fourth.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            var 掃描拍照 = stormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#credential']"));

            actions.MoveToElement(掃描拍照).Click().Perform();

            var 啟動掃描證件 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            actions.MoveToElement(啟動掃描證件).Perform();
            啟動掃描證件.Click();

            var imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_1 = imgElement.GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_2 = imgElement.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }

        [Test]
        [Order(13)]
        public async Task TwcE100_14() // driver_2中看到夾帶附件資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Fourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var stormTreeNode_Second = stormTreeNode_Fourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var 夾帶附件 = stormTreeNode_Second.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();
            Thread.Sleep(500);

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
            actions.MoveToElement(上傳).Perform();
            上傳.Click();

            var stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stormTable);

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
        [Order(14)]
        public async Task TwcE100_15() // 系統顯示[申請電子帳單需要填寫Email及聯絡電話]
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Fourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var stormTreeNode_Second = stormTreeNode_Fourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var 夾帶附件 = stormTreeNode_Second.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 新增文件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 新增文件);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            var 受理登記 = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            var 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            var outerContainer = driver.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            var innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));
            string hintText = innerContainer.Text; 

            That(hintText,Is.Not.Null);
        }

        [Test]
        [Order(15)]
        public async Task TwcE100_16() // driver_2中Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);
            Thread.Sleep(500);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            var stiEmailInput = stiEmail.FindElement(By.TagName("Input"));
            stiEmailInput.SendKeys("aaa@bbb.ccc");
            stiEmailInput.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            string Emial = stiEmail.Text;

            That(Emial, Is.EqualTo("aaa@bbb.ccc"));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            var stiEmailTelNoInput = stiEmailTelNo.FindElement(By.TagName("Input"));
            stiEmailTelNoInput.SendKeys("02-12345678");
            stiEmailTelNoInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.SwitchTo().DefaultContent();

            同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");
            
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("電子帳單聯絡電話")));
            string 聯絡電話 = stiEmailTelNo.Text;

            That(聯絡電話, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(16)]
        public async Task TwcE100_17() // 該申請案件進入未結案件中
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);
            Thread.Sleep(500);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            var stiEmailInput = stiEmail.FindElement(By.TagName("Input"));
            stiEmailInput.SendKeys("aaa@bbb.ccc");
            stiEmailInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            var stiEmailTelNoInput = stiEmailTelNo.FindElement(By.TagName("Input"));
            stiEmailTelNoInput.SendKeys("02-12345678");
            stiEmailTelNoInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            var stiNoteInput = stiNote.FindElement(By.TagName("Input"));
            stiNoteInput.SendKeys("備註內容");
            stiNoteInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 中結 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("中結")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 中結);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode_Fourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var stormTreeNode_Second = stormTreeNode_Fourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            var 夾帶附件 = stormTreeNode_Second.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 新增文件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 新增文件);
            Thread.Sleep(500);

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
            actions.MoveToElement(上傳).Click().Perform();

            var stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
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
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);

            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(17)]
        public async Task TwcE100_18() // 看到註記欄位內容顯示-備註內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiNote = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote);

            string 註記 = stiNote.Text;

            That(註記, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(18)]
        public async Task TwcE100_19() // 看到打勾顯示■中結
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 中結 = driver.FindElement(By.Id("中結"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 中結);

            That(中結.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(19)]
        public async Task TwcE100_20() // 表單內容顯示■申請電子帳單，Email欄位內顯示aaa @bbb.ccc; 聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEmail);

            string Email = stiEmail.Text;

            That(Email, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEmailTelNo);

            string 聯絡電話 = stiEmailTelNo.Text;

            That(聯絡電話, Is.EqualTo("02-12345678"));
        }
    }
}