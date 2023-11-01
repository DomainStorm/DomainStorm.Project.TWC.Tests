using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V113.Accessibility;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcC101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public TwcC101Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _actions = new Actions(_driver);
        }

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcC101_01To08()
        {
            await TwcC101_01();
            await TwcC101_02();
            await TwcC101_03();
            await TwcC101_04();
            await TwcC101_05();
            await TwcC101_06();
            await TwcC101_07();
            await TwcC101_08();
        }
        public async Task TwcC101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcC101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C101_bmDisableApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcC101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var 受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 受理);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            var signElementExists = signElement != null;
            That(signElementExists, Is.True, "未受理");
        }
        public async Task TwcC101_04()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC101_05()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-html-container > div.mx-6 > h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcC101_06()
        {
            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            closeButton.Click();

            _driver.SwitchTo().DefaultContent();
            
            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var checkImage = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img");
            That(checkImage.GetAttribute("src"), Is.Not.Null);
        }
        public async Task TwcC101_07()
        {
            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcC101_08()
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

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));
            That(imgElement, Is.Not.Null);
        }

        [Test]
        [Order(1)]
        public async Task TwcC101_09To11()
        {
            await TwcC101_09();
            await TwcC101_10();
            await TwcC101_11();
        }
        public async Task TwcC101_09() // 查詢出來該件，列在下方
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");
            
            var 受理日期起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='受理日期起']")));
            var input = 受理日期起.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期起 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期起).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月起 = new SelectElement(select);
            受理月起.SelectByText("June");

            var 受理日起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='June 3, 2023']")));
            _actions.MoveToElement(受理日起).Click().Perform();

            var 查詢 = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(查詢).Click().Perform();

            _wait.Until(_ =>
            {
                var e = _wait.Until(_ =>
                {
                    var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
                    return stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo'] > storm-table-cell > span"));
                });
                return !string.IsNullOrEmpty(e.GetAttribute("textContent")) ? e : null;
            });
        }
        public async Task TwcC101_10() // 確認掃描拍照區塊有掃描拍照證件圖像
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo'] > storm-table-cell > span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcC101_11() // PDF檔產製成功自動下載於下載區
        {
            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            var filePath = Path.Combine(_downloadDirectory, "41101699338.pdf");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var downloadButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='finished'] > div.float-end > div:nth-child(3) > button");
            downloadButton.Click();

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
