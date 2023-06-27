using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using WebDriverManager;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA100Tests
    {
        private List<ChromeDriver> _chromeDriverList;

        public TwcA100Tests()
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
        public async Task TwcA100_01() // 取得token
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task TwcA100_02() // 呼叫bmEnableApply/confirm
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A100_bmEnableApply.json"));
            
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(2)]
        public async Task TwcA100_03() // driver_2中看到申請之表單內容
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_2, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_2.SwitchTo().Frame(0);

            IWebElement applyCaseNo = driver_2.FindElement(By.CssSelector("[sti-apply-case-no]"));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            IWebElement waterNo = driver_2.FindElement(By.CssSelector("[sti-water-no]"));
            That(waterNo.Text, Is.EqualTo("41101202191"));

            IWebElement applyDate = driver_2.FindElement(By.CssSelector("[sti-apply-date]"));
            That(applyDate.Text, Is.EqualTo("2023年03月06日"));
        }


        [Test]
        [Order(3)]
        public async Task TwcA100_04() // driver_2中看到身分證字號欄位出現A123456789
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement stiTrusteeIdNo_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no] > input")));
            stiTrusteeIdNo_driver_1.SendKeys("A123456789");
            stiTrusteeIdNo_driver_1.SendKeys(Keys.Tab);

            driver_2.SwitchTo().Frame(0);

            IWebElement stiTrusteeIdNo_driver_2 = driver_2.FindElement(By.CssSelector("span[data-class='InputSelectBlock'][sti-trustee-id-no]"));
            string spanText_stiTrusteeIdNo_driver_2 = stiTrusteeIdNo_driver_2.Text;

            That(spanText_stiTrusteeIdNo_driver_2, Is.EqualTo("A123456789"));
        }

        [Test]
        [Order(4)]
        public async Task TwcA100_05() // driver_2看到受理欄位有落章
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            driver_1.SwitchTo().Frame(0);

            IWebElement 受理_driver_1 = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 受理_driver_1);
            wait.Until(ExpectedConditions.ElementToBeClickable(受理_driver_1));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 受理_driver_1);
           
            driver_2.SwitchTo().Frame(0);

            IReadOnlyList<IWebElement> signElement = driver_2.FindElements(By.CssSelector("[class='sign']"));
            That(signElement, Is.Not.Empty, "未受理");
        }

        [Test]
        [Order(5)]
        public async Task TwcA100_06() // driver_2中勾選消費性用水服務契約
        {
            await TwcA100_05();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            driver_1.SwitchTo().DefaultContent();
            driver_2.SwitchTo().DefaultContent();

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeFirst = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement hrefContract_1 = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_1']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_1).Click().Perform();

            IWebElement 消費性用水服務契約_driver_1 = driver_1.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約_driver_1);

            IWebElement 消費性用水服務契約_driver_2 = driver_2.FindElement(By.Id("消費性用水服務契約"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 消費性用水服務契約_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("消費性用水服務契約"));
                return element.GetAttribute("checked") == "true";
            });

            That(消費性用水服務契約_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(5)]
        public async Task TwcA100_07() // driver_2中勾選公司個人資料保護告知事項
        {
            await TwcA100_06();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeSecondUnderSecond = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefContract_2 = stormTreeNodeSecondUnderSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_2']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_2).Click().Perform();

            IWebElement 公司個人資料保護告知事項_driver_1 = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項_driver_1);

            IWebElement 公司個人資料保護告知事項_driver_2 = driver_2.FindElement(By.Id("公司個人資料保護告知事項"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司個人資料保護告知事項"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司個人資料保護告知事項_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(7)]
        public async Task TwcA100_08() // driver_2中勾選公司營業章程
        {
            await TwcA100_07();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeSecond = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement stormTreeNodeThird = stormTreeNodeSecond.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            IWebElement hrefContract_3 = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#contract_3']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefContract_3).Click().Perform();

            IWebElement 公司營業章程_driver_1 = driver_1.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程_driver_1);

            IWebElement 公司營業章程_driver_2 = driver_2.FindElement(By.Id("公司營業章程"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", 公司營業章程_driver_2);

            wait.Until(driver_1 =>
            {
                IWebElement element = driver_1.FindElement(By.Id("公司營業章程"));
                return element.GetAttribute("checked") == "true";
            });

            That(公司營業章程_driver_1.GetAttribute("checked"), Is.EqualTo("true"));
        }

        [Test]
        [Order(8)]
        public async Task TwcA100_09() // driver_2中表單畫面完整呈現簽名內容，並於driver_1中看到相容內容
        {
            await TwcA100_08();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            IWebElement stormVerticalNavigation = driver_1.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeThird = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[2];
            IWebElement hrefSignature = stormTreeNodeThird.GetShadowRoot().FindElement(By.CssSelector("a[href='#signature']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefSignature).Click().Perform();

            IWebElement buttonSign_driver_1 = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonSign_driver_1);

            IWebElement buttonSign_driver_2 = driver_2.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_2).ExecuteScript("arguments[0].click();", buttonSign_driver_2);

            IWebElement container_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            IWebElement container_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));

            IWebElement img_driver_1 = container_driver_1.FindElement(By.CssSelector("img"));
            IWebElement img_driver_2 = container_driver_2.FindElement(By.CssSelector("img"));

            string src_driver_1 = img_driver_1.GetAttribute("src");
            string src_driver_2 = img_driver_2.GetAttribute("src");

            That(src_driver_2, Is.EqualTo(src_driver_1));
        }


        [Test]
        [Order(9)]
        public async Task TwcA100_10() // driver_2中看到掃描拍照證件圖像
        {
            ChromeDriver driver_1 = GetNewChromeDriver();

            await TestHelper.Login(driver_1, TestHelper.UserId!, TestHelper.Password!);
            driver_1.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(driver_1, TestHelper.ApplyCaseNo!);

            string id = TestHelper.GetLastSegmentFromUrl(driver_1);
            ChromeDriver driver_2 = GetNewChromeDriver();

            await TestHelper.Login(driver_2, TestHelper.UserId!, TestHelper.Password!);
            driver_2.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

            IWebElement stormVerticalNavigation = driver_1.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeFirst = stormTreeNodeFourth.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node"));
            IWebElement hrefCredential = stormTreeNodeFirst.GetShadowRoot().FindElement(By.CssSelector("a[href='#credential']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefCredential).Click().Perform();

            IWebElement buttonScanId = driver_1.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonScanId);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", buttonScanId);

            IWebElement container_driver_1 = driver_1.FindElement(By.CssSelector("div.dropzone-container"));
            IWebElement container_driver_2 = driver_2.FindElement(By.CssSelector("div.dropzone-container"));

            IWebElement img_driver_1 = container_driver_1.FindElement(By.CssSelector("img"));
            IWebElement img_driver_2 = container_driver_2.FindElement(By.CssSelector("img"));

            string src_driver_1 = img_driver_1.GetAttribute("src");
            string src_driver_2 = img_driver_2.GetAttribute("src");

            That(src_driver_1, Is.EqualTo(src_driver_2));
        }

        [Test]
        [Order(10)]
        public async Task TwcA100_11() // driver_2中看到夾帶附件資訊
        {
            await TwcA100_05();

            ChromeDriver driver_1 = _chromeDriverList[0];
            ChromeDriver driver_2 = _chromeDriverList[1];

            driver_1.SwitchTo().DefaultContent();
            driver_2.SwitchTo().DefaultContent();

            IWebElement stormVerticalNavigation = driver_1.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFourth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[3];
            IWebElement stormTreeNodeSecond = stormTreeNodeFourth.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[1];
            IWebElement hrefFile = stormTreeNodeSecond.GetShadowRoot().FindElement(By.CssSelector("a[href='#file']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFile).Click(hrefFile).Perform();

            IWebElement buttonAddDocument = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", buttonAddDocument);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", buttonAddDocument);
            Thread.Sleep(1000);

            IList<IWebElement> hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));
            IWebElement lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
            string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

            lastHiddenInput.SendKeys(附件1Path);

            hiddenInputs = driver_1.FindElements(By.CssSelector("body > .dz-hidden-input"));

            lastHiddenInput = hiddenInputs[^1];

            string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
            string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

            lastHiddenInput.SendKeys(附件2Path);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            IWebElement uploadButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            actions.MoveToElement(uploadButton).Click().Perform();

            IWebElement stormCardSeventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
            IWebElement stormEditTable = stormCardSeventh.FindElement(By.CssSelector("storm-edit-table"));
            IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            IWebElement fileName = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']"));
            IWebElement spanName = fileName.FindElement(By.CssSelector("span"));
            string spanText = spanName.Text;

            That(spanText, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));
        }

        [Test]
        [Order(11)]
        public async Task TwcA100_12() // 該申請案件進入未結案件中等待後續排程資料於結案後消失
        {
            await TwcA100_11();

            ChromeDriver driver_1 = _chromeDriverList[0];
            _chromeDriverList[1].Quit();

            IWebElement stormVerticalNavigation = driver_1.FindElement(By.CssSelector("storm-vertical-navigation"));
            IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            IWebElement stormTreeNodeFifth = stormTreeView.GetShadowRoot().FindElements(By.CssSelector("storm-tree-node"))[4];
            IWebElement hrefFinished = stormTreeNodeFifth.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));

            Actions actions = new(driver_1);
            actions.MoveToElement(hrefFinished).Click().Perform();

            IWebElement 用印或代送件只需夾帶附件 = driver_1.FindElement(By.Id("用印或代送件只需夾帶附件"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", 用印或代送件只需夾帶附件);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", 用印或代送件只需夾帶附件);

            IWebElement confirmButton = driver_1.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].scrollIntoView(true);", confirmButton);
            ((IJavaScriptExecutor)driver_1).ExecuteScript("arguments[0].click();", confirmButton);

            WebDriverWait wait = new(driver_1, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            wait.Until(ExpectedConditions.UrlContains(targetUrl));

            IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
            IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            IWebElement stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var findElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            var element = findElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo);
            if (element != null)
            {
                string applyCaseNo = element.Text;

                That(applyCaseNo, Is.EqualTo(TestHelper.ApplyCaseNo));
            }
        }
    }
}