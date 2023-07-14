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
    public class TwcC100Tests
    {
        private List<ChromeDriver> _chromeDriverList;

        public TwcC100Tests()
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
        public async Task TwcC100_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcC100_02() // 呼叫bmDisableApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C100_bmDisableApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcC100_03() // driver_2中看到申請之表單內容
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
            string 受理編號 = spanName_ApplyCaseNo.Text;
            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));

            IWebElement spanName_WaterNo = driver.FindElement(By.CssSelector("[sti-water-no]"));
            string 水號 = spanName_WaterNo.Text;
            That(水號, Is.EqualTo("41101633338"));

            IWebElement spanName_ApplyDate = driver.FindElement(By.CssSelector("[sti-apply-date]"));
            string 申請日期 = spanName_ApplyDate.Text;
            That(申請日期, Is.EqualTo("2023年06月03日"));
        }

        [Test]
        [Order(3)]
        public async Task TwcC100_04() // driver_2中看到身分證字號欄位出現A123456789
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

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement identityNumber = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", identityNumber);

            identityNumber.SendKeys("A123456789");
            identityNumber.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            identityNumber = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-class='InputSelectBlock'][sti-trustee-id-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", identityNumber);

            string 身分證號碼 = identityNumber.Text;

            That(身分證號碼, Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcC100_05() // driver_2可看到顯示■中結■繳費
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

            IWebElement 中結 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("中結")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 中結);

            driver.SwitchTo().DefaultContent();
            
            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            繳費 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("繳費")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);

            var 繳費checked = 繳費.GetAttribute("checked");

            if (繳費checked != null) {
                Console.WriteLine("該元素具有'checked'屬性");
            }else {
                Console.WriteLine("該元素不具有'checked'屬性");
            }

            中結 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("中結")));

            var 中結checked = 中結.GetAttribute("checked");

            if (中結checked != null)
            {
                Console.WriteLine("該元素具有'checked'屬性");
            }
            else
            {
                Console.WriteLine("該元素不具有'checked'屬性");
            }
        }

        [Test]
        [Order(5)]
        public async Task TwcC100_06() // driver_2看到受理欄位有落章
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
        [Order(6)]
        public async Task TwcC100_07() // driver_2中勾選消費性用水服務契約，driver_1看到■已詳閱
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
        public async Task TwcC100_08() // driver_2中勾選公司個人資料保護告知事項，driver_1看到■已詳閱
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
        public async Task TwcC100_09() // driver_2中勾選公司營業章程，driver_1看到■已詳閱
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
                IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
                return 公司營業章程.GetAttribute("checked") == "true";
            });
            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(9)]
        public async Task TwcC100_10() // driver_2中表單畫面完整呈現簽名內容，並於driver_1中看到相容內容
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

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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

            IWebElement 簽名 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 簽名);

            string src_driver_2 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            IWebElement imgElement = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container img")));
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
        [Order(10)]
        public async Task TwcC100_11() // driver_2中看到掃描拍照證件圖像
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
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeFirst = stormTreeNodeFourth.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement 掃描拍照 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#credential']"));

            actions.MoveToElement(掃描拍照).Click().Perform();

            IWebElement 啟動掃描證件 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            actions.MoveToElement(啟動掃描證件).Perform();
            啟動掃描證件.Click();

            IWebElement imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_1 = imgElement.GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_2 = imgElement.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }

        [Test]
        [Order(11)]
        public async Task TwcC100_12() // driver_2中看到夾帶附件資訊
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
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Perform();
            上傳.Click();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            IWebElement spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
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
        public async Task TwcC100_13() // 該申請案件進入未結案件中等待後續排程資料於結案後消失
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));
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
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

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

            Console.WriteLine($"::group::---------{driver.Url}---------");
            Console.WriteLine(driver.PageSource);
            Console.WriteLine("::endgroup::");

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl)); //每幾秒log
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            //Console.WriteLine($"::group::---------{targetUrl}---------");
            //Console.WriteLine(targetUrl);
            //Console.WriteLine("::endgroup::");

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);
            
            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
    }
}