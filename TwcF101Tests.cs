using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcF101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcF101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcF101Tests).GetMethod(testMethod);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public async Task TwcF101_01To02()
        {
            await TwcF101_01();
            await TwcF101_02();
        }
        public async Task TwcF101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcF101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTypeChangeApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-F101_bmTypeChangeApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(1)]
        public async Task TwcF101_03To10()
        {
            await TwcF101_03();
            await TwcF101_04();
            await TwcF101_05();
            await TwcF101_06();
            await TwcF101_07();
            await TwcF101_08();
            await TwcF101_09();
            await TwcF101_10();
        }
        public async Task TwcF101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var spanElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-post-user-full-name='']")));

                return spanElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcF101_04()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#用印或代送件只需夾帶附件")));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(driver => checkBox.GetAttribute("checked") == "true");

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcF101_05()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【夾帶附件】或【掃描拍照】未上傳')]")));
            That(hint.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
            Thread.Sleep(1000);

            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '確定')]")));
        }
        public async Task TwcF101_06()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Perform();
        }
        public async Task TwcF101_07()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormcard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案']")));
                return stormcard != null;
            });

            var stormcard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案']")));
            var headline = stormcard.GetShadowRoot().FindElement(By.CssSelector("h5"));

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var input = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='名稱'] input")));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_08()
        {
            var upload = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '上傳')]")));

            _wait.Until(_ =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                return stormEditTable != null;
            });

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_09()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcF101_10()
        {
            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#消費性用水服務契約")));
            _actions.MoveToElement(waterServiceAgreement).Perform();
            That(waterServiceAgreement.GetAttribute("checked"), Is.EqualTo("true"));

            var dataProtectionNotice = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#公司個人資料保護告知事項")));
            _actions.MoveToElement(dataProtectionNotice).Perform();
            That(dataProtectionNotice.GetAttribute("checked"), Is.EqualTo("true"));

            var companyRegulation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#公司營業章程")));
            _actions.MoveToElement(companyRegulation).Perform();
            That(companyRegulation.GetAttribute("checked"), Is.EqualTo("true"));

            var fileName = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']")));
            _actions.MoveToElement(fileName).Perform();
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(1)]
        public async Task TwcF101_11To13()
        {
            await TwcF101_11();
            await TwcF101_12();
            await TwcF101_13();
        }
        public async Task TwcF101_11()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            string formattedApplyDateBegin = "2023-06-03";
            var applyDateBeginInput = TestHelper.FindAndMoveElement(_driver, "//storm-input-group[@label='受理日期起']//input");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveElement(_driver, "//button[text()='查詢']");
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span"), Is.Not.Null);
        }
        public async Task TwcF101_12()
        {
            var applyCaseNo = TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var href = TestHelper.FindNavigationBySpan(_driver, "夾帶附件");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveElement(_driver, "//a[text()='twcweb_01_1_夾帶附件1.pdf']");
            That(attachmentName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_13()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41188239939.pdf", "//button[text()='轉PDF']"), Is.True);
        }
    }
}