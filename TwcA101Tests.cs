using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA101Tests
    {
        private List<ChromeDriver> _chromeDriverList;

        public TwcA101Tests()
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
            var driver = new ChromeDriver();
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
        public async Task TwcA101_01()
        {
            //取得token
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcA101_02()
        {
            //呼叫bmEnableApply/confirm
            var accessToken = TestHelper.AccessToken;
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            var update = JsonConvert.DeserializeObject<WaterForm>(json);
            //update.applyCaseNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            update.applyCaseNo = TestHelper.ApplyCaseNo;
            update.userCode = TestHelper.UserId;
            var updatedJson = JsonConvert.SerializeObject(update);

            request.AddParameter("application/json", updatedJson, ParameterType.RequestBody);

            var response = await client.PostAsync(request);
            That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcA101_03()
        {
            //driver_1中看到申請之表單內容跳至夾帶附件區塊
            var driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var SecondstormTreeNode = stormTreeNode.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];

            var href = SecondstormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var container = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card#file")));
            That(container.Displayed, Is.True);
        }

        //[Test]
        //[Order(3)]
        //public async Task TwcA101_04()
        //{
        //driver_1可看到檔案上傳
        //}

        //[Test]
        //[Order(4)]
        //public async Task TwcA101_05()
        //{
        //driver_1看到夾帶附件視窗顯示有一筆附件清單資料
        //}

        [Test]
        [Order(5)]
        public async Task TwcA101_06()
        {
            //driver_1中看到■用印或代送件只需夾帶附件已打勾
            var driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(6)]
        public async Task TwcA101_07()
        {
            //系統跳出【受理】尚未核章
            var driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

            var outerContainer = driver_1.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            var innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));
            That(innerContainer.Displayed, Is.True);
        }

        [Test]
        [Order(7)]
        public async Task TwcA101_08()
        {
            //driver_1中表單受理欄位中看到核章資訊
            var driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理);

            var 已受理 = driver_1.FindElements(By.CssSelector("[class='sign']"));
            That(已受理, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(8)]
        public async Task TwcA101_09()
        {
            //確認完成畫面進入未結案件中
            await TwcA101_08();

            var driver_1 = _chromeDriverList[0];

            driver_1.SwitchTo().DefaultContent();

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            var 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

            string targetUrl = $"{TestHelper.LoginUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));

            var card = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = card.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var findElements = stormTable.GetShadowRoot()
                .FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            var element = findElements.FirstOrDefault(e => e.Text == TestHelper.ApplyCaseNo);
            var applyCaseNo = element.Text;

            That(applyCaseNo, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(9)]
        public async Task TwcA101_10()
        {
            //driver_1中看到夾帶附件區塊顯示該夾帶檔案。已勾選■已詳閱貴公司消費性用水服務契約，已勾選■已詳閱公司個人資料保護法，已勾選■已詳閱貴公司營業章程
            var driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/unfinished");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();
            var firstStormTreeNode = stormTreeRoot.FindElement(By.CssSelector("storm-tree-node:first-child"));

            var href_contract_1 = firstStormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href_contract_1).Click().Perform();

            var 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            var secondStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[1];
            var secondStormTreeNodeShadowRoot = secondStormTreeNode.GetShadowRoot();

            var href_contract_2 = secondStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_2']"));
            actions.MoveToElement(href_contract_2).Click().Perform();

            var 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));

            var thirdStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[2];
            var thirdStormTreeNodeShadowRoot = thirdStormTreeNode.GetShadowRoot();

            var href_contract_3 = thirdStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_3']"));
            actions.MoveToElement(href_contract_3).Click().Perform();

            var 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }
    }

}
