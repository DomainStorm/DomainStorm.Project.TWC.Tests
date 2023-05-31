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
    public class TwcB100Tests
    {
        private IWebDriver driver_1;
        private IWebDriver driver_2;
        private string _accessToken;
        public TwcB100Tests()
        {
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            driver_1 = new ChromeDriver();
            driver_1.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver_2 = new ChromeDriver();
            driver_2.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            return Task.CompletedTask;
        }
        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            driver_1.Quit();
            driver_2.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcB100_01()
        {
            //取得token
            _accessToken = await TestHelper.GetAccessToken();
            TestHelper.AccessToken = _accessToken;
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcB100_02()
        {
            //呼叫bmRecoverApply/confirm
            var accessToken = TestHelper.AccessToken;
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B100_bmRecoverApply.json"));
            var json = await r.ReadToEndAsync();

            var update = JsonConvert.DeserializeObject<Serialization>(json);
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
        public async Task TwcB100_03()
        {
            //於數位簽名板driver_2中看到申請之表單內容
            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            Thread.Sleep(1000);

            string[] segments = driver_1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(driver_2, TestHelper.UserId, TestHelper.Password);
            driver_2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(driver_2, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            driver_2.SwitchTo().Frame(0);

            var 受理編號_driver_2 = driver_2.FindElement(By.CssSelector("[sti-apply-case-no]"));
            var 水號_driver_2 = driver_2.FindElement(By.CssSelector("[sti-water-no]"));
            var 受理日期_driver_2 = driver_2.FindElement(By.CssSelector("[sti-apply-date]"));

            That(受理編號_driver_2.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
            That(水號_driver_2.Text, Is.EqualTo("41101220266"));
            That(受理日期_driver_2.Text, Is.EqualTo("2023年04月10日"));
        }


        [Test]
        [Order(3)]
        public async Task TwcB100_04()
        {
            //driver_2可看到身分證字號欄位出現A123456789
            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            Thread.Sleep(1000);

            string[] segments = driver_1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(driver_2, TestHelper.UserId, TestHelper.Password);
            driver_2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            driver_1.SwitchTo().Frame(0);

            var 身分證字號_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));
            var 身分證字號_driver_2 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));
            身分證字號_driver_1.SendKeys("A123456789");

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 身分證字號_driver_1);

            That(身分證字號_driver_2.GetAttribute("value"), Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcB100_05()
        {
            //driver_2可看到顯示繳費
            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));

            Thread.Sleep(1000);

            string[] segments = driver_1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(driver_2, TestHelper.UserId, TestHelper.Password);
            driver_2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            driver_1.SwitchTo().Frame(0);

            var 繳費 = driver_1.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        //[Test]
        //[Order(5)]
        //public async Task TwcB100_06()
        //{
        //await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

        //driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

        //TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

        //    var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

        //    var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
        //    var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
        //    var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
        //    var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

        //    var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));
        //    Actions actions = new Actions(driver_1);
        //    actions.MoveToElement(href).Click().Perform();

        //    var 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

        //    That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        //}

        //[Test]
        //[Order(6)]
        //public async Task TwcB100_07()
        //{
        //await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

        //driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

        //TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

        //    var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

        //    var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
        //    var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
        //    var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
        //    var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

        //    var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#finished']"));
        //    Actions actions = new Actions(driver_1);
        //    actions.MoveToElement(href).Click().Perform();

        //    var 確認受理 = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 確認受理);

        //    var outerContainer = driver_1.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
        //    var innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));
        //    That(innerContainer.Displayed, Is.True);
        //}

        //[Test]
        //[Order(7)]
        //public async Task TwcB100_08()
        //{
        //await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

        //driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

        //TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

        //    var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

        //    driver_1.SwitchTo().Frame(0);

        //    var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
        //    ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理);

        //    var 已受理 = driver_1.FindElements(By.CssSelector("[class='sign']"));
        //    That(已受理, Is.Not.Empty, "未受理");
        //}

        //[Test]
        //[Order(8)]
        //public async Task TwcB100_09()
        //{

        //}

        //[Test]
        //[Order(9)]
        //public async Task TwcB100_10()
        //{

        //}
    }
}