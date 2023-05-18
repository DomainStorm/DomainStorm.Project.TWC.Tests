using Castle.Components.DictionaryAdapter.Xml;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Runtime.CompilerServices;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class Twc01_1Tests
    {
        private IWebDriver 螢幕1;
        private IWebDriver 螢幕2;
        private static string _accessToken;
        private bool _skipSetup = true;
        private bool _skipTearDown = true;

        public Twc01_1Tests()
        {
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            if (_skipSetup)
            {
                螢幕1 = new ChromeDriver();
                螢幕1.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                螢幕2 = new ChromeDriver();
                螢幕2.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }
            return Task.CompletedTask;
        }
        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            if (_skipTearDown)
            {
                螢幕1.Quit();
                螢幕2.Quit();
            }
        }


        [Test]
        [Order(0)]
        public async Task Twc01_01()
        {
            _accessToken ??= await TestHelper.GetAccessToken();
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task Twc01_02()
        {
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/tpcweb_01_1啟用_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            var guid = TestHelper.GetSerializationObject(json);
            guid.applyCaseNo = "111124";
            var updatedJson = JsonConvert.SerializeObject(guid);

            request.AddParameter("application/json", updatedJson, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task Twc01_03()
        {
            await TestHelper.Login(螢幕1, "0511", "password");

            螢幕1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(螢幕1, "111124");

            Thread.Sleep(1000);

            string[] segments = 螢幕1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(螢幕2, "0511", "password");
            螢幕2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            螢幕1.SwitchTo().Frame(0);

            var 受理編號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var 水號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var 受理日期 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));

            That(受理編號.Text, Is.EqualTo("111124"));
            That(水號.Text, Is.EqualTo("41101202191"));
            That(受理日期.Text, Is.EqualTo("2023年03月06日"));
        }


        [Test]
        [Order(3)]
        public async Task Twc01_04()
        {
            await TestHelper.Login(螢幕1, "0511", "password");

            螢幕1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(螢幕1, "111124");

            Thread.Sleep(1000);

            string[] segments = 螢幕1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(螢幕2, "0511", "password");
            螢幕2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            螢幕1.SwitchTo().Frame(0);

            var 身分證字號 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));

            身分證字號.SendKeys("A123456789");

            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 身分證字號);

            That(身分證字號.GetAttribute("value"), Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task Twc01_05()
        {
            await TestHelper.Login(螢幕1, "0511", "password");

            螢幕1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(螢幕1, "111124");

            Thread.Sleep(1000);

            string[] segments = 螢幕1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(螢幕2, "0511", "password");
            螢幕2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            螢幕1.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);

            wait.Until(ExpectedConditions.ElementToBeClickable(受理));

            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);
            That(受理.Displayed, Is.True);
        }

        [Test]
        [Order(5)]
        public async Task Twc01_06()
        {
            _skipSetup = false;
            _skipTearDown = false;
            await TestHelper.Login(螢幕1, "0511", "password");

            螢幕1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(螢幕1, "111124");

            Thread.Sleep(1000);

            string[] segments = 螢幕1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(螢幕2, "0511", "password");
            螢幕2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();
            var firstStormTreeNode = stormTreeRoot.FindElement(By.CssSelector("storm-tree-node:first-child"));

            var href = firstStormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));
            Actions actions = new Actions(螢幕1);
            actions.MoveToElement(href).Click().Perform();

            var 消費性用水服務契約_螢幕1 = 螢幕1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_螢幕1);

            var 消費性用水服務契約_螢幕2 = 螢幕2.FindElement(By.Id("消費性用水服務契約"));
            //螢幕2.SwitchTo().Window(螢幕2.WindowHandles[0]);
            ((IJavaScriptExecutor)螢幕2).ExecuteScript("arguments[0].click();", 消費性用水服務契約_螢幕2);

            //螢幕1.SwitchTo().Window(螢幕1.WindowHandles[0]);
            That(消費性用水服務契約_螢幕1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(6)]
        public async Task Twc01_07()
        {
            //await TestHelper.Login(_driver1, "0511", "password");

            //_driver1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            //TestHelper.ClickRow(_driver1, "111124");

            //Thread.Sleep(1000);

            //string[] segments = _driver1.Url.Split('/');
            //string id = segments[segments.Length - 1];

            //await TestHelper.Login(_driver2, "0511", "password");
            //_driver2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();

            var secondStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[1];
            var secondStormTreeNodeShadowRoot = secondStormTreeNode.GetShadowRoot();

            var href = secondStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_2']"));
            Actions actions = new Actions(螢幕1);
            actions.MoveToElement(href).Click().Perform();

            var checkBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("公司個人資料保護告知事項")));
            var 公司個人資料保護告知事項 = 螢幕1.FindElement(By.Id("公司個人資料保護告知事項"));

            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
            wait.Until(ExpectedConditions.ElementToBeClickable(公司個人資料保護告知事項));
            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task Twc01_08()
        {

            //await TestHelper.Login(_driver1, "0511", "password");

            //_driver1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            //TestHelper.ClickRow(_driver1, "111124");

            //Thread.Sleep(1000);

            //string[] segments = _driver1.Url.Split('/');
            //string id = segments[segments.Length - 1];

            //await TestHelper.Login(_driver2, "0511", "password");
            //_driver2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();

            var thirdStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[2];
            var thirdStormTreeNodeShadowRoot = thirdStormTreeNode.GetShadowRoot();

            var href = thirdStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_3']"));
            Actions actions = new Actions(螢幕1);
            actions.MoveToElement(href).Click().Perform();

            var checkBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("公司營業章程")));
            var 公司營業章程 = 螢幕1.FindElement(By.Id("公司營業章程"));

            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            wait.Until(ExpectedConditions.ElementToBeClickable(公司營業章程));
            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].click();", 公司營業章程);
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task Twc01_09()
        {
            //await TestHelper.Login(_driver1, "0511", "password");

            //_driver1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            //TestHelper.ClickRow(_driver1, "111124");

            //Thread.Sleep(1000);

            //string[] segments = _driver1.Url.Split('/');
            //string id = segments[segments.Length - 1];

            //await TestHelper.Login(_driver2, "0511", "password");
            //_driver2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(螢幕1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            // 等待 vertical-navigation 可見
            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));

            // 找到 tree-view 元素
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));

            // 找到 tree-node 元素
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];

            // 取得 tree-node 元素的 ShadowRoot
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            // 找到 href 元素
            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#signature']"));
            Actions actions = new Actions(螢幕1);
            actions.MoveToElement(href).Click().Perform();

            var 簽名 = 螢幕1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名);
            wait.Until(ExpectedConditions.ElementToBeClickable(簽名));
            ((IJavaScriptExecutor)螢幕1).ExecuteScript("arguments[0].click();", 簽名);


            var img = 螢幕1.FindElement(By.CssSelector("img[src^='data:image/png;']"));
            That(螢幕1.FindElements(By.CssSelector("img[src^='data:image/png;']")).Any(), Is.True);
        }


        //    [Test]
        //    [Order(9)]
        //    public async Task Twc01_10()
        //    {
        //await TestHelper.Login(_driver1, "0511", "password");

        //_driver1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

        //TestHelper.ClickRow(_driver1, "111124");

        //    Thread.Sleep(1000);

        //    string[] segments = _driver1.Url.Split('/');
        //string id = segments[segments.Length - 1];

        //await TestHelper.Login(_driver2, "0511", "password");
        //_driver2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

        //        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        //        wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
        //        _driver.SwitchTo().Frame(0);

        //        // 等待 vertical-navigation 可見
        //        var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));

        //        // 找到 card 元素
        //        var stormCard = stormVerticalNavigation.GetShadowRoot().FindElements(By.CssSelector("storm-card"))[5];

        //        var 啟動掃描證件 = _driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
        //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 啟動掃描證件);
        //        wait.Until(ExpectedConditions.ElementToBeClickable(啟動掃描證件));
        //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 啟動掃描證件);

        //        var img = _driver.FindElement(By.CssSelector("img[src^='data:image/png;']"));
        //        That(_driver.FindElements(By.CssSelector("img[src^='data:image/png;']")).Any(), Is.True);
        //    }
    }

}