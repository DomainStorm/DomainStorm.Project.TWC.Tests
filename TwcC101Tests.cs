using OpenQA.Selenium;
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
        public async Task TwcC101_01To05()
        {
            await TwcC101_01();
            await TwcC101_02();
            await TwcC101_03();
            await TwcC101_04();
            await TwcC101_05();
        }
        public async Task TwcC101_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcC101_02() // 呼叫bmDisableApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmDisableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-C101_bmDisableApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcC101_03() // 看到表單受理欄位中看到核章資訊
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
        public async Task TwcC101_04() // 看到■用印或代送件只需夾帶附件已打勾
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcC101_05() // 系統跳出【尚未夾帶附件】訊息
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-html-container > div.mx-6 > h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        [Test]
        [Order(1)]
        public async Task TwcC101_06To07()
        {
            await TwcC101_03();
            await TwcC101_06();
            await TwcC101_07();
        }
        public async Task TwcC101_06() // 看到掃描拍照證件圖像
        {
            _driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var 掃描拍照 = fourthStormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));
            _actions.MoveToElement(掃描拍照).Click().Perform();

            var 啟動掃描證件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.btn-primary.ms-2")));
            _actions.MoveToElement(啟動掃描證件).Click().Perform();
            //等待照片加載完
            Thread.Sleep(3000);

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container img")));
            string src = imgElement.GetAttribute("src");

            That(src, Is.Not.Null);
        }
        public async Task TwcC101_07() // 確認完成畫面進入未結案件中
        {
            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));
            _actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(用印或代送件只需夾帶附件).Click().Perform();

            var 確認受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(確認受理).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var span = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(9) > div.row > div.col-sm-7")));
            string 受理編號 = span.GetAttribute("textContent");

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        [Test]
        [Order(2)]
        public async Task TwcC101_08() // 看到掃描拍照區塊顯示該證件圖像檔案。已勾選■已詳閱貴公司消費性用水服務契約、公司個人資料保護法、貴公司營業章程
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var 消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='消費性用水服務契約']")));
            _actions.MoveToElement(消費性用水服務契約).Click().Perform();

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='公司個人資料保護告知事項']")));
            _actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='公司營業章程']")));
            _actions.MoveToElement(公司營業章程).Click().Perform();

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var 掃描拍照 = fourthStormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));
            _actions.MoveToElement(掃描拍照).Click().Perform();

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));

            That(imgElement, Is.Not.Null);
        }
        [Test]
        [Order(3)]
        public async Task TwcC101_09To10()
        {
            await TwcC101_09();
            await TwcC101_10();
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

            Thread.Sleep(1000);

            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div.table-container > table > tbody > tr > td.align-middle.text-start > storm-table-cell.hydrated > span"));
            string spanText = applyCaseNo.Text;

            That(spanText, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcC101_10() // 確認掃描拍照區塊有掃描拍照證件圖像
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div.table-container > table > tbody > tr > td.align-middle.text-start > storm-table-cell.hydrated > span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var 掃描拍照 = fourthStormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));
            _actions.MoveToElement(掃描拍照).Click().Perform();

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));

            That(imgElement, Is.Not.Null);
        }
        [Test]
        [Order(4)]
        public async Task TwcC101_11() // PDF檔產製成功自動下載於下載區
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

            Thread.Sleep(1000);

            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div.table-container > table > tbody > tr > td.align-middle.text-start > storm-table-cell.hydrated > span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));
            _actions.MoveToElement(受理登記).Click().Perform();

            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            string filePath = Path.Combine(_downloadDirectory, "41101699338.pdf");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var 轉PDF = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-warning.m-0.ms-2")));
            _actions.MoveToElement(轉PDF).Click().Perform();


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
