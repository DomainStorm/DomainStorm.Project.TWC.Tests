using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcB100Tests()
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
        public async Task TwcB100_01To13()
        {
            await TwcB100_01();
            await TwcB100_02();
            await TwcB100_03();
            await TwcB100_04();
            await TwcB100_05();
            await TwcB100_06();
            await TwcB100_07();
            await TwcB100_08();
            await TwcB100_09();
            await TwcB100_10();
            await TwcB100_11();
            await TwcB100_12();
            await TwcB100_13();
        }
        public async Task TwcB100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcB100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B100_bmRecoverApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcB100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);

            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcB100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            
            var 同步前 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            var 同步前2 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _driver.SwitchTo().Frame(0);
            var stiTrusteeIdNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            stiTrusteeIdNoInput.SendKeys("A123456789" + Keys.Tab);

            _driver.SwitchTo().DefaultContent();


            
            //_wait.Until(driver =>
            //{   var 同步後 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            //    ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0];") ,同步前 == 同步後});

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiTrusteeIdNo);
            That(stiTrusteeIdNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcB100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            stiPay.SendKeys(Keys.Tab);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiPay);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='繳費']")));

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }
        public async Task TwcB100_07()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);
            消費性用水服務契約.SendKeys(Keys.Tab);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 消費性用水服務契約);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 消費性用水服務契約);
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB100_08()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);
            公司個人資料保護告知事項.SendKeys(Keys.Tab);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司個人資料保護告知事項);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司個人資料保護告知事項);
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB100_09()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司營業章程);
            公司營業章程.SendKeys(Keys.Tab);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司營業章程);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司營業章程);
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB100_10()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div > div > button.btn-primary")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", signButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", signButton);

            var signSuccess = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div:nth-child(2) > storm-upload > div > div > div:nth-child(2) > div.dz-success-mark")));
            That(signSuccess, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signImg = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] > div.mt-5 > div > form > div:nth-child(2) > storm-upload > div > div > div:nth-child(2) > div.dz-image")));
            That(signImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcB100_11()
        {
            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark")));
            That(scanSuccess, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var scanImg = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='credential'] storm-upload > div > div > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcB100_12()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var firstFile = "twcweb_01_1_夾帶附件1.pdf";
            var firstFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", firstFile);
            lastHiddenInput.SendKeys(firstFilePath);

            lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var secondFile = "twcweb_01_1_夾帶附件2.pdf";
            var secondFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", secondFile);
            lastHiddenInput.SendKeys(secondFilePath);

            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "tr:nth-child(2) > td[data-field='name'] > storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            That(TestHelper.WaitStormEditTableUpload(_driver, "tr:nth-child(2) > td[data-field='name'] > storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));

        }
        public async Task TwcB100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
    }
}