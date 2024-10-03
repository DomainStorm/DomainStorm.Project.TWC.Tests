using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcG100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcG100Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
                _testHelper = new TestHelper(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public Task TwcG100_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public Task TwcG100_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G100_bmMilitaryApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcG100_03To24()
        {
            TwcG100_03();
            TwcG100_04();
            TwcG100_05();
            TwcG100_06();
            TwcG100_07();
            TwcG100_08();
            TwcG100_09();
            TwcG100_10();
            TwcG100_11();
            TwcG100_12();
            TwcG100_13();
            TwcG100_14();
            TwcG100_15();
            TwcG100_16();
            TwcG100_17();
            TwcG100_18();
            TwcG100_19();
            TwcG100_20();
            TwcG100_21();
            TwcG100_22();
            TwcG100_23();
            TwcG100_24();

            return Task.CompletedTask;
        }
        public Task TwcG100_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));
            _testHelper.OpenNewWindowWithLastSegmentUrlAndVerify();

            return Task.CompletedTask;
        }
        public Task TwcG100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            //_testHelper.InputSendKeys(By.XPath("//span[@sti-trustee-id-no]/input"), "A123456789" + Keys.Tab);
            //var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));
            //That(idElement.GetAttribute("value"), Is.EqualTo("A123456789"));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('focusout'));",
                _testHelper.InputSendKeys(By.XPath("//span[@sti-trustee-id-no]/input"),
                    "A123456789"));

            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _testHelper.WaitElementExists(By.XPath("//span[@id='身分證號碼']"));

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼'][text()='A123456789']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public async Task TwcG100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);

            _testHelper.InputSendKeys(By.XPath("//input[@id='申請電子帳單勾選']"), Keys.Tab);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#申請電子帳單勾選")));
            That(stiApplyEmail.Selected);
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            await Task.Delay(1000);

            stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='申請電子帳單勾選']")));
            That(stiApplyEmail.Selected);
        }
        public async Task TwcG100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='撫卹令或撫卹金分領證書']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiIdentificationChoose);

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='檢附證件']/input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='檢附證件']/input")));
                return stiNote.GetAttribute("value") == "BBB";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='檢附證件']")));
                return stiIdentification != null;
            });

            var stiIdentification = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='檢附證件']")));
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-over-apply]")));
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
        public async Task TwcG100_08()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]/input")));
            stiNoteInput.SendKeys("備註內容" + Keys.Tab);

            //_wait.Until(driver =>
            //{
            //    Thread.Sleep(1000);
            //    var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]/input")));
            //    return stiNote.GetAttribute("value") == "備註內容";
            //});

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            //_wait.Until(driver =>
            //{
            //    Thread.Sleep(1000);
            //    var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]")));
            //    return stiNote != null;
            //});

            _testHelper.WaitElementExists(By.XPath("//span[@sti-note]"));

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note][text()='備註內容']"))), Is.Not.Null);
        }
        public async Task TwcG100_09()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            stiPay.SendKeys(Keys.Tab);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#繳費")));
            That(stiPay.Selected);
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            await Task.Delay(1000);

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='繳費']")));
            That(stiPay.Selected);
        }
        public async Task TwcG100_10()
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
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcG100_11()
        {
            _driver.SwitchTo().DefaultContent();
            _testHelper.SwitchWindowAndClick("//label[@for='消費性用水服務契約']");
        }
        public async Task TwcG100_12()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司個人資料保護告知事項']");
        }
        public async Task TwcG100_13()
        {
            _testHelper.SwitchWindowAndClick("//label[@for='公司營業章程']");
        }
        public async Task TwcG100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//span[text()='簽名']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var signButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='簽名']")));
            _actions.MoveToElement(signButton).Click().Perform();

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));

                return signElement != null;
            });
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));
            That(signElement, Is.Not.Null);
        }
        public async Task TwcG100_15()
        {
            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//span[text()='啟動掃描證件']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var scanButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='啟動掃描證件']")));
            _actions.MoveToElement(scanButton).Click().Perform();

            _wait.Until(driver =>
            {
                var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));

                return imgElement != null;
            });
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//img[@alt='證件_005.tiff']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcG100_16()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf", "twcweb_01_1_夾帶附件2.pdf" }, "input.dz-hidden-input:nth-of-type(3)");
            _testHelper.WaitElementExists(By.CssSelector("storm-edit-table"));

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });
        }
        public async Task TwcG100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button[type='submit']")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='【Email】未填寫或不正確']")));
            That(hint.Text, Is.EqualTo("【Email】未填寫或不正確"));

            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
        }
        public async Task TwcG100_18()
        {
            _driver.SwitchTo().Frame(0);

            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiEmail);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
                return stiEmailInput.GetAttribute("value") == "aaa@bbb.ccc";
            });

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
                return stiEmailTelNoInput.GetAttribute("value") == "02-12345678";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiEmail = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
        public async Task TwcG100_19()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG100_20()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-note]")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcG100_21()
        {
            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='繳費']")));
            That(stiPay.Selected);
        }
        public async Task TwcG100_22()
        {
            var stiEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email]")));
            That(stiEmail.Text, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-email-tel-no]")));
            That(stiEmailTelNo.Text, Is.EqualTo("02-12345678"));
        }
        public async Task TwcG100_23()
        {
            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification-choose]")));
            That(stiIdentificationChoose.Text, Is.EqualTo("撫卹令或撫卹金分領證書"));

            var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification]")));
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_24()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-over-apply]")));
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
    }
}