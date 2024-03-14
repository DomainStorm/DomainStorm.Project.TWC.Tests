using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcB101Tests()
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
        public async Task TwcB101_01To10()
        {
            await TwcB101_01();
            await TwcB101_02();
            await TwcB101_03();
            await TwcB101_04();
            await TwcB101_05();
            await TwcB101_06();
            await TwcB101_07();
            await TwcB101_08();
            await TwcB101_09();
            await TwcB101_10();
        }
        public async Task TwcB101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcB101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B101_bmRecoverApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task TwcB101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab!.Text, Is.EqualTo("新增文件"));
        }
        public async Task TwcB101_04()
        {
            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment!.Click();

            var attachment1 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment1, "input.dz-hidden-input:nth-of-type(3)");

            var attachment2 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, attachment2, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable","div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";            
            });

            var deleteButton = TestHelper.FindShadowElement(_driver, "stormToolbar", "storm-button");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            confirmButton!.Click();

            _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 1;
            });
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span").Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public async Task TwcB101_05()
        {
            var checkBox = TestHelper.FindAndMoveToElement(_driver, "[id='用印或代送件只需夾帶附件']");
            checkBox!.Click();

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB101_06()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            var errorMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage!.Text, Is.EqualTo("【受理】未核章"));

            var closeMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-actions'] button");
            closeMessage!.Click();
        }
        public async Task TwcB101_07()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[class='swal2-actions'] button")));
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcB101_08()
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

            var applyCaseNo = TestHelper.FindAndMoveToElement(_driver, "[sti-apply-case-no]");
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcB101_09()
        {
            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(contract_1!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(contract_2!.GetAttribute("checked"), Is.EqualTo("true"));

            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(contract_3!.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB101_10()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[type='submit']");

            var attachmentName = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='夾帶附件'] a");
            That(attachmentName!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
    }
}