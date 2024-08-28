using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcD100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcD100Tests()
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
        public async Task TwcD100_01To02()
        {
            await TwcD100_01();
            await TwcD100_02();
        }
        public async Task TwcD100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcD100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmAolishedApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-D100_bmAolishedApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(1)]
        public async Task TwcD100_03To16()
        {
            await TwcD100_03();
            await TwcD100_04();
            await TwcD100_05();
            await TwcD100_06();
            await TwcD100_07();
            await TwcD100_08();
            await TwcD100_09();
            await TwcD100_10();
            await TwcD100_11();
            await TwcD100_12();
            await TwcD100_13();
            await TwcD100_14();
            await TwcD100_15();
            await TwcD100_16();
        }
        public async Task TwcD100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveElement(_driver, "//span[@sti-apply-case-no]");
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcD100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-trustee-id-no]/input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);

            _wait.Until(driver =>
            {
                var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));

                return idElement.GetAttribute("value") == "A123456789";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            await Task.Delay(500);

            var idElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼']")));
            That(idElement.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcD100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]/input")));
            stiNoteInput.SendKeys("備註內容" + Keys.Tab);

            _wait.Until(driver =>
            {
                var stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-note]/input")));

                return stiNote.GetAttribute("value") == "備註內容";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='備註內容']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcD100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            _wait.Until(driver =>
            {
                var checkbox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='中結']")));

                return checkbox.GetAttribute("checked") != null;
            });

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _wait.Until(driver =>
            {
                var checkbox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='繳費']")));

                return checkbox.GetAttribute("checked") != null;
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var checkStiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            That(checkStiEnd.GetAttribute("checked"), Is.EqualTo("true"));

            var checkStiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            That(checkStiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));

                return signElement != null;
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcD100_08()
        {
            TestHelper.ClickElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 0);
        }
        public async Task TwcD100_09()
        {
            TestHelper.ClickElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 0);
        }
        public async Task TwcD100_10()
        {
            TestHelper.ClickElementInWindow(_driver, "//label[@for='公司營業章程']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司營業章程']", 0);
        }
        public async Task TwcD100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signButton = TestHelper.FindAndMoveElement(_driver, "//span[text()='簽名']");
            _actions.MoveToElement(signButton).Click().Perform();

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));

                return signElement != null;
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));
            That(signElement, Is.Not.Null);
        }
        public async Task TwcD100_12()
        {
            var scanButton = TestHelper.FindAndMoveElement(_driver, "//span[text()='啟動掃描證件']");
            _actions.MoveToElement(scanButton).Click().Perform();

            _wait.Until(driver =>
            {
                var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));

                return imgElement != null;
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));
            That(imgElement, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcD100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var addAttachment = TestHelper.FindAndMoveElement(_driver, "//button[text()='新增文件']");
            _actions.MoveToElement(addAttachment).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var upload = TestHelper.FindAndMoveElement(_driver, "//button[text()='上傳']");
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='上傳']")));

            var fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
            That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
            That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }
        public async Task TwcD100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var submitButton = TestHelper.FindAndMoveElement(_driver, "//button[text()='確認受理']");
            _actions.MoveToElement(submitButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveElement(_driver, "//span[@sti-apply-case-no]");
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcD100_15()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='備註內容']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcD100_16()
        {
            _wait.Until(driver =>
            {
                var checkbox = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));

                return checkbox.GetAttribute("checked") != null;
            });

            _wait.Until(driver =>
            {
                var checkbox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='繳費']")));

                return checkbox.GetAttribute("checked") != null;
            });
        }
    }
}