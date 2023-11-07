using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public TwcE101Tests()
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
        public async Task TwcE101_01To13()
        {
            await TwcE101_01();
            await TwcE101_02();
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
            //await TwcE101_13();
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

        public async Task TwcE101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            stiEnd.Click();
            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign != null, "未受理");
        }

        public async Task TwcE101_05()
        {
            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_06()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcE101_07()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > div.mx-6 > h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }

        public async Task TwcE101_08()
        {
            while (true)
            {
                try
                {
                    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
                    var confirmButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
                    if (confirmButton.Displayed)
                    {
                        _actions.MoveToElement(confirmButton).Click().Perform();
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiApplyEmail);

            var stiApplyEmailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單Email'] > input")));
            stiApplyEmailInput.SendKeys("aaa@bbb.ccc");
            That(stiApplyEmailInput.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='電子帳單聯絡電話'] > input")));
            stiEmailTelNoInput.SendKeys("02-12345678");
            stiEmailTelNoInput.SendKeys(Keys.Tab);
            That(stiEmailTelNoInput.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }

        public async Task TwcE101_09()
        {
            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle.Text, Is.EqualTo("沒有找到符合的結果"));
        }

        public async Task TwcE101_10()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            var fileName = stormInputGroup.GetAttribute("value");
            That(fileName, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_11()
        {
            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
            That(fileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcE101_12()
        {
            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _actions.MoveToElement(infoButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");

            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        public async Task TwcE101_13()
        {
            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = TestHelper.FindAndMoveElement(_driver, "[id='contract_1']");

            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(消費性用水服務契約).Click().Perform();

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司營業章程).Click().Perform();

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        [Test]
        [Order(13)]
        public async Task TwcE101_14() // 查詢出來該件，列在下方
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.flatpickr-calendar.animate.arrowTop.arrowLeft.open")));

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("June");

            var spanElement = driver.FindElement(By.CssSelector("span[aria-label='June 6, 2023']"));
            spanElement.Click();

            var divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            var stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            spanElement = element.FindElement(By.CssSelector("span"));
            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        [Test]
        [Order(14)]
        public async Task TwcE101_15() // 確認附件有一件[twcweb_01_1_夾帶附件1.pdf]
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var linkElement = driver.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string download = linkElement.GetAttribute("download");

            That(download, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }
        [Test]
        [Order(15)]
        public async Task TwcE101_16() // PDF檔產製成功
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var divFirst = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = divFirst.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            Thread.Sleep(750);
            selectMonth.SelectByText("June");

            var spanElement = driver.FindElement(By.CssSelector("span[aria-label='June 1, 2023']"));
            spanElement.Click();

            var divSecond = stormCard.FindElement(By.CssSelector("div.row.mt-3"));
            stormInputGroup = divSecond.FindElement(By.CssSelector("storm-input-group"));
            inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector(".form-control.multisteps-form__input"));

            inputElement.SendKeys(TestHelper.ApplyCaseNo);

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            actions.MoveToElement(element).Click().Perform();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(夾帶附件).Click().Perform();

            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            var 下載PDF = driver.FindElement(By.CssSelector("button.btn.bg-gradient-warning.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 下載PDF);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 下載PDF);

            string filePath = Path.Combine(_downloadDirectory, "41881288118.pdf");

            That(Directory.Exists(_downloadDirectory), Is.True);

            Console.WriteLine($"-----{_downloadDirectory} GetFiles-----");

            foreach (var fn in Directory.GetFiles(_downloadDirectory))
            {
                Console.WriteLine($"-----filename: {fn}-----");
            }

            Console.WriteLine($"-----{_downloadDirectory} GetFiles end-----");

            Console.WriteLine($"-----檢查檔案完整路徑: {filePath}-----");

            wait.Until(webDriver => File.Exists(filePath));

            That(File.Exists(filePath), Is.True);
        }
    }
}
