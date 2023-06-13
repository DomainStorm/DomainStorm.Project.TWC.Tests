using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;


namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB101Tests
    {
        private List<ChromeDriver> _chromeDriverList;
        public TwcB101Tests()
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
            option.AddArguments("start-maximized");
            option.AddArguments("--disable-gpu");
            if (TestHelper.GetChromeConfig().Headless)
                option.AddArguments("--headless");
            //new DriverManager().SetUpDriver(new ChromeConfig());
            Console.WriteLine("Setup");
            var driver = new ChromeDriver(option);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
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
        public async Task TwcB101_01()
        {
            //取得token
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]



        public async Task TwcB101_02()
        {
            //呼叫bmRecoverApply/confirm
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B101_bmRecoverApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcB101_03()
        {
            //driver_1中看到申請之表單內容跳至夾帶附件區塊
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement SecondstormTreeNode = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement href = SecondstormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement container = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card#file")));

            That(container.Displayed, Is.True);
        }


        //[Test]
        //[Order(3)]
        //public async Task TwcB101_04()
        //{
        //    //driver_1可看到夾帶附件先新增第一筆附件後新增第二筆附件；再刪除第一筆後最後顯示第二筆
        //}

        [Test]
        [Order(4)]
        public async Task TwcB101_05()
        {
            //driver_1中看到■用印或代送件只需夾帶附件已打勾
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            ISearchContext stormTreeNodeRoot = stormTreeNode.GetShadowRoot();
            IWebElement href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcB101_06()
        {
            //系統跳出【受理】尚未核章
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            ISearchContext stormTreeNodeRoot = stormTreeNode.GetShadowRoot();
            IWebElement href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

            IWebElement outerContainer = driver_1.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            IWebElement innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));

            That(innerContainer.Displayed, Is.True);
        }

        [Test]
        [Order(6)]
        public async Task TwcB101_07()
        {
            //driver_1表單受理欄位中看到核章資訊
            ChromeDriver driver_1 = GetNewChromeDriver();
            
            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理);

            IReadOnlyList<IWebElement> 已受理 = driver_1.FindElements(By.CssSelector("[class = 'sign']"));
            That(已受理, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(7)]
        public async Task TwcB101_08()
        {
            //確認完成畫面進入未結案件中
            await TwcB101_07();

            ChromeDriver driver_1 = _chromeDriverList[0];

            driver_1.SwitchTo().DefaultContent();

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            ISearchContext stormTreeNodeRoot = stormTreeNode.GetShadowRoot();
            IWebElement href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

            string targetUrl = $"{TestHelper.LoginUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));

            IWebElement card = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            IWebElement stormDocumentListDetail = card.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IReadOnlyList<IWebElement> findElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            var element = findElements.FirstOrDefault(e => e.Text == TestHelper.ApplyCaseNo);
            if (element != null)
            {
                string applyCaseNo = element.Text;

                That(applyCaseNo, Is.EqualTo(TestHelper.ApplyCaseNo));
            }
        }

        [Test]
        [Order(8)]
        public async Task TwcB101_09()
        {
            //driver_1中看到夾帶附件區塊顯示該夾帶檔案。已勾選■已詳閱貴公司消費性用水服務契約、公司個人資料保護法、貴公司營業章程
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement SecondstormTreeNode = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement href = SecondstormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement stormCard = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement linkElement = driver_1.FindElement(By.CssSelector("a[download='tpcweb_01_1_夾帶附件1.pdf']"));
            string downloadValue = linkElement.GetAttribute("download");

            That(downloadValue, Is.EqualTo("tpcweb_01_1_夾帶附件1.pdf"));

            IWebElement stormTreeView_contract = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IReadOnlyList<IWebElement> stormTreeNode_contract = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));
            IWebElement stormTreeNodes = stormTreeNode_contract[1];
            ISearchContext stormTreeRoot = stormTreeNodes.GetShadowRoot();
            IWebElement firstStormTreeNode = stormTreeRoot.FindElement(By.CssSelector("storm-tree-node:first-child"));
            IWebElement href_contract_1 = firstStormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            actions.MoveToElement(href_contract_1).Click().Perform();

            IWebElement 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement secondStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[1];
            ISearchContext secondStormTreeNodeShadowRoot = secondStormTreeNode.GetShadowRoot();

            IWebElement href_contract_2 = secondStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_2']"));
            actions.MoveToElement(href_contract_2).Click().Perform();

            IWebElement 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            IWebElement thirdStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[2];
            ISearchContext thirdStormTreeNodeShadowRoot = thirdStormTreeNode.GetShadowRoot();

            IWebElement href_contract_3 = thirdStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_3']"));
            actions.MoveToElement(href_contract_3).Click().Perform();

            IWebElement 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(9)]
        public async Task TwcB101_10()
        {
            //driver_1夾帶附件處只出現[tpcweb_01_1_夾帶附件2.pdf]
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/unfinished");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement SecondstormTreeNode = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement href = SecondstormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(href).Click().Perform();

            IWebElement divElement = driver_1.FindElement(By.CssSelector("div.list-group"));
            That(divElement.FindElements(By.CssSelector("a")).Count, Is.EqualTo(1), "該 <div> 元素不符合條件");

            IWebElement linkElement = divElement.FindElement(By.CssSelector("a"));
            That(linkElement.Text, Is.EqualTo("tpcweb_01_1_夾帶附件2.pdf"), "該筆資料的名稱不符合預期");
        }
    }
}