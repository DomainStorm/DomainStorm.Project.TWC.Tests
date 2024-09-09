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
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
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

            _driver.SwitchTo().DefaultContent();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//button[contains(text(), '新增文件')]"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var pdfFile = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, pdfFile, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var file = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(file.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

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

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            That(checkBox.Selected);

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
        public async Task TwcS100_02()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//i[text()='logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            _wait.Until(driver =>
            {
                var element = _driver.FindElement(By.CssSelector("h4"));
                return element != null;
            });

            var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            That(button.Text, Is.EqualTo("登入"));
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public async Task TwcS100_03To04()
        {
            await TwcS100_03();
            await TwcS100_04();
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

        [Test]
        [Order(2)]
        public async Task TwcS100_05To10()
        {
            await TwcS100_05();
            await TwcS100_06();
            await TwcS100_07();
            await TwcS100_08();
            await TwcS100_09();
            await TwcS100_10();
        }
        public async Task TwcS100_05()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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
        public async Task TwcS100_06()
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
        public async Task TwcS100_07()
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
        public async Task TwcS100_08()
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
            That(signElement.Text, Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            That(checkBox.Selected);
        }
        public async Task TwcS100_10()
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
        public async Task TwcS100_11()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/search");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card"));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                if (stormTable == null)
                    return false;

                var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return fileRows.Count >= 2;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

            var firstUserName = fileRows.Any(row =>
            {
                var firstUserName = row.FindElement(By.CssSelector("td[data-field='userName'] storm-table-cell span span"));
                return firstUserName.Text == "張博文";
            });
            That(firstUserName, Is.True);

            var secondUserName = fileRows.Any(row =>
            {
                var secondUserName = row.FindElement(By.CssSelector("td[data-field='userName'] storm-table-cell span span"));
                return secondUserName.Text == "謝德威";
            });
            That(secondUserName, Is.True);
            var userNameOne = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr > td[data-field='userName'] > storm-table-cell span"));
            That(userNameOne.GetAttribute("innerText"), Is.EqualTo("張博文"));

            var userNameTwo = stormTable.GetShadowRoot().FindElement(By.CssSelector("tr:nth-child(2) > td[data-field='userName'] > storm-table-cell span"));
            That(userNameTwo.GetAttribute("innerText"), Is.EqualTo("謝德威"));
        }

        [Test]
        [Order(4)]
        public async Task TwcS100_12To13()
        {
            await TwcS100_12();
            await TwcS100_13();
        }
        public async Task TwcS100_12()
        {
            await TestHelper.Login(_driver, "4e03", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var stormDropdown = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-dropdown span")));
            That(stormDropdown.GetAttribute("innerText"), Is.EqualTo("草屯營運所業務股 - 業務員"));
        }
        public async Task TwcS100_13()
        {
            await TestHelper.NavigateAndWait(_driver, "/search");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card"));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-03-06";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
            That(result.Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}