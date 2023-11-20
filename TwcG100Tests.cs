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
    public class TwcG100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcG100Tests()
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
        public async Task TwcG100_01To24()
        {
            await TwcG100_01();
            await TwcG100_02();
            await TwcG100_03();
            await TwcG100_04();
            await TwcG100_05();
            await TwcG100_06();
            await TwcG100_07();
            await TwcG100_08();
            await TwcG100_09();
            await TwcG100_10();
            await TwcG100_11();
            await TwcG100_12();
            await TwcG100_13();
        }
        public async Task TwcG100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcG100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G100_bmMilitaryApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcG100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiApplyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(stiApplyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            var stiWaterNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            That(stiWaterNo.Text, Is.EqualTo("41105533310"));

            var stiApplyDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));
            That(stiApplyDate.Text, Is.EqualTo("2023年06月13日"));
        }
        public async Task TwcG100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            stiTrusteeIdNoInput.SendKeys("A123456789" + Keys.Tab);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiTrusteeIdNo);
            That(stiTrusteeIdNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcG100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiApplyEmail = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='申請電子帳單勾選']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiApplyEmail);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiApplyEmail = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='申請電子帳單勾選']")));
            That(stiApplyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiIdentificationChoose);
            stiIdentificationChoose = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='檢附證件group3']")));
            That(stiIdentificationChoose.GetAttribute("checked"), Is.EqualTo("true"));

            var stiIdentificationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='檢附證件'] > input")));
            stiIdentificationInput.SendKeys("BBB" + Keys.Tab);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var stiIdentification = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-identification]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiIdentification);
            That(stiIdentification.Text, Is.EqualTo("BBB"));
        }
        public async Task TwcG100_07()
        {
            var stiOverApply = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-over-apply]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiOverApply);
            That(stiOverApply.Text, Is.EqualTo("否"));
        }
        public async Task TwcG100_08() 
        { 
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note] > input")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            stiNote.SendKeys("備註內容" + Keys.Tab);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiNote = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiNote);
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcG100_09()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiPay);
            Thread.Sleep(500);

            _driver.SwitchTo().DefaultContent();

            var 同步狀態 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p.d-none")));
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            stiPay = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='繳費']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", stiPay);
            That(stiPay.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_10()
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
        public async Task TwcG100_11()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 消費性用水服務契約);
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_12()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司個人資料保護告知事項);
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var 公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司營業章程 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", 公司營業章程);
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcG100_14()
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            //先加入延遲1秒，不然會還沒scroll完就click
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            var 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            var 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            var 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            var 簽名 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 簽名);

            string src_driver_2 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            string? src = null;

            while (src == null)
            {
                try
                {
                    var imgElement = driver.FindElement(By.CssSelector("div.dropzone-container img"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", imgElement);
                    src = imgElement.GetAttribute("src");
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("簽名內容生成中");
                }
            }

            string src_driver_1 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

            That(src_driver_1, Is.EqualTo(src_driver_2));
        }

        [Test]
        [Order(14)]
        public async Task TwcG100_15() // driver_2中看到掃描拍照證件圖像
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            Actions actions = new(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var stormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("storm-tree-node"));
            var 掃描拍照 = stormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));

            actions.MoveToElement(掃描拍照).Click().Perform();

            var 啟動掃描證件 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 啟動掃描證件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 啟動掃描證件);

            var imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_1 = imgElement.GetAttribute("src");

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
            string src_driver_2 = imgElement.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }

        [Test]
        [Order(15)]
        public async Task TwcG100_16() // driver_2中看到夾帶附件資訊
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            var stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            driver.SwitchTo().Window(driver.WindowHandles[1]);

            stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
            stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stormTable);

            var fileName1 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(1) > td[data-field='name']"));
            var spanName1 = fileName1.FindElement(By.CssSelector("span"));
            string spanText1 = spanName1.Text;

            var fileName2 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(2) > td[data-field='name']"));
            var spanName2 = fileName2.FindElement(By.CssSelector("span"));
            string spanText2 = spanName2.Text;

            That(spanText1, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
            That(spanText2, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }

        [Test]
        [Order(16)]
        public async Task TwcG100_17() // 系統顯示[申請電子帳單需要填寫Email及聯絡電話]
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            //先加入延遲1秒，不然會還沒scroll完就click
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));

            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            var 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            var outerContainer = driver.FindElement(By.CssSelector("div.swal2-container.swal2-center.swal2-backdrop-show"));
            var innerContainer = outerContainer.FindElement(By.CssSelector("div.swal2-popup.swal2-modal.swal2-icon-warning.swal2-show"));
            string hintText = innerContainer.Text;

            That(hintText, Is.Not.Null);
        }

        [Test]
        [Order(17)]
        public async Task TwcG100_18() // driver_2中Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);
            Thread.Sleep(500);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            var stiEmailInput = stiEmail.FindElement(By.TagName("Input"));
            stiEmailInput.SendKeys("aaa@bbb.ccc");
            stiEmailInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.SwitchTo().DefaultContent();

            var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            string Email = stiEmail.Text;

            That(Email, Is.EqualTo("aaa@bbb.ccc"));

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Frame(0);

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            var stiEmailTelNoInput = stiEmailTelNo.FindElement(By.TagName("Input"));
            stiEmailTelNoInput.SendKeys("02-12345678");
            stiEmailTelNoInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.SwitchTo().DefaultContent();

            同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.SwitchTo().Frame(0);

            stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            string 聯絡電話 = stiEmailTelNo.Text;

            That(聯絡電話, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(18)]
        public async Task TwcG100_19() // 該申請案件進入未結案件中
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 申請電子帳單勾選);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            var stiEmailInput = stiEmail.FindElement(By.TagName("input"));
            stiEmailInput.SendKeys("aaa@bbb.ccc");
            stiEmailInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            var stiEmailTelNoInput = stiEmailTelNo.FindElement(By.TagName("input"));
            stiEmailTelNoInput.SendKeys("02-12345678");
            stiEmailTelNoInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 撫卹 = driver.FindElement(By.Id("檢附證件group3"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 撫卹);

            var 檢附證件 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("檢附證件")));
            var 檢附證件Input = 檢附證件.FindElement(By.TagName("input"));
            檢附證件Input.SendKeys("BBB");
            檢附證件Input.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 否 = driver.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 否);

            var stiNote = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-note]")));
            var stiNoteInput = stiNote.FindElement(By.TagName("input"));
            stiNoteInput.SendKeys("備註內容");
            stiNoteInput.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 繳費);

            var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            //先加入延遲1秒，不然會還沒scroll完就click
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            actions.MoveToElement(新增文件).Perform();
            新增文件.Click();

            wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

            IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
            var lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            var 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(上傳).Click().Perform();

            var stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));

            actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            var 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 確認受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 確認受理);

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo)!;
            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(19)]
        public async Task TwcG100_20() // driver_1中有看到註記欄位內容顯示-備註內容
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiNote = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[sti-note]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiNote);
            string 註記 = stiNote.Text;

            That(註記, Is.EqualTo("備註內容"));
        }

        [Test]
        [Order(20)]
        public async Task TwcG100_21() // driver_1中有看到打勾顯示■繳費
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(21)]
        public async Task TwcG100_22() // 表單內容顯示■申請電子帳單，Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 申請電子帳單勾選 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 申請電子帳單勾選);

            bool isChecked = 申請電子帳單勾選.Selected;

            That(isChecked, Is.True);

            var stiEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEmail);
            string Email = stiEmail.Text;

            That(Email, Is.EqualTo("aaa@bbb.ccc"));

            var stiEmailTelNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEmailTelNo);
            string 聯絡電話 = stiEmailTelNo.Text;

            That(聯絡電話, Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(22)]
        public async Task TwcG100_23() // 檢附證件欄位看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 撫卹 = driver.FindElement(By.Id("檢附證件radio"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            string spanText = 撫卹.Text;

            That(spanText, Is.EqualTo("撫卹令或撫卹金分領證書"));

            var identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification]")));

            string spanText_Identification = identification.Text;
            That(spanText_Identification, Is.EqualTo("BBB"));
        }

        [Test]
        [Order(23)]
        public async Task TwcG100_24() // 超戶申請欄位看到●否
        {
            ChromeDriver driver = TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 否 = driver.FindElement(By.Id("超戶申請radio"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            string spanText = 否.Text;

            That(spanText, Is.EqualTo("否"));
        }
    }
}