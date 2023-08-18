using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
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
            That(水號, Is.EqualTo("41105533310"));

            IWebElement spanName_ApplyDate = driver.FindElement(By.CssSelector("[sti-apply-date]"));
            string 申請日期 = spanName_ApplyDate.Text;
            That(申請日期, Is.EqualTo("2023年06月13日"));
        }

        [Test]
        [Order(3)]
        public async Task TwcG100_04() // driver_2中看到身分證字號欄位出現A123456789
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

            IWebElement idNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", idNo);

            idNo.SendKeys("A123456789");
            idNo.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            idNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-class='InputSelectBlock'][sti-trustee-id-no='']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", idNo);

            string 身分證號碼 = idNo.Text;

            That(身分證號碼, Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcG100_05() // driver_2看到■申請電子帳單
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

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);
            IWebElement 申請電子帳單勾選value = driver.FindElement(By.Id("申請電子帳單勾選value"));
            string spanGetAttribute = 申請電子帳單勾選value.GetAttribute("textContent");

            That(spanGetAttribute, Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcG100_06() // driver_2看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB  
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

            IWebElement 撫卹 = driver.FindElement(By.Id("檢附證件group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 撫卹);

            bool isChecked = 撫卹.Selected;

            That(isChecked, Is.True);

            IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));
            identification.SendKeys("BBB");
            identification.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            identification = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[data-class='InputSelectBlock'][sti-identification]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", identification);

            string actualValue = identification.Text;
            That(actualValue, Is.EqualTo("BBB"));
        }

        [Test]
        [Order(6)]
        public async Task TwcG100_07() // driver_2中看到●否
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

            IWebElement 否 = driver.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 否);
            否.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            IWebElement stiOverApply = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[data-class='MultiInputSelectBlockBlock'][sti-over-apply]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiOverApply);

            string actualValue = stiOverApply.Text;

            That(actualValue, Is.EqualTo("否"));
        }

        [Test]
        [Order(7)]
        public async Task TwcG100_08() // driver_2可看到註記欄位內容顯示-備註內容 四個字 
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

            IWebElement stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);

            stiNote.SendKeys("備註內容");
            stiNote.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-class='InputSelectBlock'][sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);

            string 註記欄位 = stiNote.Text;

            That(註記欄位, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(8)]
        public async Task TwcG100_09() // driver_2可看到顯示■繳費
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

            if (繳費checked != null)
            {
                Console.WriteLine("該元素具有'checked'屬性");
            }
            else
            {
                Console.WriteLine("該元素不具有'checked'屬性");
            }
        }

        [Test]
        [Order(9)]
        public async Task TwcG100_10() // driver_2看到受理欄位有落章
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
        [Order(10)]
        public async Task TwcG100_11() // driver_2中勾選消費性用水服務契約
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
        [Order(11)]
        public async Task TwcG100_12() // driver_2中勾選公司個人資料保護告知事項
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
        [Order(12)]
        public async Task TwcG100_13() // driver_2中勾選公司營業章程
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
        [Order(13)]
        public async Task TwcG100_14() // driver_2中表單畫面完整呈現簽名內容，並於driver_1中看到相容內容
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
        [Order(14)]
        public async Task TwcG100_15() // driver_2中看到掃描拍照證件圖像
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
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 啟動掃描證件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 啟動掃描證件);

            IWebElement imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_1 = imgElement.GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_2 = imgElement.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }

        [Test]
        [Order(15)]
        public async Task TwcG100_16() // driver_2中看到夾帶附件資訊
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

            IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

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
        [Order(16)]
        public async Task TwcG100_17() // 系統顯示[申請電子帳單需要填寫Email及聯絡電話]
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
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

            stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement 受理登記 = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(受理登記).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            IWebElement outerContainer = driver.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            IWebElement innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));
            string hintText = innerContainer.Text;

            That(hintText, Is.Not.Null);
        }

        [Test]
        [Order(17)]
        public async Task TwcG100_18() // driver_2中Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
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

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);
            Thread.Sleep(500);

            IWebElement email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email.SendKeys("aaa@bbb.ccc");
            email.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            IWebElement itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            IWebElement innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            IWebElement 同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            IWebElement stiEmailElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            string spanText_Email = stiEmailElement.Text;

            That(spanText_Email, Is.EqualTo("aaa@bbb.ccc"));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            IWebElement telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", telNo);
            telNo.SendKeys("02-12345678");
            telNo.SendKeys(Keys.Tab);

            driver.SwitchTo().DefaultContent();

            itemContainer = driver.FindElement(By.CssSelector("div.container-fluid.py-4.position-relative"));
            innerContainer = itemContainer.FindElement(By.CssSelector("div.position-fixed.translate-middle-y"));
            同步狀態 = innerContainer.FindElement(By.CssSelector("p.d-none"));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            IWebElement stiTelNoElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("電子帳單聯絡電話")));
            string spanText_TelNo = stiTelNoElement.Text;

            That(spanText_TelNo, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(18)]
        public async Task TwcG100_19() // 該申請案件進入未結案件中
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            IWebElement email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email.SendKeys("aaa@bbb.ccc");
            email.SendKeys(Keys.Tab);

            IWebElement telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            telNo.SendKeys("02-12345678");
            telNo.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            IWebElement 撫卹 = driver.FindElement(By.Id("檢附證件group3"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 撫卹);
            Thread.Sleep(500);

            IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));
            identification.SendKeys("BBB");
            identification.SendKeys(Keys.Tab);

            IWebElement 否 = driver.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 否);
            Thread.Sleep(500);

            IWebElement stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));

            stiNote.SendKeys("備註內容");
            stiNote.SendKeys(Keys.Tab);

            IWebElement 繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 繳費);
            Thread.Sleep(500);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);

            driver.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement 夾帶附件 = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new (driver);
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

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

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
        [Order(19)]
        public async Task TwcG100_20() // driver_1中有看到註記欄位內容顯示-備註內容
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement stiNote = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[data-class='InputSelectBlock'][sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote);

            var stiNoteElement = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-note]"));
            string spanText = stiNoteElement.Text;

            That(spanText, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(20)]
        public async Task TwcG100_21() // driver_1中有看到打勾顯示■繳費
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(21)]
        public async Task TwcG100_22() // 表單內容顯示■申請電子帳單，Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);

            bool isChecked = applyEmail.Selected;

            That(isChecked, Is.True);

            IWebElement email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", email);

            IWebElement stiEmailElement = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email='']"));
            string spanText_Email = stiEmailElement.Text;

            That(spanText_Email, Is.EqualTo("aaa@bbb.ccc"));

            IWebElement telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", telNo);

            IWebElement stiTelNoElement = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email-tel-no='']"));
            string spanText_TelNo = stiTelNoElement.Text;

            That(spanText_TelNo, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(22)]
        public async Task TwcG100_23() // 檢附證件欄位看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 撫卹 = driver.FindElement(By.Id("檢附證件radio"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            string spanText = 撫卹.Text;

            That(spanText, Is.EqualTo("撫卹令或撫卹金分領證書"));

            IWebElement identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification]")));

            string spanText_Identification = identification.Text;

            That(spanText_Identification, Is.EqualTo("BBB"));
        }

        [Test]
        [Order(23)]
        public async Task TwcG100_24() // 超戶申請欄位看到●否
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, TestHelper.UserId!, TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            IWebElement 否 = driver.FindElement(By.Id("超戶申請radio"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            string spanText = 否.Text;

            That(spanText, Is.EqualTo("否"));
        }
    }
}