    using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public TwcG101Tests()
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
        public async Task TwcG101_01To16()
        {
            await TwcG101_01();
            await TwcG101_02();
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
            await TwcG101_15();
            await TwcG101_16();
        }
        public async Task TwcG101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcG101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G101_bmMilitaryApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcG101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            stiPay.Click();
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");
        }
        public async Task TwcG101_05()
        {
            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_06()
        {
            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            _wait.Until(_ =>
            {
                try
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
                    That(stiIdentificationChoose.GetAttribute("checked"), Is.EqualTo("true"));
                    return true;
                }
                catch
                {
                    stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
                    return false;
                }
            });
            
            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
            _wait.Until(_ =>
            {
                try
                {
                    stiIdentificationInput.SendKeys("BBB");
                    return true;
                }
                catch
                {
                    stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
                    return false;
                }
            });
        }
        public async Task TwcG101_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio'] > input[id='超戶申請group2']")));
            That(stiOverApply.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_09()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > div.mx-6 > h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcG101_10()
        {
            _wait.Until(_ =>
            {
                try
                {
                    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
                    var confirmButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
                    if (confirmButton.Displayed)
                    {
                        _actions.MoveToElement(confirmButton).Click().Perform();
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return true;
                }
            });

            _driver.SwitchTo().Frame(0);

            var stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] > input")));
            _wait.Until(_ =>
            {
                try
                {
                    stiEmailInput.SendKeys("aaa@bbb.ccc");
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    stiEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] > input")));
                    return false;
                }
            });

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] > input")));
            _wait.Until(_ =>
            {
                try
                {
                    stiEmailTelNoInput.SendKeys("02-12345678");
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] > input")));
                    return false;
                }
            });
        }
        public async Task TwcG101_11()
        {
            _driver.SwitchTo().DefaultContent();

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcG101_12()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_13()
        {
            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_14()
        {
            _wait.Until(_ =>
            {
                WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
                _actions.MoveToElement(infoButton).Click().Perform();
                try
                {
                    if (infoButton.Displayed)
                    {
                        _actions.MoveToElement(infoButton).Click().Perform();
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return true;
                }
            });

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcG101_15()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", chceckSign);
            That(chceckSign != null, "未受理");

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiPay);
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG101_16()
        {
            var stiIdentificationTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='檢附證件']")));
            That(stiIdentificationTitle.Text, Is.EqualTo("BBB"));

            var stiOverApplyTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[id='超戶申請radio']")));
            That(stiOverApplyTitle.Text, Is.EqualTo("否"));
        }

        [Test]
        [Order(1)]
        public async Task TwcG101_17To19()
        {
            await TwcG101_17();
        }
        public async Task TwcG101_17()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var 受理日期起 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='受理日期起']")));
            var input = 受理日期起.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期起 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期起).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月起 = new SelectElement(select);
            受理月起.SelectByText("June");

            var 受理日起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='June 17, 2023']")));
            _actions.MoveToElement(受理日起).Click().Perform();

            var 查詢 = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(查詢).Click().Perform();
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell > span"), Is.Not.Null);
        }
        public async Task TwcG101_18()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo'] > storm-table-cell > span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcG101_19()
        {
            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }
            var filePath = Path.Combine(_downloadDirectory, "41105536610.pdf");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var downloadButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='finished'] > div.float-end > div:nth-child(3) > button");
            _actions.MoveToElement(downloadButton).Click().Perform();
            That(Directory.Exists(_downloadDirectory), Is.True);

            Console.WriteLine($"-----檢查檔案完整路徑: {filePath}-----");
            _wait.Until(webDriver =>
            {
                Console.WriteLine($"-----{_downloadDirectory} GetFiles-----");
                foreach (var fn in Directory.GetFiles(_downloadDirectory))
                {
                    Console.WriteLine($"-----filename: {fn}-----");
                }
                Console.WriteLine($"-----{_downloadDirectory} GetFiles end-----");
                return File.Exists(filePath);
            });
            That(File.Exists(filePath), Is.True);
        }
    }
}