using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcA100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _actions = new Actions(_driver);
        }

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcA100_01To37()
        {
            await TwcH100();
            await TwcQ100();
            await TwcA100_12();
            await TwcA100_13();
            await TwcA100_14();
            await TwcA100_15();
            await TwcA100_16();
            await TwcA100_17();
            await TwcA100_18();
            await TwcA100_19();
            await TwcA100_20();
            await TwcA100_21();
            await TwcA100_22();
            await TwcA100_23();
            await TwcA100_24();
            await TwcA100_25();
            await TwcA100_26();
            await TwcA100_27();
            await TwcA100_28();
            await TwcA100_29();
            await TwcA100_30();
            await TwcA100_31();
            await TwcA100_32();
            await TwcA100_33();
            await TwcA100_34();
            await TwcA100_35();
            await TwcA100_36();
            await TwcA100_37();
        }
        public async Task TwcH100()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體管理']")));

            var addFileButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='媒體管理'] button");
            addFileButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='媒體管理'] button")));

            var attachment = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, attachment, "input.dz-hidden-input");

            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
            That(attachmentName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='新增檔案'] button")));
            That(TestHelper.FindShadowElement(_driver, "stormEditTable", "span")!.Text, Is.EqualTo("testmedia.mp4"));

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='節目單管理']");
            That(stormCard.Text, Is.EqualTo("節目單管理"));

            var addListButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='節目單管理'] button");
            addListButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='節目單管理'] button")));

            stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增節目單']");
            That(stormCard.Text, Is.EqualTo("新增節目單"));

            var addMediaButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增節目單'] button");
            addMediaButton!.Click();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            addButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("span.rz-button-box")));

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group")));
            var stormInputGroupInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            stormInputGroupInput.SendKeys("節目單測試");

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.ql-editor")));
            stormTextEditorInput.SendKeys("跑馬燈測試");

            var createButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='新增節目單'] button[form='create']");
            createButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("storm-card[headline='新增節目單']")));
            var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-bottom > div.table-pageInfo");
            That(pageInfo!.Text, Is.EqualTo("顯示第 1 至 1 筆，共 1 筆"));

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));

            var searchButton = TestHelper.FindAndMoveToElement(_driver, "storm-card[headline='節目單審核'] button");
            searchButton!.Click();

            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var auditButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(3)"));
            _actions.MoveToElement(auditButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            confirmButton!.Click();

            _wait.Until(driver => {
                var statusElement = TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='playListStatus'] span");
                return statusElement!.Text == "核准";
            });
            That(TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='playListStatus'] span")!.Text, Is.EqualTo("核准"));
        }
        public async Task TwcQ100()
        {
            await TwcA100_02();
            await TwcA100_03();
            await TwcA100_04();
            await TwcA100_05();
            await TwcA100_06();
            await TwcA100_07();
            await TwcA100_08();
            await TwcA100_09();
            await TwcA100_10();
            await TwcA100_11();
        }
        public async Task TwcA100_02()
        {
            var logout = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='./logout']")));
            logout!.Click();

            TestHelper.ChangeUser(_driver, "meizi");
        }
        public async Task TwcA100_03()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增問卷']");
            That(stormCard.Text, Is.EqualTo("新增問卷"));
        }
        public async Task TwcA100_04()
        {
            var nameStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷名稱']")));
            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("測試名稱" + Keys.Tab);

            var descriptionStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷頁首說明']")));
            var descriptionInput = descriptionStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            descriptionInput.SendKeys("測試頁首說明" + Keys.Tab);

            var textStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷結尾文字']")));
            var textInput = textStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            textInput.SendKeys("測試問卷結尾文字" + Keys.Tab);

            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form.multisteps-form__form > div:nth-child(1) div.button-row button.bg-gradient-dark");
            nextPageButton!.Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button[data-bs-target='#createMultipleChoice']")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div.multisteps-form__content div.row h5");
            That(contentTitle!.Text, Is.EqualTo("建立題目"));
        }

        public async Task TwcA100_05()
        {
            var createMultipleChoiceButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-bs-target='#createMultipleChoice']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", createMultipleChoiceButton);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button[id='closeButton']")));

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增題目']");
            That(stormCard!.Text, Is.EqualTo("新增題目"));
        }

        public async Task TwcA100_06()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            contentInput.SendKeys("題目1");

            var optionSelect = TestHelper.FindAndMoveToElement(_driver, "storm-select[label='選項數量'] >div.choices");
            optionSelect!.Click();

            var optionValue = TestHelper.FindAndMoveToElement(_driver, "div[data-value='3']");
            optionValue!.Click();
            Thread.Sleep(1000);

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionOneInput.SendKeys("同意" + Keys.Tab);

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionTwoInput.SendKeys("普通" + Keys.Tab);

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionThreeInput.SendKeys("不同意" + Keys.Tab);

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();
            Thread.Sleep(1000);

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.float-end > button")));
        }

        public async Task TwcA100_07()
        {
            await TwcA100_05();
        }
        public async Task TwcA100_08()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            contentInput.SendKeys("題目2");

            var optionSelect = TestHelper.FindAndMoveToElement(_driver, "storm-select[label='選項數量'] >div.choices");
            optionSelect!.Click();

            var optionValue = TestHelper.FindAndMoveToElement(_driver, "div[data-value='3']");
            optionValue!.Click();
            Thread.Sleep(1000);

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionOneInput.SendKeys("同意" + Keys.Tab);

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionTwoInput.SendKeys("普通" + Keys.Tab);

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionThreeInput.SendKeys("不同意" + Keys.Tab);

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();
            Thread.Sleep(1000);

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.float-end > button")));
        }
        public async Task TwcA100_09()
        {
            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form.multisteps-form__form > div:nth-child(2) div.button-row button.bg-gradient-dark");
            nextPageButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("form.multisteps-form__form > div:nth-child(2) div.button-row button.bg-gradient-dark")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div.multisteps-form__content div[slot='2'] h5");
            That(contentTitle!.Text, Is.EqualTo("問卷預覽"));
            //var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content div[slot='2'] h5")));
            //That(contentTitle.Text, Is.EqualTo("問卷預覽"));
        }
        public async Task TwcA100_10()
        {
            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form.multisteps-form__form > div:nth-child(3) div.button-row button.bg-gradient-dark");
            nextPageButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("form.multisteps-form__form > div:nth-child(3) div.button-row button.bg-gradient-dark")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div.multisteps-form__content div[slot='3'] h5");
            That(contentTitle!.Text, Is.EqualTo("問卷完成"));
        }
        public async Task TwcA100_11()
        {
            var sendButton = TestHelper.FindAndMoveToElement(_driver, "form.multisteps-form__form > div:nth-child(4) div.button-row button.bg-gradient-dark");
            sendButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("form.multisteps-form__form > div:nth-child(4) div.button-row button.bg-gradient-dark")));

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='問卷狀態']");
            That(stormCard.Text, Is.EqualTo("問卷狀態"));
        }

        public async Task TwcA100_12()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcA100_13()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A100_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcA100_14()
        {
            var logout = TestHelper.FindAndMoveToElement(_driver, "a[href='./logout']");
            logout!.Click();

            TestHelper.ChangeUser(_driver, "4e03");
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);

            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/now");

            var stormCarousel = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-carousel")));
            var videoAutoPlay = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video[autoplay]"));
            That(videoAutoPlay, Is.Not.Null, "影片正在撥放");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");
        }
        public async Task TwcA100_15()
        {
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveToElement(_driver, "[sti-apply-case-no]");
            //_wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA100_16()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);
            Thread.Sleep(1000);

            idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            idNoInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            var idNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no]")));
            That(idNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcA100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", accepted);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            accepted = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", accepted);

            var approver = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理'] [class='sign-name'] span")));
            That(approver.Text, Is.EqualTo("李麗花"));
        }
        public async Task TwcA100_18()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var contract_1 = TestHelper.FindAndMoveToElement(_driver, "[id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().DefaultContent();

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_1);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='消費性用水服務契約']")));
            That(contract_1.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_19()
        {
            var contract_2 = TestHelper.FindAndMoveToElement(_driver, "[id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_2);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_2 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司個人資料保護告知事項']")));
            That(contract_2.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_20()
        {
            var contract_3 = TestHelper.FindAndMoveToElement(_driver, "[id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", contract_3);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            contract_3 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='公司營業章程']")));
            That(contract_3.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_21()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signature = TestHelper.FindAndMoveToElement(_driver, "[id='signature'] button:nth-child(2)");
            signature!.Click();

            var img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            img = TestHelper.FindAndMoveToElement(_driver, "img[alt='簽名_001.tiff']");
            That(img, Is.Not.Null);
        }
        public async Task TwcA100_22()
        {
            var scanButton = TestHelper.FindAndMoveToElement(_driver, "[headline='掃描拍照'] button:nth-child(2)");
            scanButton!.Click();

            var scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            scanImg = TestHelper.FindAndMoveToElement(_driver, "storm-upload [alt='證件_005.tiff']");
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcA100_23()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var attachmentTab = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            That(attachmentTab!.Text, Is.EqualTo("新增文件"));

            var addAttachment = TestHelper.FindAndMoveToElement(_driver, "[headline='夾帶附件'] button");
            addAttachment!.Click();

            var attachment1 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, attachment1, "input.dz-hidden-input:nth-of-type(3)");

            var attachment2 = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, attachment2, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[headline='新增檔案'] storm-input-group")));
                return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var upload = TestHelper.FindAndMoveToElement(_driver, "[headline='新增檔案'] button");
            upload!.Click();

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var target = TestHelper.FindShadowElement(_driver, "stormEditTable", "div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";
            });
        }
        public async Task TwcA100_24()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "[headline='受理登記'] button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[headline='受理登記'] button")));

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));

            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = TestHelper.FindAndMoveToElement(_driver, "[sti-apply-case-no]");
            That(applyCaseNo!.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA100_25()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

            var questionnaireTiele = TestHelper.FindAndMoveToElement(_driver, "h3");
            That(questionnaireTiele!.Text, Is.EqualTo("用水設備各種異動服務申請滿意度問卷調查"));
        }
        public async Task TwcA100_26()
        {
            var questOne = TestHelper.FindAndMoveToElement(_driver, "form div[slot='0'] > div:nth-child(2) > div > div > div > input");
            questOne!.Click();

            var questTwo = TestHelper.FindAndMoveToElement(_driver, "form div[slot='0'] > div:nth-child(3) > div > div > div > input");
            questTwo!.Click();
        }
        public async Task TwcA100_27()
        {
            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "div.multisteps-form__content > div.button-row > button");
            nextPageButton!.Click();
            //((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", nextPageButton);

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.multisteps-form__content > div.button-row > button")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div[slot='1']");
            //_wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[slot='1']")));
            That(contentTitle!.Text, Is.EqualTo("填寫無誤後，提交問卷"));
        }
        public async Task TwcA100_28()
        {
            var sendButton = TestHelper.FindAndMoveToElement(_driver, "button[title='Send']");
            //_wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[title='Send']")));
            sendButton!.Click();
        }
        public async Task TwcA100_29()
        {
            Thread.Sleep(3000);
            _driver.Close();
        }
        public async Task TwcA100_30()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            logout!.Click();

            var login = TestHelper.FindAndMoveToElement(_driver, "div.text-center > button");
            That(login!.Text, Is.EqualTo("登入"));
        }

        public async Task TwcA100_31()
        {
            TestHelper.ChangeUser(_driver, "meizi");
        }
        public async Task TwcA100_32()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='問卷狀態']");
            That(stormCard.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcA100_33()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var deleteButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(4)"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            That(confirmButton!.Text, Is.EqualTo("刪除"));
        }
        public async Task TwcA100_34()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack > button")));

            var errorMessage = TestHelper.FindAndMoveToElement(_driver, "[class='swal2-html-container'] h5");
            That(errorMessage!.Text, Is.EqualTo("已有問卷資料不得刪除"));
        }
        public async Task TwcA100_35()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.swal2-actions > button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.swal2-actions > button")));

            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var chartButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(3)"));
            _actions.MoveToElement(chartButton).Click().Perform();

            var stormChart = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-chart")));
            var canvas = stormChart.GetShadowRoot().FindElement(By.CssSelector("canvas"));
            That(canvas, Is.Not.Null);
        }
        public async Task TwcA100_36()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var chartButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(2)"));
            _actions.MoveToElement(chartButton).Click().Perform();
        }
        public async Task TwcA100_37()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-dialog-wrapper button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-dialog-wrapper button")));

            var date = TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='planDisableDate'] span");
            That(date!.Text, Is.Not.Empty);
        }
    }
}