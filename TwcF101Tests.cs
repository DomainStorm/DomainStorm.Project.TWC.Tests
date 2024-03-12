using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
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
        public async Task TwcF101_01To10()
        {
            await TwcF101_01();
            await TwcF101_02();
            await TwcF101_03();
            await TwcF101_04();
            await TwcF101_05();
            await TwcF101_06();
            await TwcF101_07();
            await TwcF101_08();
            await TwcF101_09();
            await TwcF101_10();
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
        public async Task TwcF101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcF101_04()
        {
            _driver.SwitchTo().DefaultContent();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcF101_05()
        {
            var submitButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='受理登記'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcF101_06()
        {

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.swal2-actions button");
            _actions.MoveToElement(confirmButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-actions button")));

            var pageInfo = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("共 0 筆"));
        }
        public async Task TwcF101_07()
        {
            var createAttachmentButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='file'] button");
            _actions.MoveToElement(createAttachmentButton).Click().Perform();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_08()
        {
            var submitButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增檔案'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增檔案'] button")));
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_09()
        {
            var href = TestHelper.FindShadowRootElement(_driver, "[href='#finished']");
            _actions.MoveToElement(href).Click().Perform();

            var checkButton = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");

            var submitButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='受理登記'] button");
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcF101_10()
        {
            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", chceckSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);

            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
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

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起']")));
            var applyDateBeginInput = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));

            string formattedApplyDateBegin = "2023-06-03";
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedApplyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginInput);

            var search = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='綜合查詢'] button");
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span"), Is.Not.Null);
        }
        public async Task TwcF101_12()
        {
             var applyCaseNo = TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] span");
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var href = TestHelper.FindShadowRootElement(_driver, "[href='#file']");
            _actions.MoveToElement(href).Click().Perform();

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcF101_13()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41188239939.pdf", "storm-card[id='finished'] button"), Is.True);
        }
    }
}