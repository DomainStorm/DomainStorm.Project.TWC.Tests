using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcG101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcG101Tests).GetMethod(testMethod);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public async Task TwcG101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        [NoBrowser]
        public async Task TwcG101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G101_bmMilitaryApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcG101_03To16()
        {
            await TwcG101_03();
            await TwcG101_04();
            await TwcG101_05();
            await TwcG101_06();
            await TwcG101_07();
            await TwcG101_08();
            await TwcG101_09();
            await TwcG101_10();
            await TwcG101_11();
            await TwcG101_12();
            await TwcG101_13();
            await TwcG101_14();
        }
        public async Task TwcG101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#繳費")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#繳費")));

            That(stiPay.Selected);
        }
        public async Task TwcG101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signElement = _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("span[sti-post-user-full-name='']"));
                return element.Displayed ? element : null;
            });

            That(signElement!.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcG101_05()
        {
            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#申請電子帳單勾選")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#申請電子帳單勾選")));

            That(stiApplyEmail.Selected);
        }
        public async Task TwcG101_06()
        {
            var pensionCertificateSelect = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[value='撫卹令或撫卹金分領證書']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", pensionCertificateSelect);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", pensionCertificateSelect);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("input[value='撫卹令或撫卹金分領證書']")));

            That(pensionCertificateSelect.Selected);

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-identification] input")));
            stiIdentificationInput.SendKeys("BBB");

            _wait.Until(driver =>
            {
                var element = _driver.FindElement(By.CssSelector("span[sti-identification] input"));
                return element.GetAttribute("value") == "BBB";
            });

            That(stiIdentificationInput.GetAttribute("value"), Is.EqualTo("BBB"));
        }
        public async Task TwcG101_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-over-apply] input[id='超戶申請group2']")));
            That(stiOverApply.Selected);
        }
        public async Task TwcG101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var checkBox = _driver.FindElement(By.CssSelector("#用印或代送件只需夾帶附件"));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            That(checkBox.Selected);
        }
        public async Task TwcG101_09()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【夾帶附件】或【掃描拍照】未上傳')]")));
            That(hint.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));

            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '確定')]")));
        }
        public async Task TwcG101_10()
        {
            _driver.SwitchTo().Frame(0);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email] input")));
            stiEmailInput.SendKeys("aaa@bbb.ccc" + Keys.Tab);
            //等待畫面渲染
            await Task.Delay(1000);

            _wait.Until(ExpectedConditions.TextToBePresentInElementValue(By.CssSelector("span[sti-email] input"), "aaa@bbb.ccc"));

            stiEmailInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email] input")));
            That(stiEmailInput.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email-tel-no] input")));
            stiEmailTelNoInput.SendKeys("02-12345678" + Keys.Tab);
            await Task.Delay(1000);

            _wait.Until(ExpectedConditions.TextToBePresentInElementValue(By.CssSelector("span[sti-email-tel-no] input"), "02-12345678"));

            stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email-tel-no] input")));
            That(stiEmailTelNoInput.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }
        public async Task TwcG101_11()
        {
            _driver.SwitchTo().DefaultContent();

            _wait.Until(_driver =>
            {
                var element = _driver.FindElement(By.XPath("//button[contains(text(), '新增文件')]"));

                if (!element.Displayed)
                {
                    _actions.MoveToElement(element).Perform();
                }

                return element.Displayed ? element : null;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            That(addFileButton!.Displayed, Is.True);
        }
        public async Task TwcG101_12()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_13()
        {
            var upload = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '上傳')]")));

            _wait.Until(_ =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                return stormEditTable != null;
            });

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_14()
        {
            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(3)]
        public async Task TwcG101_15To16()
        { 
            await TwcG101_15();
            await TwcG101_16();
        }
        public async Task TwcG101_15()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/unfinished");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = _driver.FindElement(By.CssSelector("#消費性用水服務契約"));
            _actions.MoveToElement(waterServiceAgreement).Perform();
            That(waterServiceAgreement.Selected);

            var dataProtectionNotice = _driver.FindElement(By.CssSelector("#公司個人資料保護告知事項"));
            _actions.MoveToElement(dataProtectionNotice).Perform();
            That(dataProtectionNotice.Selected);

            var companyRegulation = _driver.FindElement(By.CssSelector("#公司營業章程"));
            _actions.MoveToElement(companyRegulation).Perform();
            That(companyRegulation.Selected);

            var fileName = _driver.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            _actions.MoveToElement(fileName).Perform();
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _driver.FindElement(By.CssSelector("#申請電子帳單勾選"));
            That(stiApplyEmail.Selected);

            var stiPay = _driver.FindElement(By.CssSelector("#繳費"));
            That(stiPay.Selected);

            var stiPostUserFullName = _driver.FindElement(By.CssSelector("span[sti-post-user-full-name]"));
            That(stiPostUserFullName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcG101_16()
        {
            var checkStiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件']")));
            That(checkStiIdentification.Text, Is.EqualTo("BBB"));

            var checkStiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio']")));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='超戶申請radio']")));
            That(checkStiOverApply.Text, Is.EqualTo("否"));
        }

        [Test]
        [Order(4)]
        public async Task TwcG101_17To19()
        {
            await TwcG101_17();
            await TwcG101_18();
            await TwcG101_19();
        }
        public async Task TwcG101_17()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/search");

            _wait.Until(_ =>
            {
                var stormCard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card")));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-06-13";
            var applyDateBeginSelect = _driver.FindElement(By.CssSelector("storm-input-group[label='受理日期起'] input"));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            var applyCaseNo = _wait.Until(_driver =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
                return applyCaseNoElements.FirstOrDefault(element => element.Text == TestHelper.ApplyCaseNo);
            });

            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG101_18()
        {
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().DefaultContent();

            _wait.Until(driver =>
            {
                var element = _driver.FindElement(By.CssSelector("#file"));

                if (!element.Displayed)
                {
                    _actions.MoveToElement(element).Perform();
                }

                return element.Displayed ? element : null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']")));
            _actions.MoveToElement(fileName).Perform();

            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_19()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41105533310.pdf", "//button[text()='轉PDF']"), Is.True);
        }
    }
}