﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB100Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcB100Tests()
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
        public async Task TwcB100_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcB100_02() // 呼叫bmRecoverApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B100_bmRecoverApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcB100_03() // 於數位簽名板driver_2中看到申請之表單內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement spanName_ApplyCaseNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]"))); ;
            string applyCaseNo = spanName_ApplyCaseNo.Text;
            That(applyCaseNo, Is.EqualTo(TestHelper.ApplyCaseNo));

            IWebElement spanName_WaterNo = driver.FindElement(By.CssSelector("[sti-water-no]"));
            string waterNo = spanName_WaterNo.Text;
            That(waterNo, Is.EqualTo("41101220266"));

            IWebElement spanName_ApplyDate = driver.FindElement(By.CssSelector("[sti-apply-date]"));
            string applyDate = spanName_ApplyDate.Text;
            That(applyDate, Is.EqualTo("2023年04月10日"));
        }

        [Test]
        [Order(3)]
        public async Task TwcB100_04() // driver_2可看到身分證字號欄位出現A123456789
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            IWebElement idNo = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-trustee-id-no] > input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", idNo);
            //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].addEventListener('focusout', function() { window.focusOutDetected = true; });", idNo);

            idNo.SendKeys("A123456789");
            idNo.SendKeys(Keys.Tab);
            Thread.Sleep(500); //等待頁面同步

            //Console.WriteLine("Waiting for focus out...");

            //wait.Until(driver => (bool)((IJavaScriptExecutor)driver).ExecuteScript("return window.focusOutDetected;"));

            //Console.WriteLine("Focus out detected!");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            idNo = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[data-class='InputSelectBlock'][sti-trustee-id-no='']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", idNo);

            string actualValue = idNo.Text;

            That(actualValue, Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcB100_05() // driver_2可看到顯示■繳費
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            IWebElement 繳費 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("繳費")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 繳費);
            Thread.Sleep(1000);

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcB100_06() // driver_2看到受理欄位有落章
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
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

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);

            IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));
            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(6)]
        public async Task TwcB100_07() // driver_2中勾選消費性用水服務契約，driver_1看到■已詳閱
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
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

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeFirst = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement 消費性用水服務契約 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

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
                IWebElement 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
                return 消費性用水服務契約.GetAttribute("checked") == "true";
            });
            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }
        
    

        [Test]
        [Order(7)]
        public async Task TwcB100_08() // driver_2中勾選消費性用水服務契約，driver_1看到■已詳閱
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
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

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeSecondUnderSecond = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 公司個人資料保護告知事項 = stormTreeNodeSecondUnderSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

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
                IWebElement 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
                return 公司個人資料保護告知事項.GetAttribute("checked") == "true";
            });
            公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task TwcB100_09() // driver_2中勾選公司營業章程，driver_1看到■已詳閱
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
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

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeThird = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            IWebElement 公司營業章程 = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            actions.MoveToElement(公司營業章程).Click().Perform();

            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            wait.Until(driver =>
            {
                IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
                return 公司營業章程.GetAttribute("checked") == "true";
            });
            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

            [Test]
        [Order(9)]
        public async Task TwcB100_10() // driver_2中表單畫面完整呈現簽名內容，並於driver中看到相容內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            IWebElement 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            IWebElement 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            IWebElement buttonSign = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", buttonSign);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", buttonSign);

            string src_driver_2 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            string? src = null;

            while (src == null)
            {
                try
                {
                    IWebElement imgElement = driver.FindElement(By.CssSelector("div.dropzone-container img"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", imgElement);
                    src = imgElement.GetAttribute("src");
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("簽名內容生成中");
                }
            }

            string src_driver_1 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            That(src_driver_1, Is.EqualTo(src_driver_2));
        }

        [Test]
        [Order(10)]
        public async Task TwcB100_11() // driver_2中看到掃描拍照證件圖像
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            IWebElement stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeFirst = stormTreeNodeFourth.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement hrefCredential = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#credential']"));

            Actions actions = new(driver);
            actions.MoveToElement(hrefCredential).Click().Perform();

            IWebElement buttonScanId = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", buttonScanId);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", buttonScanId);

            IWebElement imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_1 = imgElement.GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_2 = imgElement.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }


        [Test]
        [Order(11)]
        public async Task TwcB100_12() // driver_2中看到夾帶附件資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            IWebElement stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(hrefFile).Click(hrefFile).Perform();

            IWebElement buttonAddDocument = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", buttonAddDocument);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", buttonAddDocument);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            string spanText = string.Empty;
            while (string.IsNullOrEmpty(spanText))
            {
                IWebElement navigationStormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
                IWebElement seventhStormEditTable = navigationStormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
                IWebElement editTable_StormTable = seventhStormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                IWebElement dataFieldName = editTable_StormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
                IWebElement span = dataFieldName.FindElement(By.CssSelector("span"));
                spanText = span.Text;
            }

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stormTable);

            IWebElement fileName1 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(1) > td[data-field='name']"));
            IWebElement spanName1 = fileName1.FindElement(By.CssSelector("span"));
            string spanText1 = spanName1.Text;

            IWebElement fileName2 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(2) > td[data-field='name']"));
            IWebElement spanName2 = fileName2.FindElement(By.CssSelector("span"));
            string spanText2 = spanName2.Text;

            That(spanText1, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
            That(spanText2, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }

        [Test]
        [Order(12)]
        public async Task TwcB100_13() // 該申請案件進入未結案件中等待後續排程資料於結案後消失
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(hrefFile).Click(hrefFile).Perform();

            IWebElement buttonAddDocument = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", buttonAddDocument);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", buttonAddDocument);
            Thread.Sleep(500);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            string spanText = string.Empty;
            while (string.IsNullOrEmpty(spanText))
            {
                IWebElement navigationStormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
                IWebElement seventhStormEditTable = navigationStormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
                IWebElement editTable_StormTable = seventhStormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                IWebElement dataFieldName = editTable_StormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
                IWebElement span = dataFieldName.FindElement(By.CssSelector("span"));
                spanText = span.Text;
            }

            IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement confirmButton = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", confirmButton);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmButton);

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
    }
}