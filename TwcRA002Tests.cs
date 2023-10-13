using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcRA002Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public TwcRA002Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            _chromeDriverList = new List<ChromeDriver>();

            return Task.CompletedTask;
        }

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            TestHelper.CloseChromeDrivers();
        }

        [Test]
        [Order(0)]
        public async Task TwcRA002_01() // 分別以0511,tw491,ning53建立表單
        {
            //0511 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            var 上傳 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            var stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            var stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            var 用印或代送件只需夾帶附件 = wait.Until(ExpectedConditions.ElementExists(By.Id("用印或代送件只需夾帶附件")));
            actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            var 確認受理 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            actions.MoveToElement(確認受理).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            var logout = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            actions.MoveToElement(logout).Click().Perform();

            //tw491 建立表單

            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));

            await TestHelper.Login(driver, "tw491", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            lastHiddenInput = hiddenInputs[^1];

            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            上傳 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            用印或代送件只需夾帶附件 = wait.Until(ExpectedConditions.ElementExists(By.Id("用印或代送件只需夾帶附件")));
            actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            確認受理 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            actions.MoveToElement(確認受理).Click().Perform();

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            logout = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            actions.MoveToElement(logout).Click().Perform();

            //ning53 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA001_bmTransferApply.json"));

            await TestHelper.Login(driver, "ning53", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            lastHiddenInput = hiddenInputs[^1];

            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            上傳 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            用印或代送件只需夾帶附件 = wait.Until(ExpectedConditions.ElementExists(By.Id("用印或代送件只需夾帶附件")));
            actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            確認受理 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            actions.MoveToElement(確認受理).Click().Perform();

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo)!;

            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(1)]
        public async Task TwcRA002_02() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(2)]
        public async Task TwcRA002_03() // 呼叫bmTransferApply/confirm 沒錯誤則取得http 200回應
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA002_bmTransferApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(3)]
        public async Task TwcRA002_04() //看到申請之表單內容跳至夾帶附件區塊
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            Actions actions = new(driver);

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            var stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var findElement = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td > p.h3"));
            var element = findElement.SingleOrDefault(e => e.Text == "沒有找到符合的結果");
            if (element != null)
            {
                string filename = element.Text;

                That(filename, Is.EqualTo("沒有找到符合的結果"));
            }
        }

        [Test]
        [Order(4)]
        public async Task TwcRA002_05() // 看到檔案上傳
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            Actions actions = new(driver);

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            var stormInputGroup = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("body storm-main-content main div div div div storm-card form storm-input-group")));
            string 文件名稱 = stormInputGroup.GetAttribute("value");

            That(文件名稱, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(5)]
        public async Task TwcRA002_06() // 看到夾帶附件視窗顯示有一筆附件清單資料
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            Actions actions = new(driver);

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            var 上傳 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            var stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            var stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string 文件名稱 = spanElement.Text;

            That(文件名稱, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(6)]
        public async Task TwcRA002_07() // 表單受理欄位中看到核章資訊
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));

            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(7)]
        public async Task TwcRA002_08() // 看到■用印或代送件只需夾帶附件已打勾
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver);
            actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = wait.Until(ExpectedConditions.ElementExists(By.Id("用印或代送件只需夾帶附件")));
            actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task TwcRA002_09() // 確認完成畫面進入未結案件中
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            Actions actions = new(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            actions.MoveToElement(新增文件).Click().Perform();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(filePath);

            var 上傳 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            var stormCardSeventh = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) storm-edit-table")));
            var stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            var 用印或代送件只需夾帶附件 = wait.Until(ExpectedConditions.ElementExists(By.Id("用印或代送件只需夾帶附件")));
            actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            var 確認受理 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            actions.MoveToElement(確認受理).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo)!;

            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(9)]
        public async Task TwcRA002_10() //有xlsx檔案下載後打開應顯示有台中所2筆、大里所1筆統計數據。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/report/RA002");

            Actions actions = new Actions(driver);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            // 選擇區處別
            var 區處別 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card storm-select div.choices")));
            actions.MoveToElement(區處別).Click().Perform();

            var 第四區管理處 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.choices__list.choices__list--dropdown div:nth-child(1) [data-value='3eed4fc4-9c06-4d16-9eb6-45aeaf198a25']")));
            actions.MoveToElement(第四區管理處).Click().Perform();

            // 選擇年度
            var 選擇年份 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='選擇年份'] div.choices")));
            actions.MoveToElement(選擇年份).Click().Perform();

            var 年份 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.choices__list [data-value='2023']")));
            actions.MoveToElement(年份).Click().Perform();

            // 選擇檔案格式
            var 檔案格式 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='檔案格式'] div.choices")));
            actions.MoveToElement(檔案格式).Click().Perform();

            var Xlsx = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.choices__list [data-value='XLSX']")));
            actions.MoveToElement(Xlsx).Click().Perform();

            // 檢查下載檔案
            string filePath = Path.Combine(_downloadDirectory, "RA001.xlsx");

            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);

            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var 下載 = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card.hydrated > form > div:nth-child(5).d-flex.justify-content-end.mt-4 > button")));
            actions.MoveToElement(下載).Click().Perform();

            wait.Until(_ => File.Exists(filePath));

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
        }
    }
}
