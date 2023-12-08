using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
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

            var addFileButton = TestHelper.FindAndMoveElement(_driver, "storm-card > div > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card > div > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var uploadButton = TestHelper.FindAndMoveElement(_driver, "button[type='submit']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            Thread.Sleep(1000);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            var addListButton = TestHelper.FindAndMoveElement(_driver, "button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
            _actions.MoveToElement(addListButton).Click().Perform();

            var addMediaButton = TestHelper.FindAndMoveElement(_driver, "button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button")));
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

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");

            var approveButton = TestHelper.WaitStormTableUpload(_driver, "div.table-responsive storm-button:nth-child(3)");
            _actions.MoveToElement(approveButton).Click().Perform();

            var approveTrueButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-wrapper button")));
            _actions.MoveToElement(approveTrueButton).Click().Perform();
            Thread.Sleep(1000);
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

            var stormCard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增問卷']")));
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
        }
        public async Task TwcA100_11()
        {
        }
        public async Task TwcA100_12()
        {
        }
        public async Task TwcA100_13()
        {
        }
        public async Task TwcA100_14()
        {
        }
        public async Task TwcA100_15()
        {
        }
        public async Task TwcA100_16()
        {
        }
        public async Task TwcA100_17()
        {
        }
        public async Task TwcA100_18()
        {
        }
        public async Task TwcA100_19()
        {
        }
        public async Task TwcA100_20()
        {
        }
        public async Task TwcA100_21()
        {
        }
        public async Task TwcA100_22()
        {
        }
        public async Task TwcA100_23()
        {
        }
        public async Task TwcA100_24()
        {
        }
        public async Task TwcA100_25()
        {
        }
        public async Task TwcA100_26()
        {
        }
        public async Task TwcA100_27()
        {
        }
        public async Task TwcA100_28()
        {
        }
        public async Task TwcA100_29()
        {
        }
        public async Task TwcA100_30()
        {
        }
        public async Task TwcA100_31()
        {
        }
        public async Task TwcA100_32()
        {
        }
        public async Task TwcA100_33()
        {
        }
        public async Task TwcA100_34()
        {
        }
        public async Task TwcA100_35()
        {
        }
        public async Task TwcA100_36()
        {
        }
        public async Task TwcA100_37()
        {
        }
    }
}