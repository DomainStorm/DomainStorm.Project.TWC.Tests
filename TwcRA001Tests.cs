using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using OfficeOpenXml;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcRA001Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public TwcRA001Tests()
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
        public async Task TwcRA001_01() // 0511,tw491身分各建立表單，無錯誤
        {
            //0511 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));

            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft"); 
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var 受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 受理);

            _driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var 夾帶附件 = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4) > div.list-group > storm-tree-node:nth-child(2) > a[href='#file']"));
            _actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(新增文件).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var 上傳 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 > button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(上傳).Click().Perform();

            That(TestHelper.WaitUploadCompleted(_driver), Is.Not.Null);

            var 受理登記 = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5) >  a[href='#finished']"));
            _actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(8) > div.float-end > div.d-flex.flex-row-reverse > div.form-check > input[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            var 確認受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(8) > div.float-end > div.d-flex.flex-row-reverse:nth-child(3) > button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(確認受理).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var 登出 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            _actions.MoveToElement(登出).Click().Perform();

            //tw491 建立表單
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-S100_bmTransferApply.json"));

            await TestHelper.Login(_driver, "tw491", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 受理);

            _driver.SwitchTo().DefaultContent();

            stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            夾帶附件 = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4) > div.list-group > storm-tree-node:nth-child(2) > a[href='#file']"));
            _actions.MoveToElement(夾帶附件).Click().Perform();

            新增文件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            _actions.MoveToElement(新增文件).Click().Perform();

            lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            上傳 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(上傳).Click().Perform();

            That(TestHelper.WaitUploadCompleted(_driver), Is.Not.Null);

            受理登記 = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5) > a[href='#finished']"));
            _actions.MoveToElement(受理登記).Click().Perform();

            用印或代送件只需夾帶附件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            確認受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(確認受理).Click().Perform();

            targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var span = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(9) > div.row > div.col-sm-7")));
            string 受理編號 = span.GetAttribute("textContent");

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(1)]
        public async Task TwcRA001_02To09()
        {
            await TwcRA001_02();
            await TwcRA001_03();
            await TwcRA001_04();
            await TwcRA001_05();
            await TwcRA001_06();
            await TwcRA001_07();
            await TwcRA001_08();
            await TwcRA001_09();
        }
        public async Task TwcRA001_02() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcRA001_03() //呼叫bmTransferApply/confirm 沒錯誤則取得http 200回應

        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-RA001_bmTransferApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task TwcRA001_04() //看到申請之表單內容跳至夾帶附件區塊
        {
            await TestHelper.Login(_driver, "ning53", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var 夾帶附件 = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2) > a[href='#file']"));
            _actions.MoveToElement(夾帶附件).Click().Perform();

            var stormCardSeventh = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(7) > storm-edit-table")));
            var stormTable = stormCardSeventh.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var pElement = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td > p.h3"));
            var text = pElement.SingleOrDefault(t => t.Text == "沒有找到符合的結果");
            if (text != null)
            {
                string filename = text.Text;

                That(filename, Is.EqualTo("沒有找到符合的結果"));
            }
        }

        public async Task TwcRA001_05() // 看到檔案上傳
        {
            var 新增文件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-primary")));
            _actions.MoveToElement(新增文件).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
            lastHiddenInput.SendKeys(filePath);

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.modal.fade.show storm-input-group")));
            string 文件名稱 = stormInputGroup.GetAttribute("value");

            That(文件名稱, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        public async Task TwcRA001_06() // 看到夾帶附件視窗顯示有一筆附件清單資料
        {
            var 上傳 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(上傳).Click().Perform();

            That(TestHelper.WaitUploadCompleted(_driver), Is.Not.Null);
        }

        public async Task TwcRA001_07() // 表單受理欄位中看到核章資訊
        {
            _driver.SwitchTo().Frame(0);

            var 受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 受理);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            bool signElementExists = signElement != null;

            That(signElementExists, Is.True, "未受理");
        }

        public async Task TwcRA001_08() // 看到■用印或代送件只需夾帶附件已打勾
        {
            _driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var 受理登記 = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5) >  a[href='#finished']"));
            _actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        public async Task TwcRA001_09() // 確認完成畫面進入未結案件中
        {
            var 確認受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(確認受理).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var span = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(9) > div.row > div.col-sm-7")));
            string 受理編號 = span.GetAttribute("textContent");

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(2)]
        public async Task TwcRA001_10To11()
        {
            await TwcRA001_10();
            await TwcRA001_11();
        }
        public async Task TwcRA001_10() //使用者帳號0511登入水籍系統
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
        }

        public async Task TwcRA001_11() //有xlsx檔案下載後打開應顯示有台中所2筆、大里所1筆統計數據。
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/report/RA001");
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            Console.WriteLine($"::group::檢查結構---------{_driver.Url}---------");
            Console.WriteLine(_driver.PageSource);
            Console.WriteLine("::endgroup::");

            // 選擇區處別
            var 區處別 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card > form > div.mt-3 > storm-select >div.choices")));
            _actions.MoveToElement(區處別).Click().Perform();

            var 第四區管理處 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.choices__list.choices__list--dropdown div:nth-child(1) [data-value='3eed4fc4-9c06-4d16-9eb6-45aeaf198a25']")));
            _actions.MoveToElement(第四區管理處).Click().Perform();

            // 選擇受理日期起
            var 受理日期起 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='受理日期起']")));
            var input = 受理日期起.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期起 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期起).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月起 = new SelectElement(select);
            受理月起.SelectByText("March");

            var 受理日起 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='March 6, 2023']")));
            _actions.MoveToElement(受理日起).Click().Perform();

            //選擇受理日期迄 
            var 受理日期迄 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='受理日期迄']")));
            input = 受理日期迄.GetShadowRoot().FindElement(By.CssSelector("input"));
            受理日期迄 = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(受理日期迄).Click().Perform();

            select = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var 受理月迄 = new SelectElement(select);
            受理月迄.SelectByText("April");

            var 受理日迄 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='April 6, 2023']")));
            _actions.MoveToElement(受理日迄).Click().Perform();

            // 選擇檔案格式
            var 檔案格式 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[label='檔案格式'] div.choices")));
            _actions.MoveToElement(檔案格式).Click().Perform();

            var Xlsx = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.choices__list [data-value='XLSX']")));
            _actions.MoveToElement(Xlsx).Click().Perform();

            // 檢查下載檔案
            string filePath = Path.Combine(_downloadDirectory, "RA001.xlsx");
            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var 下載 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card.hydrated > form > div:nth-child(5).d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(下載).Click().Perform();

            _wait.Until(_ => File.Exists(filePath));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            string E7Value = worksheet.Cells["E7"].Text;
            string E8Value = worksheet.Cells["E8"].Text;

            That(E7Value == "2" && E8Value == "1",Is.True);
        }
    }
}