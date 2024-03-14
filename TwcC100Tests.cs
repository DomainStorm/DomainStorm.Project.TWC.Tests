using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcC100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcC100Tests()
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
        public async Task TwcC100_01To13()
        {
            await TwcC100_01();
            await TwcC100_02();
            await TwcC100_03();
            await TwcC100_04();
            await TwcC100_05();
            await TwcC100_06();
            await TwcC100_07();
            await TwcC100_08();
            await TwcC100_09();
            await TwcC100_10();
            await TwcC100_11();
            await TwcC100_12();
            await TwcC100_13();
        }
        public async Task TwcC100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcC100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C100_bmDisableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcC100_03()
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

            var applyCaseNo = TestHelper.FindAndMoveToElement(_driver, "[sti-apply-case-no]");
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcC100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);
            Thread.Sleep(1000);

            idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            var idNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no]")));
            That(idNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcC100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcC100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "[id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().DefaultContent();

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_1);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            That(contract_1.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC100_08()
        {
            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "[id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_2);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            That(contract_2.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC100_09()
        {
            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "[id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_3);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            That(contract_3.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC100_10()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signature = TestHelper.FindAndMoveToElement(_driver, "[id='signature'] button:nth-child(2)");
            signature!.Click();

            var img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");
            That(img, Is.Not.Null);
        }
        public async Task TwcC100_11()
        {
            var scanButton = TestHelper.FindAndMoveToElement(_driver, "[headline='掃描拍照'] button:nth-child(2)");
            scanButton!.Click();

            var scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcC100_12()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab!.Text, Is.EqualTo("新增文件"));

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
            upload.Click();

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });
        }
        public async Task TwcC100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

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
    }
}