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
        private IWebDriver �ù�1;
        private IWebDriver �ù�2;
        private static string _accessToken;
        private bool _skipSetup = true;
        private bool _skipTearDown = true;

        public Twc01_1Tests()
        {
        }

        [SetUp] // �b�C�Ӵ��դ�k���e���檺��k
        public Task Setup()
        {
            if (_skipSetup)
            {
                �ù�1 = new ChromeDriver();
                �ù�1.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                �ù�2 = new ChromeDriver();
                �ù�2.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }
            return Task.CompletedTask;
        }
        [TearDown] // �b�C�Ӵ��դ�k������檺��k
        public void TearDown()
        {
            if (_skipTearDown)
            {
                �ù�1.Quit();
                �ù�2.Quit();
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

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/tpcweb_01_1�ҥ�_bmEnableApply.json"));
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
            await TestHelper.Login(�ù�1, "0511", "password");

            �ù�1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(�ù�1, "111124");

            Thread.Sleep(1000);

            string[] segments = �ù�1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(�ù�2, "0511", "password");
            �ù�2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            �ù�1.SwitchTo().Frame(0);

            var ���z�s�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var ���� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var ���z��� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));

            That(���z�s��.Text, Is.EqualTo("111124"));
            That(����.Text, Is.EqualTo("41101202191"));
            That(���z���.Text, Is.EqualTo("2023�~03��06��"));
        }


        [Test]
        [Order(3)]
        public async Task Twc01_04()
        {
            await TestHelper.Login(�ù�1, "0511", "password");

            �ù�1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(�ù�1, "111124");

            Thread.Sleep(1000);

            string[] segments = �ù�1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(�ù�2, "0511", "password");
            �ù�2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            �ù�1.SwitchTo().Frame(0);

            var �����Ҧr�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input ")));

            �����Ҧr��.SendKeys("A123456789");

            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", �����Ҧr��);

            That(�����Ҧr��.GetAttribute("value"), Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task Twc01_05()
        {
            await TestHelper.Login(�ù�1, "0511", "password");

            �ù�1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(�ù�1, "111124");

            Thread.Sleep(1000);

            string[] segments = �ù�1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(�ù�2, "0511", "password");
            �ù�2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            �ù�1.SwitchTo().Frame(0);

            var ���z = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#���z")));

            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", ���z);

            wait.Until(ExpectedConditions.ElementToBeClickable(���z));

            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", ���z);
            That(���z.Displayed, Is.True);
        }

        [Test]
        [Order(5)]
        public async Task Twc01_06()
        {
            _skipSetup = false;
            _skipTearDown = false;
            await TestHelper.Login(�ù�1, "0511", "password");

            �ù�1.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft");

            TestHelper.ClickRow(�ù�1, "111124");

            Thread.Sleep(1000);

            string[] segments = �ù�1.Url.Split('/');
            string id = segments[segments.Length - 1];

            await TestHelper.Login(�ù�2, "0511", "password");
            �ù�2.Navigate().GoToUrl($@"{TestHelper.LoginUrl}/draft/second-screen/{id}");

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();
            var firstStormTreeNode = stormTreeRoot.FindElement(By.CssSelector("storm-tree-node:first-child"));

            var href = firstStormTreeNode.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));
            Actions actions = new Actions(�ù�1);
            actions.MoveToElement(href).Click().Perform();

            var ���O�ʥΤ��A�ȫ���_�ù�1 = �ù�1.FindElement(By.Id("���O�ʥΤ��A�ȫ���"));
            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", ���O�ʥΤ��A�ȫ���_�ù�1);

            var ���O�ʥΤ��A�ȫ���_�ù�2 = �ù�2.FindElement(By.Id("���O�ʥΤ��A�ȫ���"));
            //�ù�2.SwitchTo().Window(�ù�2.WindowHandles[0]);
            ((IJavaScriptExecutor)�ù�2).ExecuteScript("arguments[0].click();", ���O�ʥΤ��A�ȫ���_�ù�2);

            //�ù�1.SwitchTo().Window(�ù�1.WindowHandles[0]);
            That(���O�ʥΤ��A�ȫ���_�ù�1.GetAttribute("checked"), Is.EqualTo("true"));
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

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();

            var secondStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[1];
            var secondStormTreeNodeShadowRoot = secondStormTreeNode.GetShadowRoot();

            var href = secondStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_2']"));
            Actions actions = new Actions(�ù�1);
            actions.MoveToElement(href).Click().Perform();

            var checkBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("���q�ӤH��ƫO�@�i���ƶ�")));
            var ���q�ӤH��ƫO�@�i���ƶ� = �ù�1.FindElement(By.Id("���q�ӤH��ƫO�@�i���ƶ�"));

            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", ���q�ӤH��ƫO�@�i���ƶ�);
            wait.Until(ExpectedConditions.ElementToBeClickable(���q�ӤH��ƫO�@�i���ƶ�));
            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].click();", ���q�ӤH��ƫO�@�i���ƶ�);
            That(���q�ӤH��ƫO�@�i���ƶ�.GetAttribute("checked"), Is.EqualTo("true"));
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

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"));

            var stormTreeNodes = stormTreeNode[1];
            var stormTreeRoot = stormTreeNodes.GetShadowRoot();

            var thirdStormTreeNode = stormTreeRoot.FindElements(By.CssSelector("storm-tree-node"))[2];
            var thirdStormTreeNodeShadowRoot = thirdStormTreeNode.GetShadowRoot();

            var href = thirdStormTreeNodeShadowRoot.FindElement(By.CssSelector("a[href='#contract_3']"));
            Actions actions = new Actions(�ù�1);
            actions.MoveToElement(href).Click().Perform();

            var checkBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("���q��~���{")));
            var ���q��~���{ = �ù�1.FindElement(By.Id("���q��~���{"));

            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", ���q��~���{);
            wait.Until(ExpectedConditions.ElementToBeClickable(���q��~���{));
            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].click();", ���q��~���{);
            That(���q��~���{.GetAttribute("checked"), Is.EqualTo("true"));
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

            var wait = new WebDriverWait(�ù�1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            // ���� vertical-navigation �i��
            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));

            // ��� tree-view ����
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));

            // ��� tree-node ����
            var stormTreeNode = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];

            // ���o tree-node ������ ShadowRoot
            var stormTreeNodeRoot = stormTreeNode.GetShadowRoot();

            // ��� href ����
            var href = stormTreeNodeRoot.FindElement(By.CssSelector("a[href='#signature']"));
            Actions actions = new Actions(�ù�1);
            actions.MoveToElement(href).Click().Perform();

            var ñ�W = �ù�1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].scrollIntoView(true);", ñ�W);
            wait.Until(ExpectedConditions.ElementToBeClickable(ñ�W));
            ((IJavaScriptExecutor)�ù�1).ExecuteScript("arguments[0].click();", ñ�W);


            var img = �ù�1.FindElement(By.CssSelector("img[src^='data:image/png;']"));
            That(�ù�1.FindElements(By.CssSelector("img[src^='data:image/png;']")).Any(), Is.True);
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

        //        // ���� vertical-navigation �i��
        //        var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));

        //        // ��� card ����
        //        var stormCard = stormVerticalNavigation.GetShadowRoot().FindElements(By.CssSelector("storm-card"))[5];

        //        var �Ұʱ��y�ҥ� = _driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
        //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", �Ұʱ��y�ҥ�);
        //        wait.Until(ExpectedConditions.ElementToBeClickable(�Ұʱ��y�ҥ�));
        //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", �Ұʱ��y�ҥ�);

        //        var img = _driver.FindElement(By.CssSelector("img[src^='data:image/png;']"));
        //        That(_driver.FindElements(By.CssSelector("img[src^='data:image/png;']")).Any(), Is.True);
        //    }
    }

}