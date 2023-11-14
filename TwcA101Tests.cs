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

            var abandonButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='finished'] > div.float-end > div:nth-child(3) > button.bg-gradient-danger");
            _actions.MoveToElement(abandonButton).Click().Perform();

            var deleteButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-actions > button.swal2-confirm")));
            That(deleteButton.Text, Is.EqualTo("刪除"));
        }
        public async Task TwcA101_04()
        {
            var deleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.swal2-actions > button.swal2-confirm")));
            deleteButton.Click();

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle.Text, Is.EqualTo("沒有找到符合的結果"));
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

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcA101_08()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            var fileName = stormInputGroup.GetAttribute("value");
            That(fileName, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_09()
        {
            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_10()
        {
            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();

            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA101_11()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-html-container > h5")));
            That(hintTitle.Text, Is.EqualTo("【受理】未核章"));
        }
        public async Task TwcA101_12()
        {
            var confirmButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(confirmButton).Click().Perform();

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");
        }
        public async Task TwcA101_13()
        {
            _driver.SwitchTo().DefaultContent();

            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");

            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA101_14()
        {
            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(消費性用水服務契約).Click().Perform();

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司營業章程).Click().Perform();

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
    }
}