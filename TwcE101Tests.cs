﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcE101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcE101Tests).GetMethod(testMethod);
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
        public async Task TwcE101_01To02()
        {
            await TwcE101_01();
            await TwcE101_02();
        }
        public async Task TwcE101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcE101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E101_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(1)]
        public async Task TwcE101_03To12()
        {
            await TwcE101_03();
            await TwcE101_04();
            await TwcE101_05();
            await TwcE101_06();
            await TwcE101_07();
            await TwcE101_08();
            await TwcE101_09();
            await TwcE101_10();
            await TwcE101_11();
            await TwcE101_12();
        }
        public async Task TwcE101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#中結")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);

            _wait.Until(driver => stiEnd.GetAttribute("checked") == "true");

            var checkStiEnd = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#中結")));
            That(checkStiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var spanElement = _driver.FindElement(By.CssSelector("span[sti-post-user-full-name='']"));

                return spanElement != null;
            });

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }

        public async Task TwcE101_05()
        {
            _driver.SwitchTo().DefaultContent();

            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Perform();
        }

        public async Task TwcE101_06()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '新增文件')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormcard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案']")));
                return stormcard != null;
            });

            var stormcard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案']")));
            var headline = stormcard.GetShadowRoot().FindElement(By.CssSelector("h5"));

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var input = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='名稱'] input")));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_07()
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

        public async Task TwcE101_08()
        {
            var checkBox = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#用印或代送件只需夾帶附件")));
            _actions.MoveToElement(checkBox).Click().Perform();

            _wait.Until(driver => checkBox.GetAttribute("checked") == "true");

            That(checkBox.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_09()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '【聯絡電話】未填寫')]")));
            That(hint.Text, Is.EqualTo("【聯絡電話】未填寫"));
            Thread.Sleep(1000);

            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[contains(text(), '確定')]")));
        }

        public async Task TwcE101_10()
        {
            _driver.SwitchTo().Frame(0);

            var phoneInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email-tel-no] input")));
            phoneInput.SendKeys("02-12345678");

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var phoneInput = driver.FindElement(By.CssSelector("span[sti-email-tel-no] input"));
                return phoneInput.GetAttribute("value") == "02-12345678";
            });

            var phoneElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-email-tel-no] input")));
            That(phoneElement.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }

        public async Task TwcE101_11()
        {
            _driver.SwitchTo().DefaultContent();

            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[contains(text(), '確認受理')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            //function url 後 基礎元素出來 

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        public async Task TwcE101_12()
        {
            var phone = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-email-tel-no]")));
            That(phone.Text, Is.EqualTo("02-12345678"));

            _driver.SwitchTo().DefaultContent();

            var waterServiceAgreement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#消費性用水服務契約")));
            _actions.MoveToElement(waterServiceAgreement).Perform();
            That(waterServiceAgreement.GetAttribute("checked"), Is.EqualTo("true"));

            var dataProtectionNotice = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#公司個人資料保護告知事項")));
            _actions.MoveToElement(dataProtectionNotice).Perform();
            That(dataProtectionNotice.GetAttribute("checked"), Is.EqualTo("true"));

            var companyRegulation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#公司營業章程")));
            _actions.MoveToElement(companyRegulation).Perform();
            That(companyRegulation.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(2)]
        public async Task TwcE101_13To15()
        {
            await TwcE101_13();
            await TwcE101_14();
            await TwcE101_15();
        }
        public async Task TwcE101_13()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            _wait.Until(_ =>
            {
                var stormCard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card")));
                return stormCard != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));

            var applyDateBegin = "2023-06-03";
            var applyDateBeginSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='受理日期起'] input")));
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{applyDateBegin}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", applyDateBeginSelect);

            var searchButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='查詢']")));
            _actions.MoveToElement(searchButton).Click().Perform();

            var applyCaseNo = _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='applyCaseNo'] span"));
                return element.Displayed ? element : null;
            });

            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        public async Task TwcE101_14()
        {
            var stormtable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormtable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='applyCaseNo'] span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var fileName = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']")));
            _actions.MoveToElement(fileName).Perform();
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_15()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41881288118.pdf", "//button[text()='轉PDF']"), Is.True);
        }
    }
}