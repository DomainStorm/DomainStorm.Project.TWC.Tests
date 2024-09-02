using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcA101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcA101Tests).GetMethod(testMethod);
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

        public async Task TwcA101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        [NoBrowser]

        public async Task TwcA101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcA101_03To04()
        {
            await TwcA101_03();
            await TwcA101_04();
        }

        public async Task TwcA101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.Draft(_driver);
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//button[contains(text(), '捨棄草稿')]"));

                if (!element.Displayed)
                {
                    _actions.MoveToElement(element).Perform();
                }

                return element != null;
            });

            var discardButton = _driver.FindElement(By.XPath("//button[contains(text(), '捨棄草稿')]"));
            _actions.MoveToElement(discardButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='是否刪除？']")));

            var deleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '刪除')]")));
            That(deleteButton.Text, Is.EqualTo("刪除"));
        }

        public async Task TwcA101_04()
        {
            var deleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '刪除')]")));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/draft"));

            _wait.Until(_driver =>
            {
                var element = _driver.FindElement(By.CssSelector("storm-table"));
                return element != null;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
            That(result.Text, Is.EqualTo("沒有找到符合的結果"));
        }

        [Test]
        [Order(3)]
        [NoBrowser]
        public async Task TwcA101_05()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(4)]
        [NoBrowser]
        public async Task TwcA101_06()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(5)]
        public async Task TwcA101_07To14()
        {
            await TwcA101_07();
            await TwcA101_08();
            await TwcA101_09();
            await TwcA101_10();
            await TwcA101_11();
            await TwcA101_12();
            await TwcA101_13();
            await TwcA101_14();
        }
        public async Task TwcA101_07()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.Draft(_driver);
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//button[contains(text(), '新增文件')]"));

                if (!element.Displayed)
                {
                    _actions.MoveToElement(element).Perform();
                }

                return element.Displayed ? element : null;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            That(addFileButton!.Displayed, Is.True);
        }
        public async Task TwcA101_08()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_09()
        {
            var upload = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '上傳')]")));

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-edit-table"));

                return element;
            });

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_10()
        {
            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(driver => checkBox.GetAttribute("checked") == "true");

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA101_11()
        {
            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【受理】未核章')]")));
            That(hint.Text, Is.EqualTo("【受理】未核章"));

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '確定')]")));
        }
        public async Task TwcA101_12()
        {
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
        public async Task TwcA101_13()
        {
            _driver.SwitchTo().DefaultContent();

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA101_14()
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
    }
}