using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
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
        public async Task TwcDA001_01To02()
        {
            await TwcDA001_01();
            await TwcDA001_02();
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

        [Test]
        [Order(1)]
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
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var href = TestHelper.FindNavigationBySpan(_driver, "夾帶附件");
            _actions.MoveToElement(href).Click().Perform();
        }
        public async Task TwcDA001_04()
        {
            var addAttachment = TestHelper.FindAndMoveElement(_driver, "//button[text()='新增文件']");
            _actions.MoveToElement(addAttachment).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");
        }
        public async Task TwcDA001_05()
        {
            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf";
            });

            var upload = TestHelper.FindAndMoveElement(_driver, "//button[text()='上傳']");
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='上傳']")));

            _wait.Until(driver =>
            {
                var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo");
                return pageInfo!.Text == "顯示第 1 至 1 筆，共 1 筆";
            });
        }
        public async Task TwcDA001_06()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));

                return signElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcDA001_07()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindNavigationBySpan(_driver, "受理登記");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "//input[@id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcDA001_08()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "//button[text()='確認受理']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確認受理']")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveElement(_driver, "//span[@sti-apply-case-no]");
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcDA001_09()
        {
            _driver.SwitchTo().DefaultContent();

            var logout = TestHelper.FindAndMoveElement(_driver, "//i[text()='logout']");
            _actions.MoveToElement(logout).Click().Perform();
        }

        [Test]
        [Order(2)]
        public async Task TwcDA001_10()
        {
            await TestHelper.Login(_driver, "live", TestHelper.Password!);
            var iframeElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(iframeElement);

            var Taichung = TestHelper.FindAndMoveElement(_driver, "//*[@data-unformatted='台中服務所']");
            That(Taichung.Text, Is.EqualTo("台中服務所"));
            
            var Dali = TestHelper.FindAndMoveElement(_driver, "//*[@data-unformatted='大里服務所']");
            That(Dali.Text, Is.EqualTo("大里服務所"));
            
            var Caotun = TestHelper.FindAndMoveElement(_driver, "//*[@data-unformatted='草屯營運所']");
            That(Caotun.Text, Is.EqualTo("草屯營運所"));
        }

        [Test]
        [Order(3)]
        public async Task TwcDA001_11()
        {
            await TestHelper.Login(_driver, "alarmsue", TestHelper.Password!);
            var iframeElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(iframeElement);

            var Penghu = TestHelper.FindAndMoveElement(_driver, "//*[@data-unformatted='澎湖營運所']");
            That(Penghu.Text, Is.EqualTo("澎湖營運所"));
        }

        [Test]
        [Order(4)]
        public async Task TwcDA001_12()
        {
            await TestHelper.Login(_driver, "eugene313", TestHelper.Password!);
            var iframeElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(iframeElement);

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