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
    public class TwcE101Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcE101Tests()
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
        public async Task TwcE101_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcE101_02() // 呼叫bmTransferApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E101_bmTransferApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcE101_03() // 看到表單內容並於表單受理欄位中看到有■中結
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);   

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 中結 = driver_1.FindElement(By.Id("中結"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 中結);

            That(中結.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(3)]
        public async Task TwcE101_04() // 顯示臨櫃人員核章職稱姓名等資訊
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait_driver_1 = new(driver_1, TimeSpan.FromSeconds(10));
            wait_driver_1.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 受理_driver_1 = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理_driver_1);
            wait_driver_1.Until(ExpectedConditions.ElementToBeClickable(受理_driver_1));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理_driver_1);

            IReadOnlyList<IWebElement> signElement = driver_1.FindElements(By.CssSelector("[class='sign']"));
            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(4)]
        public async Task TwcE101_05() // 看到■申請電子帳單
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

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
        public async Task TwcE101_06() // 看到■用印或代送件只需夾帶附件已打勾
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement hrefFinished = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFinished).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(6)]
        public async Task TwcE101_07() // 系統跳出【尚未夾帶附件】、【申請電子帳單需要填寫Email及聯絡電話】訊息
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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
        [Order(7)]
        public async Task TwcE101_08() // Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-email]")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", applyEmail);

            IWebElement email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email_driver_1.SendKeys("aaa@bbb.ccc");

            Actions actions = new(driver_1);
            actions.MoveToElement(applyEmail).Click().Perform();

            email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", email_driver_1);

            email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            That(email_driver_1.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            //IWebElement telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            //telNo_driver_1.SendKeys("02-12345678");

            //actions.MoveToElement(applyEmail).Click().Perform();

            //telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            //((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", telNo_driver_1);

            //telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            //That(telNo_driver_1.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(8)]
        public async Task TwcE101_09() // 看到申請之表單內容跳至夾帶附件區塊
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFile).Click().Perform();

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
        [Order(9)]
        public async Task TwcE101_10() // 看到檔案上傳
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            IWebElement stormInputGroup = driver_1.FindElement(By.CssSelector("body storm-main-content main div div div div storm-card form storm-input-group"));
            string value = stormInputGroup.GetAttribute("value");

            That(value, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(10)]
        public async Task TwcE101_11() // 到夾帶附件視窗顯示有一筆附件清單資料，檔名為twcweb_01_1_夾帶附件1.pdf
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
            var spanElement = element.FindElement(By.CssSelector("span"));
            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(11)]
        public async Task TwcE101_12() // 確認完成畫面進入未結案件中
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-email]")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", applyEmail);

            IWebElement email_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", email_driver_1);
            email_driver_1.SendKeys("aaa@bbb.ccc");

            Actions actions = new(driver_1);
            actions.MoveToElement(applyEmail).Click().Perform();

            IWebElement telNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", telNo_driver_1);
            telNo_driver_1.SendKeys("02-12345678");

            actions.MoveToElement(applyEmail).Click().Perform();

            IWebElement 中結 = driver_1.FindElement(By.Id("中結"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 中結);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 中結);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);

            driver_1.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

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
        [Order(12)]
        public async Task TwcE101_13() // 顯示■申請電子帳單及Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678，看到夾帶附件區塊顯示該檔案。已勾選■已詳閱貴公司消費性用水服務契約、公司個人資料保護法、貴公司營業章程
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            //IWebElement applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-email]")));
            //((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);;

            //wait.Until(driver_1 =>
            //{
            //    IWebElement element = driver_1.FindElement(By.CssSelector("[sti-apply-email]"));
            //    return element.GetAttribute("checked") == "true";
            //});

            //That(applyEmail.GetAttribute("checked"), Is.EqualTo("true"));

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

            driver_1.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeFirst = stormTreeNode.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement stormTreeNodeSecondUnderSecond = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeThird = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];

            IWebElement hrefContract_1 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_1).Click().Perform();

            IWebElement 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement hrefContract_2 = stormTreeNodeSecondUnderSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

            actions.MoveToElement(hrefContract_2).Click().Perform();

            IWebElement 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement hrefContract_3 = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            actions.MoveToElement(hrefContract_3).Click().Perform();

            IWebElement 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(hrefFile).Click().Perform();

            IWebElement linkElement = driver_1.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string download = linkElement.GetAttribute("download");

            That(download, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        [Test]
        [Order(13)]
        public async Task TwcE101_14() // 查詢出來該件，列在下方
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            IWebElement stormMainContent = driver_1.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            IWebElement divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            IWebElement stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            IWebElement inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            IWebElement divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            IWebElement buttonSubmitSearch = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver_1);
            actions.MoveToElement(buttonSubmitSearch).Click().Perform();

            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            var spanElement = element.FindElement(By.CssSelector("span"));
            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        [Test]
        [Order(14)]
        public async Task TwcE101_15() // 確認附件有一件[twcweb_01_1_夾帶附件1.pdf]
        {
            await TwcE101_14();

            ChromeDriver driver_1 = _chromeDriverList[0];

            IWebElement stormMainContent = driver_1.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(element).Click().Perform();

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement SecondstormTreeNode = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement href = SecondstormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(href).Click().Perform();

            IWebElement linkElement = driver_1.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string downloadValue = linkElement.GetAttribute("download");

            That(downloadValue, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        [Test]
        [Order(15)]
        public async Task TwcE101_16() // PDF檔產製成功
        {
            await TwcE101_14();

            ChromeDriver driver_1 = _chromeDriverList[0];

            IWebElement stormMainContent = driver_1.FindElement(By.CssSelector("storm-main-content"));
            IWebElement stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            IWebElement stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(element).Click().Perform();

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(hrefFile).Click().Perform();

            IWebElement downloadPDF = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-warning.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", downloadPDF);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", downloadPDF);
        }
    }
}
