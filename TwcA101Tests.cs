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
        private IWebDriver driver_1;
        private string _accessToken;
        private string _applyCaseNo = "0529101";
        public TwcA101Tests()
        {
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            driver_1 = new ChromeDriver();
            driver_1.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return Task.CompletedTask;
        }
        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            driver_1.Quit();
        }
        
        [Test]
        [Order(0)]
        public async Task TwcA101_01()
        {
            _accessToken = await TestHelper.GetAccessToken();
            TestHelper.TestConfig.AccessToken = _accessToken;
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcA101_02()
        {
            var accessToken = TestHelper.AccessToken;
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            var update = JsonConvert.DeserializeObject<Serialization>(json);
            //update.applyCaseNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            update.applyCaseNo = "0529101";
            var updatedJson = JsonConvert.SerializeObject(update);

            request.AddParameter("application/json", updatedJson, ParameterType.RequestBody);

            var response = await client.PostAsync(request);
            That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcA101_03()
        {
            await TestHelper.Login(driver_1, "0511", "password");

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, _applyCaseNo);

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

        //}

        //[Test]
        //[Order(4)]
        //public async Task TwcA101_05()
        //{

        //}

        [Test]
        [Order(5)]
        public async Task TwcA101_06()
        {
            await TestHelper.Login(driver_1, "0511", "password");

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, _applyCaseNo);

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
            await TestHelper.Login(driver_1, "0511", "password");

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, _applyCaseNo);

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
            await TestHelper.Login(driver_1, "0511", "password");

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, _applyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理);

            var 已受理 = driver_1.FindElements(By.CssSelector("[class='sign']"));
            That(已受理, Is.Not.Empty, "未受理");
        }

        //[Test]
        //[Order(8)]
        //public async Task TwcA101_09()
        //{

        //}

        //[Test]
        //[Order(9)]
        //public async Task TwcA101_10()
        //{

        //}
    }
    
}
