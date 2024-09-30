using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcS100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcS100Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
                _testHelper = new TestHelper(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public Task TwcS100_01To02()
        {
            TwcS100_01();
            TwcS100_02();

            return Task.CompletedTask;
        }
        public Task TwcS100_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _testHelper.WaitElementExists(By.CssSelector("span[sti-post-user-full-name='']"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("張博文"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }
        public Task TwcS100_02()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.XPath("//i[text()='logout']"));
            _testHelper.WaitElementExists(By.CssSelector("h4"));

            var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            That(button.Text, Is.EqualTo("登入"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcS100_03To04()
        {
            TwcS100_03();
            TwcS100_04();

            return Task.CompletedTask;
        }
        public Task TwcS100_03()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }
        public Task TwcS100_04()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcS100_05To10()
        {
            TwcS100_05();
            TwcS100_06();
            TwcS100_07();
            TwcS100_08();
            TwcS100_09();
            TwcS100_10();

            return Task.CompletedTask;
        }
        public Task TwcS100_05()
        {
            _testHelper.Login("tw491", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }
        public Task TwcS100_06()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            return Task.CompletedTask;
        }
        public Task TwcS100_07()
        {
            var content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            return Task.CompletedTask;
        }
        public Task TwcS100_08()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _testHelper.WaitElementExists(By.CssSelector("span[sti-post-user-full-name='']"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(content.Text, Is.EqualTo("謝德威"));

            return Task.CompletedTask;
        }
        public Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcS100_10()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcS100_11()
        {
            _testHelper.Login("tw491", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                if (stormTable == null)
                    return false;

                var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return fileRows.Count == 2;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

            var userNames = fileRows.Select(row =>
            {
                var userNameElement = row.FindElement(By.CssSelector("td[data-field='userName'] storm-table-cell span span"));
                return userNameElement.Text;
            }).ToList();

            That(userNames.Contains("張博文"), Is.True);
            That(userNames.Contains("謝德威"), Is.True);

            var firstUserName = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(1) > td[data-field='userName'] > storm-table-cell span"));
            var secondUserName = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(2) > td[data-field='userName'] > storm-table-cell span"));
            var names = new List<string> { firstUserName.GetAttribute("innerText"), secondUserName.GetAttribute("innerText") };

            That(names.Contains("張博文"), Is.True);
            That(names.Contains("謝德威"), Is.True);

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcS100_12To13()
        {
            TwcS100_12();
            TwcS100_13();

            return Task.CompletedTask;
        }
        public Task TwcS100_12()
        {
            _testHelper.Login("4e03", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-sidenav"));

            var stormDropdown = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-dropdown span")));
            That(stormDropdown.GetAttribute("innerText"), Is.EqualTo("草屯營運所業務股 - 業務員"));

            return Task.CompletedTask;
        }
        public async Task TwcS100_13()
        {
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-table"));

            var content = _testHelper.WaitShadowElement("p", "沒有找到符合的結果");
            That(content.Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}
