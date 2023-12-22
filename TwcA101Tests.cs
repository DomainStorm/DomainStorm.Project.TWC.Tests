using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
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
        public async Task TwcA101_01To04()
        {
            await TwcA101_01();
            await TwcA101_02();
            await TwcA101_03();
            await TwcA101_04();
        }
        public async Task TwcA101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcA101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcA101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var abandonButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='finished'] button.bg-gradient-danger");
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", abandonButton);

            var confirmButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-actions button.swal2-confirm")));
            That(confirmButton.Text, Is.EqualTo("刪除"));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", confirmButton);
        }
        public async Task TwcA101_04()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-actions button.swal2-confirm")));

            var pageInfo = TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }

        [Test]
        [Order(1)]
        public async Task TwcA101_05To14()
        {
            await TwcA101_05();
            await TwcA101_06();
            await TwcA101_07();
            await TwcA101_08();
            await TwcA101_09();
            await TwcA101_10();
            await TwcA101_11();
            await TwcA101_12();
            await TwcA101_13();
            await TwcA101_14();
        }
        public async Task TwcA101_05()
        {
            await TwcA101_01();
        }
        public async Task TwcA101_06()
        {
            await TwcA101_02();
        }
        public async Task TwcA101_07()
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
        public async Task TwcA101_08()
        {
            var createAttachmentButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();
            
            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_09()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_10()
        {
            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkBox = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkBox).Click().Perform();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA101_11()
        {
            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info");
            _actions.MoveToElement(submitButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > h5")));
            That(hintTitle.Text, Is.EqualTo("【受理】未核章"));

            var confirmButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(confirmButton).Click().Perform();
        }
        public async Task TwcA101_12()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signCheck = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(signCheck!, Is.Not.Null);
        }
        public async Task TwcA101_13()
        {
            _driver.SwitchTo().DefaultContent();

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
        public async Task TwcA101_14()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_1'] > div.d-flex > div.form-check > input")));
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_2'] > div.d-flex > div.form-check > input")));
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_3'] > div.d-flex > div.form-check > input")));
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(attachmentName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
    }
}