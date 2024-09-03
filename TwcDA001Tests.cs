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
        public TwcDA001Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcDA001Tests).GetMethod(testMethod);
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
        public async Task TwcDA001_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public async Task TwcDA001_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-DA001_bmTransferApply.json") , true);
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcDA001_03To09()
        {
            await TwcDA001_03();
            await TwcDA001_04();
            await TwcDA001_05();
            await TwcDA001_06();
            await TwcDA001_07();
            await TwcDA001_08();
            await TwcDA001_09();
        }
        public async Task TwcDA001_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
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
        public async Task TwcDA001_04()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcDA001_05()
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
        public async Task TwcDA001_06()
        {
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
        public async Task TwcDA001_07()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(_driver => checkBox.Selected);

            That(checkBox.Selected);
        }
        public async Task TwcDA001_08()
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
        public async Task TwcDA001_09()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            await TestHelper.ChangeUser(_driver, "live");

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-sidenav"));
                return element != null;
            });
        }

        [Test]
        [Order(3)]
        public async Task TwcDA001_10()
        {
            await TestHelper.Login(_driver, "live", TestHelper.Password!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var Taichung = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='台中服務所']")));
            That(Taichung.Text, Is.EqualTo("台中服務所"));
            
            var Dali = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='大里服務所']")));
            That(Dali.Text, Is.EqualTo("大里服務所"));
            
            var Caotun = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='草屯營運所']")));
            That(Caotun.Text, Is.EqualTo("草屯營運所"));
        }

        [Test]
        [Order(4)]
        public async Task TwcDA001_11()
        {
            await TestHelper.Login(_driver, "alarmsue", TestHelper.Password!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var Penghu = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-unformatted='澎湖營運所']")));
            That(Penghu.Text, Is.EqualTo("澎湖營運所"));
        }

        [Test]
        [Order(5)]
        public async Task TwcDA001_12()
        {
            await TestHelper.Login(_driver, "eugene313", TestHelper.Password!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            // 查找所有可能的站所元素
            var taichung = _driver.FindElements(By.XPath("//*[@data-unformatted='台中服務所']"));
            var dali = _driver.FindElements(By.XPath("//*[@data-unformatted='大里服務所']"));
            var caotun = _driver.FindElements(By.XPath("//*[@data-unformatted='草屯營運所']"));
            var penghu = _driver.FindElements(By.XPath("//*[@data-unformatted='澎湖營運所']"));

            // 确保所有站所都不存在
            That(taichung.Count, Is.EqualTo(0));
            That(dali.Count, Is.EqualTo(0));
            That(caotun.Count, Is.EqualTo(0));
            That(penghu.Count, Is.EqualTo(0));
        }
    }
}