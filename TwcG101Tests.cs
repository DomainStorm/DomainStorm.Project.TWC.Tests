﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcG101Tests
    {
        private string _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        private List<ChromeDriver> _chromeDriverList;
        public TwcG101Tests()
        {
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            _chromeDriverList = new List<ChromeDriver>();

            return Task.CompletedTask;
        }
        private ChromeDriver GetNewChromeDriver()
        {
            var option = new ChromeOptions();
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-gpu");
            option.AddArgument("--enable-javascript");
            option.AddArgument("--allow-running-insecure-content");
            option.AddArgument("--ignore-urlfetcher-cert-requests");
            option.AddArgument("--disable-web-security");
            option.AddArgument("--ignore-certificate-errors");
            option.AddUserProfilePreference("download.default_directory", _downloadDirectory);
            //option.AddArguments("--no-sandbox");

            if (TestHelper.GetChromeConfig().Headless)
                option.AddArgument("--headless");

            new DriverManager().SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig());
            var driver = new ChromeDriver(option);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            _chromeDriverList.Add(driver);

            return driver;
        }

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            foreach (ChromeDriver driver in _chromeDriverList)
            {
                driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        public async Task TwcG101_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcG101_02() // 呼叫bmMilitaryApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmMilitaryApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-G101_bmMilitaryApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcG101_03() // 看到表單內容並於表單受理欄位中看到有■繳費
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 繳費 = driver.FindElement(By.Id("繳費"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 繳費);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 繳費);

            That(繳費.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(3)]
        public async Task TwcG101_04() // 顯示臨櫃人員核章職稱姓名等資訊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));

            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(4)]
        public async Task TwcG101_05() // 看到■申請電子帳單
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            That(applyEmail.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcG101_06() // 看到●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 撫卹 = driver.FindElement(By.Id("檢附證件group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 撫卹);

            That(撫卹.GetAttribute("checked"), Is.EqualTo("true"));

            var identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));

            identification.SendKeys("BBB");

            That(identification.GetAttribute("value"), Is.EqualTo("BBB"));
        }

        [Test]
        [Order(6)]
        public async Task TwcG101_07() // 看到●否
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var 否 = driver.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 否);

            That(否.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task TwcG101_08() // 看到■用印或代送件只需夾帶附件已打勾
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver);
            actions.MoveToElement(受理登記).Click().Perform();

            var 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            That(用印或代送件只需夾帶附件.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task TwcG101_09() // 系統跳出【尚未夾帶附件】、【申請電子帳單需要填寫Email及聯絡電話】訊息
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fifthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(5)"));
            var 受理登記 = fifthStormTreeNode.FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver);
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

            That(hintText,Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳\r\n確定"));
        }

        [Test]
        [Order(9)]
        public async Task TwcG101_10() // Email欄位內顯示aaa@bbb.ccc;聯絡電話欄位內顯示02-12345678
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email.SendKeys("aaa@bbb.ccc");
            email.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            telNo.SendKeys("02-12345678");
            telNo.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            That(email.GetAttribute("value"), Is.EqualTo("aaa@bbb.ccc"));

            telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            That(telNo.GetAttribute("value"), Is.EqualTo("02-12345678"));
        }

        [Test]
        [Order(10)]
        public async Task TwcG101_11() // 看到申請之表單內容跳至夾帶附件區塊
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver);
            actions.MoveToElement(夾帶附件).Click().Perform();

            var stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var findElement = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td > p.h3"));
            var element = findElement.SingleOrDefault(e => e.Text == "沒有找到符合的結果");
            if (element != null)
            {
                string filename = element.Text;

                That(filename, Is.EqualTo("沒有找到符合的結果"));
            }
        }

        [Test]
        [Order(11)]
        public async Task TwcG101_12() // 看到檔案上傳
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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

            var stormInputGroup = driver.FindElement(By.CssSelector("body storm-main-content main div div div div storm-card form storm-input-group"));
            string value = stormInputGroup.GetAttribute("value");

            That(value, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(12)]
        public async Task TwcG101_13() // 看到夾帶附件視窗顯示有一筆附件清單資料，檔名為twcweb_01_1_夾帶附件1.pdf
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

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

            var stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            var element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
            var spanElement = element.FindElement(By.CssSelector("span"));
            wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

            string spanText = spanElement.Text;

            That(spanText, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(13)]
        public async Task TwcG101_14() // 確認完成畫面進入未結案件中
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var applyEmail = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("申請電子帳單勾選")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", applyEmail);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyEmail);

            var email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email] > input")));
            email.SendKeys("aaa@bbb.ccc");
            email.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no] > input")));
            telNo.SendKeys("02-12345678");
            telNo.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 撫卹 = driver.FindElement(By.Id("檢附證件group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 撫卹);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 撫卹);

            var identification = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-identification] > input")));

            identification.SendKeys("BBB");
            identification.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            var 否 = driver.FindElement(By.Id("超戶申請group2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 否);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 否);

            var 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].dispatchEvent(new Event('click'));", 受理);

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new (driver);
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

            var stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            var stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
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
            element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);

            string 受理編號 = element.Text;

            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
        }

        [Test]
        [Order(14)]
        public async Task TwcG101_15() // 表單內容顯示■申請電子帳單，Email欄位內顯示aaa @bbb.ccc; 聯絡電話欄位內顯示02-12345678，已勾選■已詳閱貴公司消費性用水服務契約、公司個人資料保護法、貴公司營業章程
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var email = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", email);

            var stiEmailElement = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email='']"));
            string spanText_Email = stiEmailElement.Text;

            That(spanText_Email, Is.EqualTo("aaa@bbb.ccc"));

            var telNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-email-tel-no]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", telNo);

            var stiTelNoElement = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-email-tel-no='']"));
            string spanText_TelNo = stiTelNoElement.Text;

            That(spanText_TelNo, Is.EqualTo("02-12345678"));

            driver.SwitchTo().DefaultContent();

            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var secondStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
            var firstStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node"));
            var 消費性用水服務契約 = firstStormTreeNode.FindElement(By.CssSelector("a[href='#contract_1']"));

            Actions actions = new(driver);
            actions.MoveToElement(消費性用水服務契約).Click().Perform();

            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);

            wait.Until(driver =>
            {
                var 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
                return 消費性用水服務契約.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            secondStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
            var 公司個人資料保護告知事項 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#contract_2']"));

            actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);

            wait.Until(driver =>
            {
                var 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
                return 消費性用水服務契約.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            secondStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
            var thirdStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node:nth-child(3)"));
            var 公司營業章程 = thirdStormTreeNode.FindElement(By.CssSelector("a[href='#contract_3']"));

            actions.MoveToElement(公司營業章程).Click().Perform();

            公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);

            wait.Until(driver =>
            {
                var 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
                return 公司營業章程.GetAttribute("checked") == "true";
            });

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(15)]
        public async Task TwcG101_16() // 看到檢附證件欄位●撫卹令或撫卹金分領證書被選擇並有欄位內顯示值BBB 。看到超戶申請欄位顯示看到●否
        {
            ChromeDriver driver = GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver.SwitchTo().Frame(0);

            var stiIdentification = driver.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-identification]"));

            string spanText_Identification = stiIdentification.Text;

            That(spanText_Identification, Is.EqualTo("BBB"));

            var stiOverApply = driver.FindElement(By.CssSelector("span[data-class='MultiInputSelectBlockBlock'][sti-over-apply]"));

            string spanText_stiOverApply = stiOverApply.Text;

            That(spanText_stiOverApply, Is.EqualTo("否"));
        }

        [Test]
        [Order(16)]
        public async Task TwcG101_17() // 查詢出來該件，列在下方
        {
            ChromeDriver driver = GetNewChromeDriver();

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
        [Order(17)]
        public async Task TwcG101_18() // 確認附件有一件[twcweb_01_1_夾帶附件1.pdf]
        {
            ChromeDriver driver = GetNewChromeDriver();

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

            stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            actions.MoveToElement(element).Click().Perform();

            var stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
            var secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
            var 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

            actions.MoveToElement(夾帶附件).Click().Perform();

            var linkElement = driver.FindElement(By.CssSelector("a[download='twcweb_01_1_夾帶附件1.pdf']"));
            string downloadValue = linkElement.GetAttribute("download");

            That(downloadValue, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
        }

        [Test]
        [Order(18)]
        public async Task TwcG101_19() // PDF檔產製成功
        {
            ChromeDriver driver = GetNewChromeDriver();

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

            stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCardSecond = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCardSecond.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));

            actions.MoveToElement(element).Click().Perform();

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

            string filePath = Path.Combine(_downloadDirectory, "41105536610.pdf");

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