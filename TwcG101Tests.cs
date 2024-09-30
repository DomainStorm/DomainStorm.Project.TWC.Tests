using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcG101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcG101Tests).GetMethod(testMethod!);
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
        public Task TwcG101_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcG101_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G101_bmMilitaryApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcG101_03To16()
        {
            TwcG101_03();
            TwcG101_04();
            TwcG101_05();
            TwcG101_06();
            TwcG101_07();
            TwcG101_08();
            TwcG101_09();
            TwcG101_10();
            TwcG101_11();
            TwcG101_12();
            TwcG101_13();
            TwcG101_14();

            return Task.CompletedTask;
        }
        public Task TwcG101_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#繳費")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#繳費")));
            That(stiPay.Selected);

            return Task.CompletedTask;
        }
        public Task TwcG101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-post-user-full-name][text()='張博文']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcG101_05()
        {
            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#申請電子帳單勾選")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#申請電子帳單勾選")));
            That(stiApplyEmail.Selected);

            return Task.CompletedTask;
        }
        public Task TwcG101_06()
        {
            var pensionCertificateSelect = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[value='撫卹令或撫卹金分領證書']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", pensionCertificateSelect);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", pensionCertificateSelect);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("input[value='撫卹令或撫卹金分領證書']")));
            That(pensionCertificateSelect.Selected);

            _testHelper.InputSendKeys(By.CssSelector("span[sti-identification] input"), "BBB" + Keys.Tab);

            _wait.Until(ExpectedConditions.TextToBePresentInElementValue(By.CssSelector("span[sti-identification] input"), "BBB"));

            return Task.CompletedTask;
        }
        public Task TwcG101_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-over-apply] input[id='超戶申請group2']")));
            That(stiOverApply.Selected);

            return Task.CompletedTask;
        }
        public Task TwcG101_08()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcG101_09()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【夾帶附件】或【掃描拍照】未上傳')]")));
            That(hint.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            _testHelper.ElementClick(By.XPath("//button[text()='確定']"));

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));

            return Task.CompletedTask;
        }
        public Task TwcG101_10()
        {
            _driver.SwitchTo().Frame(0);

            _testHelper.InputSendKeys(By.CssSelector("span[sti-email] input"), "aaa@bbb.ccc" + Keys.Tab);

            _wait.Until(ExpectedConditions.TextToBePresentInElementValue(By.CssSelector("span[sti-email] input"), "aaa@bbb.ccc"));

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email] input")));
            That(stiEmailInput.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            _testHelper.InputSendKeys(By.CssSelector("span[sti-email-tel-no] input"), "02-12345678" + Keys.Tab);

            _wait.Until(ExpectedConditions.TextToBePresentInElementValue(By.CssSelector("span[sti-email-tel-no] input"), "02-12345678"));

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email-tel-no] input")));
            That(stiEmailTelNoInput.GetAttribute("value"), Is.EqualTo("02-12345678"));

            return Task.CompletedTask;
        }
        public Task TwcG101_11()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }
        public Task TwcG101_12()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));

            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            return Task.CompletedTask;
        }
        public Task TwcG101_13()
        {
            var content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            return Task.CompletedTask;
        }
        public Task TwcG101_14()
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
        public Task TwcG101_15To16()
        {
            TwcG101_15();
            TwcG101_16();

            return Task.CompletedTask;
        }
        public Task TwcG101_15()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/unfinished", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.MoveAndCheck("#消費性用水服務契約");
            _testHelper.MoveAndCheck("#公司個人資料保護告知事項");
            _testHelper.MoveAndCheck("#公司營業章程");
            _testHelper.CheckElementText("a[download='twcweb_01_1_夾帶附件1.pdf']", "twcweb_01_1_夾帶附件1.pdf");

            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _driver.FindElement(By.CssSelector("#申請電子帳單勾選"));
            That(stiApplyEmail.Selected);

            var stiPay = _driver.FindElement(By.CssSelector("#繳費"));
            That(stiPay.Selected);

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-post-user-full-name][text()='張博文']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcG101_16()
        {
            var checkStiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件']")));
            That(checkStiIdentification.Text, Is.EqualTo("BBB"));

            var checkStiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='超戶申請radio']")));
            That(checkStiOverApply.Text, Is.EqualTo("否"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public Task TwcG101_17To19()
        {
            TwcG101_17();
            TwcG101_18();
            TwcG101_19();

            return Task.CompletedTask;
        }
        public Task TwcG101_17()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/search", By.CssSelector("storm-card"));
            _testHelper.WaitElementExists(By.XPath("//button[text()='查詢']"));

            var applyDateBegin = "2023-06-13";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            _testHelper.ElementClick(By.XPath("//button[text()='查詢']"));

            var applyCaseNo = _wait.Until(_driver =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
                return applyCaseNoElements.FirstOrDefault(element => element.Text == TestHelper.ApplyCaseNo);
            });

            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }
        public Task TwcG101_18()
        {
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.CheckElementText("a[download='twcweb_01_1_夾帶附件1.pdf']", "twcweb_01_1_夾帶附件1.pdf");

            return Task.CompletedTask;
        }
        public Task TwcG101_19()
        {
            _testHelper.DownloadFileAndVerify("41105533310.pdf", "//button[text()='轉PDF']");

            return Task.CompletedTask;
        }
    }
}