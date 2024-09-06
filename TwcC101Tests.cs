using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcC101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcC101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcC101Tests).GetMethod(testMethod);
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
        public async Task TwcC101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public async Task TwcC101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C101_bmDisableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcC101_03To07()
        {
            await TwcC101_03();
            await TwcC101_04();
            await TwcC101_05();
            await TwcC101_06();
            await TwcC101_07();
        }
        public async Task TwcC101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(_driver =>
            {
                var spanElement = _driver.FindElement(By.CssSelector("span[sti-post-user-full-name='']"));

                return spanElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcC101_04()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));
             
            That(checkBox.Selected);
        }
        public async Task TwcC101_05()
        {
            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【夾帶附件】或【掃描拍照】未上傳')]")));
            That(hint.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '確定')]")));
        }
        public async Task TwcC101_06()
        {
            var scanButton = _driver.FindElement(By.XPath("//span[text()='啟動掃描證件']"));
            _actions.MoveToElement(scanButton).Click().Perform();

            _wait.Until(driver =>
            {
                var imgElement = _driver.FindElement(By.CssSelector("img[alt='證件_005.tiff']"));

                return imgElement != null;
            });

            var imgElement = _driver.FindElement(By.XPath("//img[@alt='證件_005.tiff']"));
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcC101_07()
        {
            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(3)]
        public async Task TwcC101_08()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/unfinished");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = _driver.FindElement(By.CssSelector("#消費性用水服務契約"));
            _actions.MoveToElement(waterServiceAgreement).Perform();
            That(waterServiceAgreement.Selected);

            var dataProtectionNotice = _driver.FindElement(By.CssSelector("#公司個人資料保護告知事項"));
            _actions.MoveToElement(dataProtectionNotice).Perform();
            That(dataProtectionNotice.Selected);

            var companyRegulation = _driver.FindElement(By.CssSelector("#公司營業章程"));
            _actions.MoveToElement(companyRegulation).Perform();
            That(companyRegulation.Selected);

            var imgElement = _driver.FindElement(By.XPath("//storm-card[position()=6]//img"));
            _actions.MoveToElement(imgElement).Perform();
            That(imgElement, Is.Not.Null);
        }

        [Test]
        [Order(4)]
        public async Task TwcC101_09To11()
        {
            await TwcC101_09();
            await TwcC101_10();
            await TwcC101_11();
        }
        public async Task TwcC101_09()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/search");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card"));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-06-03";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            //var applyDateBeginSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起'] input")));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            var applyCaseNo = _wait.Until(_driver =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
                return applyCaseNoElements.FirstOrDefault(element => element.Text == TestHelper.ApplyCaseNo);
            });

            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcC101_10()
        {
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);
            //var stormtable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            //var applyCaseNo = stormtable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='applyCaseNo'] span"));
            //_actions.MoveToElement(applyCaseNo).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().DefaultContent();

            var imgElement = _driver.FindElement(By.XPath("//storm-card[position()=6]//img"));
            _actions.MoveToElement(imgElement).Perform();
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcC101_11()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41101699338.pdf", "//button[text()='轉PDF']"), Is.True);
        }
    }
}