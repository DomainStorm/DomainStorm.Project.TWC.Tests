using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Net.Mail;
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
            Thread.Sleep(1000);

            var discardButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button:nth-child(2)");
            discardButton!.Click();

            var deleteButton = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            That(deleteButton!.Text, Is.EqualTo("刪除"));
        }
        public async Task TwcA101_04()
        {
            var deleteButton = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            deleteButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));

            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "[class='table-pageInfo']");
            That(pageInfo.Text, Is.EqualTo("共 0 筆"));
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

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab!.Text, Is.EqualTo("新增文件"));
        }
        public async Task TwcA101_08()
        {
            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment!.Click();

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input:nth-of-type(3)");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_09()
        {
            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcA101_10()
        {
            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox!.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA101_11()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            var errorMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage!.Text, Is.EqualTo("【受理】未核章"));

            var closeMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            closeMessage!.Click();
        }
        public async Task TwcA101_12()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcA101_13()
        {
            _driver.SwitchTo().DefaultContent();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

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

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(contract_1!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(contract_2!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(contract_3!.GetAttribute("checked"), Is.EqualTo("true"));

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[type='submit']");

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName!.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
    }
}