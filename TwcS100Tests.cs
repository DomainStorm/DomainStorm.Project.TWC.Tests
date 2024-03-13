using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcS100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcS100_01To02()
        {
            await TwcS100_01();
            await TwcS100_02();
        }
        public async Task TwcS100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);

            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab.Text, Is.EqualTo("新增文件"));

            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcS100_02()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            logout.Click();

            var login = TestHelper.FindAndMoveToElement(_driver, "button");
            That(login.Text, Is.EqualTo("登入"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS100_03To10()
        {
            await TwcS100_03();
            await TwcS100_04();
            await TwcS100_05();
            await TwcS100_06();
            await TwcS100_07();
            await TwcS100_08();
            await TwcS100_09();
            await TwcS100_10();
        }
        public async Task TwcS100_03()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcS100_04()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcS100_05()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);
            Thread.Sleep(1000);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab.Text, Is.EqualTo("新增文件"));
        }
        public async Task TwcS100_06()
        {
            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_07()
        {
            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_08()
        {
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcS100_10()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(2)]
        public async Task TwcS100_11To13()
        {
            await TwcS100_11();
            await TwcS100_12();
            await TwcS100_13();
        }
        public async Task TwcS100_11()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-03-06";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "[headline='綜合查詢'] button");
            search.Click();

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-pageInfo");
            That(pageInfo.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }
        public async Task TwcS100_12()
        {
            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            logout.Click();

            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("4e03");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var login = TestHelper.FindAndMoveToElement(_driver, "button");
            login.Click();

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");
        }
        public async Task TwcS100_13()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-06-17";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "[headline='綜合查詢'] button");
            search.Click();

            That(TestHelper.FindShadowElement(_driver, "stormTable", "p").Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}