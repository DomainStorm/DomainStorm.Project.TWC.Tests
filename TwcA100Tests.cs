using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
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
        public async Task TwcA100_01()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > div > button")));

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > div > button");
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var uploadButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo")!.Text,Is.EqualTo("顯示第 1 至 1 筆，共 1 筆"));

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            var addListButton = TestHelper.FindAndMoveElement(_driver, "button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
            _actions.MoveToElement(addListButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));

            var addMediaButton = TestHelper.FindAndMoveElement(_driver, "button");
            _actions.MoveToElement(addMediaButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack > storm-table")));
            var stormTableCell = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody > tr > td[data-field='name']"));
            _actions.MoveToElement(stormTableCell).Click().Perform();

            var addButton = TestHelper.FindAndMoveElement(_driver, "span.rz-button-box");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("span.rz-button-box")));
            _actions.MoveToElement(addButton).Click().Perform();

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group")));
            var stormInputGroupInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            stormInputGroupInput.SendKeys("節目單測試");

            var stormTextEditorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor > div.ql-container > div.ql-editor")));
            stormTextEditorInput.SendKeys("跑馬燈測試");

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button.bg-gradient-info");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.bg-gradient-info")));
            _actions.MoveToElement(submitButton).Click().Perform();
            var pageInfo = TestHelper.WaitStormTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => pageInfo.Text == "顯示第 1 至 1 筆，共 1 筆");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");

            var approveButton = TestHelper.WaitStormTableUpload(_driver, "div.table-responsive storm-button:nth-child(3)");
            _actions.MoveToElement(approveButton).Click().Perform();

            var approveTrueButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-wrapper button")));
            _actions.MoveToElement(approveTrueButton).Click().Perform();
            var status = TestHelper.WaitStormTableUpload(_driver, "div.table-responsive td[data-field='playListStatus'] span");
            _wait.Until(driver => status!.Text == "核准");
            That(TestHelper.WaitStormTableUpload(_driver, "div.table-responsive td[data-field='playListStatus'] span")!.Text, Is.EqualTo("核准"));
        }

        [Test]
        [Order(1)]
        public async Task TwcA100_02To11()
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
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            That(logout.Text, Is.EqualTo("logout"));
        }
        public async Task TwcA100_03()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增問卷']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
        }
        public async Task TwcA100_04()
        {
            var nameStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷名稱']");
            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            nameInput.SendKeys("測試名稱");

            var descriptionStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷頁首說明']");
            var descriptionInput = descriptionStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            descriptionInput.SendKeys("測試頁首說明");

            var textStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷結尾文字']");
            var textInput = textStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            textInput.SendKeys("測試問卷結尾文字");

            var nextPageButton = TestHelper.FindAndMoveElement(_driver, "form.multisteps-form__form > div:nth-child(1) div.button-row button.bg-gradient-dark");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(1) div.button-row button.bg-gradient-dark")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content div.row h5")));
            That(contentTitle.Text, Is.EqualTo("建立題目"));
        }

        public async Task TwcA100_05()
        {
            var addButton = TestHelper.FindAndMoveElement(_driver, "div.text-end button");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.text-end button")));
            _actions.MoveToElement(addButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
        }

        public async Task TwcA100_06()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            contentInput.SendKeys("題目1");

            var optionSelect = TestHelper.FindAndMoveElement(_driver, "storm-select[label='選項數量'] > div.choices");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] > div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = TestHelper.FindAndMoveElement(_driver, "div[data-value='3']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionOneInput.SendKeys("同意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionThreeInput.SendKeys("不同意");

            var addButton = TestHelper.FindAndMoveElement(_driver, "div.float-end > button");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();
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

            var optionSelect = TestHelper.FindAndMoveElement(_driver, "storm-select[label='選項數量'] > div.choices");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] > div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = TestHelper.FindAndMoveElement(_driver, "div[data-value='3']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionOneInput.SendKeys("同意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div input"));
            optionThreeInput.SendKeys("不同意");

            var addButton = TestHelper.FindAndMoveElement(_driver, "div.float-end > button");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();
        }
        public async Task TwcA100_09()
        {
            var nextPageButton = TestHelper.FindAndMoveElement(_driver, "form.multisteps-form__form > div:nth-child(2) div.button-row button.bg-gradient-dark");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(2) div.button-row button.bg-gradient-dark")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content div[slot='2'] h5")));
            That(contentTitle.Text, Is.EqualTo("問卷預覽"));
        }
        public async Task TwcA100_10()
        {
            var nextPageButton = TestHelper.FindAndMoveElement(_driver, "form.multisteps-form__form > div:nth-child(3) div.button-row button.bg-gradient-dark");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(3) div.button-row button.bg-gradient-dark")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content div[slot='3'] h5")));
            That(contentTitle.Text, Is.EqualTo("問卷完成"));
        }
        public async Task TwcA100_11()
        {
            var sendButton = TestHelper.FindAndMoveElement(_driver, "form.multisteps-form__form > div:nth-child(4) div.button-row button.bg-gradient-dark");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(4) div.button-row button.bg-gradient-dark")));
            _actions.MoveToElement(sendButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='問卷狀態']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }

        [Test]
        [Order(2)]
        public async Task TwcA100_12To30()
        {
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
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
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

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA100_16()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiTrusteeIdNoInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-trustee-id-no] > input")));
            stiTrusteeIdNoInput.SendKeys("A123456789" + Keys.Tab);
            Thread.Sleep(1000);

            stiTrusteeIdNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-trustee-id-no] > input")));
            stiTrusteeIdNoInput.SendKeys(Keys.Tab);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            var stiTrusteeIdNo = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[sti-trustee-id-no]")));
            That(stiTrusteeIdNo.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcA100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            Thread.Sleep(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var signName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.sign-name > span")));
            That(signName.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcA100_18()
        {            
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "input[id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().DefaultContent();

            消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            消費性用水服務契約 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='消費性用水服務契約']")));
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_19()
        {
            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "input[id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司個人資料保護告知事項 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司個人資料保護告知事項']")));
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_20()
        {
            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "input[id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            公司營業章程 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 公司營業章程);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            公司營業章程 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='公司營業章程']")));
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcA100_21()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var signButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='signature'] button.btn-primary")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", signButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView();", signButton);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dz-image > img[alt='簽名_001.tiff']")));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signImg = TestHelper.FindAndMoveElement(_driver, "div.dz-image > img[alt='簽名_001.tiff']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dz-image > img[alt='簽名_001.tiff']")));
            That(signImg, Is.Not.Null);
        }
        public async Task TwcA100_22()
        {
            var scanButton = TestHelper.FindAndMoveElement(_driver, "storm-card[id='credential'] > form > div > div > button.btn-primary");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark");
            That(scanSuccess, Is.Not.Null, "未上傳");

            var scanImg = TestHelper.FindAndMoveElement(_driver, "div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-image > img")));
            That(scanImg, Is.Not.Null, "尚未上傳完成");
        }
        public async Task TwcA100_23()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var fileName = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增檔案'] > form > div > storm-input-group");
                return fileName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            var fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
            _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
            That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
        }
        public async Task TwcA100_24()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var submitButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _actions.MoveToElement(submitButton).Click().Perform();

            var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcA100_25()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

            var questionnaireTiele = TestHelper.FindAndMoveElement(_driver, "h3");
            That(questionnaireTiele.Text, Is.EqualTo("用水設備各種異動服務申請滿意度問卷調查"));
        }
        public async Task TwcA100_26()
        {
            var questOne = TestHelper.FindAndMoveElement(_driver, "form div[slot='0'] > div:nth-child(2) > div > div > div > input");
            _actions.MoveToElement(questOne).Click().Perform();

            var questTwo = TestHelper.FindAndMoveElement(_driver, "form div[slot='0'] > div:nth-child(3) > div > div > div > input");
            _actions.MoveToElement(questTwo).Click().Perform();
        }
        public async Task TwcA100_27()
        {
            var nextPageButton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__content > div.button-row > button");
            _actions.MoveToElement(nextPageButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[slot='1']")));
            That(contentTitle.Text, Is.EqualTo("填寫無誤後，提交問卷"));
        }
        public async Task TwcA100_28()
        {
            var sendButton = TestHelper.FindAndMoveElement(_driver, "button[title='Send']");
            _actions.MoveToElement(sendButton).Click().Perform();

            var hintContent = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h2[class='swal2-title']")));
            That(hintContent.Text, Is.EqualTo("提交完成！"));
        }
        public async Task TwcA100_29()
        {
            _driver.Close();
        }
        public async Task TwcA100_30()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var logout = TestHelper.FindAndMoveElement(_driver, "storm-tooltip > div > a[href='./logout']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            var logingButton = TestHelper.FindAndMoveElement(_driver, "div.text-center > button");
            That(logingButton.Text, Is.EqualTo("登入"));
        }

        [Test]
        [Order(3)]
        public async Task TwcA100_31To37()
        {
            await TwcA100_31();
            await TwcA100_32();
            await TwcA100_33();
            await TwcA100_34();
            await TwcA100_35();
            await TwcA100_36();
            await TwcA100_37();
        }
        public async Task TwcA100_31()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            That(logout.Text, Is.EqualTo("logout"));
        }
        public async Task TwcA100_32()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormCard = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='問卷狀態']");
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcA100_33()
        {
            var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-button:nth-child(4)");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var hintContent = TestHelper.FindAndMoveElement(_driver, "div.rz-stack > h5");
            That(hintContent.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcA100_34()
        {
            var checkDelete = TestHelper.FindAndMoveElement(_driver, "div.rz-stack > button");
            _actions.MoveToElement(checkDelete).Click().Perform();

            var hintContent = TestHelper.FindAndMoveElement(_driver, "div.swal2-html-container > h5");
            That(hintContent.Text, Is.EqualTo("已有問卷資料不得刪除"));
        }
        public async Task TwcA100_35()
        {
            var button = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button");
            _actions.MoveToElement(button).Click().Perform();

            var chartButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-button:nth-child(3)");
            _actions.MoveToElement(chartButton).Click().Perform();

            var stormChart = TestHelper.FindAndMoveElement(_driver, "storm-chart");
            var canvas = stormChart.GetShadowRoot().FindElement(By.CssSelector("canvas"));
            That(canvas, Is.Not.Null);
        }
        public async Task TwcA100_36()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var takeDownButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-button:nth-child(2)");
            _actions.MoveToElement(takeDownButton).Click().Perform();

            var hintContent = TestHelper.FindAndMoveElement(_driver, "p.rz-dialog-confirm-message");
            That(hintContent.Text, Is.EqualTo("確認是否要下架？"));
        }
        public async Task TwcA100_37()
        {
            var chcekButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-confirm-buttons > button");
            _actions.MoveToElement(chcekButton).Click().Perform();

            var checkTakeDown = TestHelper.FindAndMoveElement(_driver, "div.rz-align-items-normal > button");
            _actions.MoveToElement(checkTakeDown).Click().Perform();
        }
    }
}