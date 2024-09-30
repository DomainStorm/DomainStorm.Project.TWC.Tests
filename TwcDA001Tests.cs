using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcDA001Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcDA001Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcDA001Tests).GetMethod(testMethod)!;
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
            _driver?.Quit();
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public Task TwcDA001_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcDA001_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-DA001_bmTransferApply.json"), true).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcDA001_03To09()
        {
            TwcDA001_03();
            TwcDA001_04();
            TwcDA001_05();
            TwcDA001_06();
            TwcDA001_07();
            TwcDA001_08();
            TwcDA001_09();

            return Task.CompletedTask;
        }
        public Task TwcDA001_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }
        public Task TwcDA001_04()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            return Task.CompletedTask;
        }
        public Task TwcDA001_05()
        {
            var content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            return Task.CompletedTask;
        }
        public Task TwcDA001_06()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var content = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));
            _wait.Until(_ => content.Text == "張博文");

            return Task.CompletedTask;
        }
        public Task TwcDA001_07()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcDA001_08()
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
        public Task TwcDA001_09()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("a[href='./logout']"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcDA001_10()
        {
            _testHelper.Login("live", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var Taichung = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='台中服務所']")));
            That(Taichung.Text, Is.EqualTo("台中服務所"));

            var Dali = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='大里服務所']")));
            That(Dali.Text, Is.EqualTo("大里服務所"));

            var Caotun = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='草屯營運所']")));
            That(Caotun.Text, Is.EqualTo("草屯營運所"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcDA001_11()
        {
            _testHelper.Login("alarmsue", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var Penghu = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='澎湖營運所']")));
            That(Penghu.Text, Is.EqualTo("澎湖營運所"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(5)]
        public Task TwcDA001_12()
        {
            _testHelper.Login("eugene313", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var taichung = _driver.FindElements(By.XPath("//*[@data-unformatted='台中服務所']"));
            var dali = _driver.FindElements(By.XPath("//*[@data-unformatted='大里服務所']"));
            var caotun = _driver.FindElements(By.XPath("//*[@data-unformatted='草屯營運所']"));
            var penghu = _driver.FindElements(By.XPath("//*[@data-unformatted='澎湖營運所']"));

            That(taichung.Count, Is.EqualTo(0));
            That(dali.Count, Is.EqualTo(0));
            That(caotun.Count, Is.EqualTo(0));
            That(penghu.Count, Is.EqualTo(0));

            return Task.CompletedTask;
        }
    }
}