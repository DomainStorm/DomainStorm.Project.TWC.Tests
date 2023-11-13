using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcS100Tests()
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
        public async Task TwcS100_01To02()
        {
            await TwcS100_01();
            await TwcS100_02();
        }
        public async Task TwcS100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");

            _driver.SwitchTo().DefaultContent();

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcS100_02()
        {
            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            var buttonTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
            That(buttonTitle.Text, Is.EqualTo("登入"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS100_03To10()
        {
            await TwcS100_03();
            await TwcS100_04();
            await TwcS100_05();
            await TwcS100_06();
            await TwcS100_07();
            await TwcS100_08();
            await TwcS100_09();
            await TwcS100_10();
        }
        public async Task TwcS100_03()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcS100_04()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcS100_05()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcS100_06()
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
        public async Task TwcS100_07()
        {
            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            if (uploadButton.Displayed)
            {
                _actions.MoveToElement(uploadButton).Click().Perform();
            }
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        public async Task TwcS100_08()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");
        }
        public async Task TwcS100_09()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcS100_10()
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

        [Test]
        [Order(2)]
        public async Task TwcS100_11To12()
        {
            await TwcS100_11();
            await TwcS100_12();
        }
        public async Task TwcS100_11()
        {
            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var 受理日期起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='受理日期起']")));
            var input = 受理日期起.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期起 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期起).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月起 = new SelectElement(select);
            受理月起.SelectByText("March");

            var 受理日起 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='March 6, 2023']")));
            _actions.MoveToElement(受理日起).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();

            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell > span"), Is.Not.Null);
            That(TestHelper.WaitStormTableUpload(_driver, "tr > td[data-field='userName'] > storm-table-cell > span")!.Text, Is.EqualTo("張博文"));
            That(TestHelper.WaitStormTableUpload(_driver, "tr:nth-child(2) > td[data-field='userName'] > storm-table-cell > span")!.Text, Is.EqualTo("謝德威"));
        }
        public async Task TwcS100_12()
        {
            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            await TestHelper.Login(_driver, "4e03", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var stormDropdown = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-sidenav > aside > ul > li > storm-dropdown > div > a > div > div > span")));
            That(stormDropdown.Text, Is.EqualTo("草屯營運所業務股 - 業務員"));
        }
        public async Task TwcS100_13() //因不同站所，故查詢不到任何資料
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();;

            await TestHelper.Login(driver, "4e03", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            var 查詢 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));

            Actions actions = new(driver);
            actions.MoveToElement(inputElement).Click().Perform();
            var calendar = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".flatpickr-calendar.open")));
            var monthDropdown = calendar.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var spanElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("span[aria-label='March 6, 2023']")));
            actions.MoveToElement(spanElement).Click().Perform();

            actions.MoveToElement(查詢).Click().Perform();

            var secondStormCard = stormMainContent.FindElement(By.CssSelector("storm-card:nth-child(2)"));
            var stormDocumentListDetail = secondStormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var text = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive.border table tbody tr td p")).GetAttribute("textContent");

            That(text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}