using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
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
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcDA001_01To12()
        {
            await TwcDA001_01();
            await TwcDA001_02();
            await TwcDA001_03();
            await TwcDA001_04();
            await TwcDA001_05();
            await TwcDA001_06();
            await TwcDA001_07();
            await TwcDA001_08();
            await TwcDA001_09();
        }
        public async Task TwcDA001_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcDA001_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-DA001_bmTransferApply.json") , true);
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcDA001_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }
        public async Task TwcDA001_04()
        {
            var createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachmentOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachmentOne, "input.dz-hidden-input:nth-of-type(3)");
        }
        public async Task TwcDA001_05()
        {
            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf";
            });

            var submitButton = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增檔案'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("storm-card[headline='新增檔案'] button")));

            _wait.Until(driver =>
            {
                var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo");
                return pageInfo!.Text == "顯示第 1 至 1 筆，共 1 筆";
            });
        }
        public async Task TwcDA001_06()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcDA001_07()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcDA001_08()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcDA001_09()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("live");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
        }
        public async Task TwcDA001_10()
        {
            var formatTextOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("text[data-unformatted='台中服務所']")));
            That(formatTextOne.Text, Is.EqualTo("台中服務所"));

            var formatTextTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("text[data-unformatted='大里服務所']")));
            That(formatTextTwo.Text, Is.EqualTo("大里服務所"));

            var formatTextThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("text[data-unformatted='草屯營運所']")));
            That(formatTextThree.Text, Is.EqualTo("草屯營運所"));
        }
        public async Task TwcDA001_11()
        {
            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("alarmsue");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var formatText = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("text[data-unformatted='澎湖營運所']")));
            That(formatText.Text, Is.EqualTo("澎湖營運所"));
        }
        public async Task TwcDA001_12()
        {
            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _actions.MoveToElement(logout).Click().Perform();

            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
            usernameElement.SendKeys("eugene313");

            var passwordElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
            passwordElement.SendKeys(TestHelper.Password!);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[icon='leaderboard']")));
        }
    }
}