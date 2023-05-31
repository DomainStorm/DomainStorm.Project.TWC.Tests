﻿using Newtonsoft.Json;
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
            //driver_2可看到顯示■繳費
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

        [Test]
        [Order(5)]
        public async Task TwcB100_06()
        {
            //driver_2看到受理欄位有落章
            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait_driver_1 = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            var wait_driver_2 = new WebDriverWait(driver_2, TimeSpan.FromSeconds(10));
            Thread.Sleep(1000);

            string[] segments = driver_1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(driver_2, TestHelper.UserId, TestHelper.Password);
            driver_2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            wait_driver_1.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            wait_driver_2.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            driver_1.SwitchTo().Frame(0);

            var 受理_driver_1 = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理_driver_1);

            wait_driver_1.Until(ExpectedConditions.ElementToBeClickable(受理_driver_1));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理_driver_1);

            wait_driver_2.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.CssSelector("iframe")));
            var 已受理 = driver_2.FindElements(By.CssSelector("[class='sign']"));
            That(已受理, Is.Not.Empty, "未受理");

        }

        [Test]
        [Order(6)]
        public async Task TwcB100_07()
        {
            //driver_2中看到申請之表單內容滾動；driver_1看到■已詳閱貴公司消費性用水服務契約
            await TwcB100_06();

            driver_1.SwitchTo().DefaultContent();
            driver_2.SwitchTo().DefaultContent();

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();
            var firstStormTreeNode = stormTreeRoot.FindElement(By.CssSelector("storm-tree-node:first-child"));

            var href = firstStormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            var 消費性用水服務契約_driver_2 = driver_2.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 消費性用水服務契約_driver_2);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task TwcB100_08()
        {
            //driver_2中看到申請之表單內容滾動；driver_1看到■已詳閱公司個人資料保護法
            await TwcB100_07();

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();
            var secondStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[1];
            var secondStormTreeNodeShadowRoot = secondStormTreeNode.GetShadowRoot();

            var href = secondStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_2']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            var 公司個人資料保護告知事項_driver_2 = driver_2.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項_driver_2);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task TwcB100_09()
        {
            //driver_2中看到申請之表單內容滾動；driver_1看到■已詳閱貴公司營業章程
            await TwcB100_08();

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();

            var thirdStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[2];
            var thirdStormTreeNodeShadowRoot = thirdStormTreeNode.GetShadowRoot();

            var href = thirdStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_3']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            var 公司營業章程_driver_2 = driver_2.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司營業章程_driver_2);

            wait.Until(driver_1 =>
            {
                var element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(9)]
        public async Task TwcB100_10()
        {
            // driver_2表單畫面完整呈現簽名內容；driver_1同樣看到完整簽名內容及年月日時分秒資訊
            await TwcB100_09();

            var wait = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#signature']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 簽名_driver_1 = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名_driver_1);

            var 簽名_driver_2 = driver_2.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 簽名_driver_2);

            var containerElement_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            var containerElement_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));

            var 圖片元素_driver_1 = containerElement_driver_1.FindElement(By.CssSelector("img"));
            var 圖片元素_driver_2 = containerElement_driver_2.FindElement(By.CssSelector("img"));

            var 圖片_driver_1_src = 圖片元素_driver_1.GetAttribute("src");
            var 圖片_driver_2_src = 圖片元素_driver_2.GetAttribute("src");

            That(圖片_driver_2_src, Is.EqualTo(圖片_driver_1_src));
        }

        [Test]
        [Order(10)]
        public async Task TwcB100_11()
        {
            //driver_2中看到掃描拍照證件圖像
            await TestHelper.Login(driver_1, TestHelper.UserId, TestHelper.Password);

            driver_1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo);

            var wait_driver_1 = new WebDriverWait(driver_1, TimeSpan.FromSeconds(10));
            var wait_driver_2 = new WebDriverWait(driver_2, TimeSpan.FromSeconds(10));
            Thread.Sleep(1000);

            string[] segments = driver_1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(driver_2, TestHelper.UserId, TestHelper.Password);
            driver_2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            wait_driver_1.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            wait_driver_2.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait_driver_1.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            var UnderRootstormTreeNode = stormTreeNodeRoot.GetShadowRoot();
            var href = UnderRootstormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));
            Actions actions = new Actions(driver_1);
            actions.MoveToElement(href).Click().Perform();

            var 啟動掃描證件 = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 啟動掃描證件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 啟動掃描證件);

            var containerElement_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            var containerElement_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));
            var 圖片元素_driver_1 = containerElement_driver_1.FindElement(By.CssSelector("img"));
            var 圖片元素_driver_2 = containerElement_driver_2.FindElement(By.CssSelector("img"));
            var 圖片_driver_1_src = 圖片元素_driver_1.GetAttribute("src");
            var 圖片_driver_2_src = 圖片元素_driver_2.GetAttribute("src");

            That(圖片_driver_1_src, Is.EqualTo(圖片_driver_2_src));
        }


        //[Test]
        //[Order(11)]
        //public async Task TwcB100_12()
        //{
        //    //driver_2中看到夾帶附件資訊
        //}

        [Test]
        [Order(12)]
        public async Task TwcB100_13()
        {
            //該申請案件進入未結案件中等待後續排程資料於結案後消失
            await TwcB100_10();

            driver_2.Quit();

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
    }
}