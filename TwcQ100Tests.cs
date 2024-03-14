using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcQ100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;

        public TwcQ100Tests()
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
        public async Task TwcQ100_01To18()
        {
            await TwcQ100_01();
            await TwcQ100_02();
            await TwcQ100_03();
            await TwcQ100_04();
            await TwcQ100_05();
            await TwcQ100_06();
            await TwcQ100_07();
            await TwcQ100_08();
            await TwcQ100_09();
            await TwcQ100_10();
            await TwcQ100_11();
            await TwcQ100_12();
            await TwcQ100_13();
            await TwcQ100_14();
            await TwcQ100_15();
            await TwcQ100_16();
            await TwcQ100_17();
            await TwcQ100_18();
            await TwcQ100_19();
            await TwcQ100_20();
            await TwcQ100_21();
            await TwcQ100_22();
            await TwcQ100_23();
            await TwcQ100_24();
        }

        public async Task TwcQ100_01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='系統公告']");
            That(stormCard.Text, Is.EqualTo("系統公告"));
        }

        public async Task TwcQ100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增問卷']");
            That(stormCard.Text, Is.EqualTo("新增問卷"));
        }
        public async Task TwcQ100_03()
        {
            var nameStormInputGroup = TestHelper.FindAndMoveToElement(_driver, "storm-input-group[label='問卷名稱']");
            var nameInput = nameStormInputGroup!.GetShadowRoot().FindElement(By.CssSelector("input"));
            nameInput.SendKeys("這是問卷名稱");

            var descriptionStormInputGroup = TestHelper.FindAndMoveToElement(_driver, "storm-input-group[label='問卷頁首說明']");
            var descriptionInput = descriptionStormInputGroup!.GetShadowRoot().FindElement(By.CssSelector("input"));
            descriptionInput.SendKeys("這是問卷頁首說明");

            var textStormInputGroup = TestHelper.FindAndMoveToElement(_driver, "storm-input-group[label='問卷結尾文字']");
            var textInput = textStormInputGroup!.GetShadowRoot().FindElement(By.CssSelector("input"));
            textInput.SendKeys("這是問卷結尾文字");

            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form > div:nth-child(1) > div > div.button-row > button");
            nextPageButton!.Click();
            //var nextPageButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form > div:nth-child(1) > div > div.button-row > button")));
            //_actions.MoveToElement(nextPageButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[slot='0']")));

            var slotTitle = TestHelper.FindAndMoveToElement(_driver, "div[slot='1'] h5");
            That(slotTitle!.Text, Is.EqualTo("建立題目"));

            //var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[slot='1'] h5")));
            //That(contentTitle.Text, Is.EqualTo("建立題目"));
        }
        public async Task TwcQ100_04()
        {
            var createButton = TestHelper.FindAndMoveToElement(_driver, "form.multisteps-form__form > div:nth-child(2) > div.multisteps-form__content > div[slot='1'] > div.row > div.col >div.text-end > button");
            createButton!.Click();
            Thread.Sleep(1000);

            //var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("form.multisteps-form__form > div:nth-child(2) > div.multisteps-form__content > div[slot='1'] > div.row > div.col >div.text-end > button")));
            //_actions.MoveToElement(createButton).Click().Perform();

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='新增題目']");
            That(stormCard.Text, Is.EqualTo("新增題目"));
        }
        public async Task TwcQ100_05()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            contentInput.SendKeys("整體而言，我對本次活動非常滿意");

            var optionSelect = TestHelper.FindAndMoveToElement(_driver, "storm-select[label='選項數量'] >div.choices");
            optionSelect!.Click();

            //var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            //_actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = TestHelper.FindAndMoveToElement(_driver, "div[data-value='5']");
            optionValue!.Click();
            Thread.Sleep(1000);

            //var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            //_actions.MoveToElement(optionValue).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionOneInput.SendKeys("非常同意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionTwoInput.SendKeys("同意");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionThreeInput.SendKeys("普通");

            var optionFourStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4']")));
            var optionFourInput = optionFourStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFourInput.SendKeys("不同意");

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();

            //var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            //_actions.MoveToElement(addButton).Click().Perform();

            var validationMessage = TestHelper.FindAndMoveToElement(_driver, "li.validation-message");
            //var validationMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("li.validation-message")));
            That(validationMessage!.Text, Is.EqualTo("尚有選項未輸入"));
        }
        public async Task TwcQ100_06()
        {
            var optionFiveStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5']")));
            var optionFiveInput = optionFiveStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFiveInput.SendKeys("非常不同意");

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();
            Thread.Sleep(1000);

            //var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            //_actions.MoveToElement(addButton).Click().Perform();

            var content = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3 > div.col > h5");
            var optionOne = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.flex-wrap > div > label");
            var optionTwo = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.flex-wrap > div:nth-child(2) > label");
            var optionThree = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.flex-wrap > div:nth-child(3) > label");
            var optionFour = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.flex-wrap > div:nth-child(4) > label");
            var optionFive = TestHelper.FindAndMoveToElement(_driver, "div.d-flex.flex-wrap > div:nth-child(5) > label");

            //var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(1) > label")));
            //var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(2) > label")));
            //var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(3) > label")));
            //var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(4) > label")));
            //var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(5) > label")));

            That(content!.Text, Is.EqualTo("整體而言，我對本次活動非常滿意"));
            That(optionOne!.Text, Is.EqualTo("非常同意"));
            That(optionTwo!.Text, Is.EqualTo("同意"));
            That(optionThree!.Text, Is.EqualTo("普通"));
            That(optionFour!.Text, Is.EqualTo("不同意"));
            That(optionFive!.Text, Is.EqualTo("非常不同意"));

            var questionTitle = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3 > div > h5");
            //var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3 > div > h5")));
            That(questionTitle!.Text, Is.EqualTo("整體而言，我對本次活動非常滿意"));
        }
        public async Task TwcQ100_07()
        {
            await TwcQ100_04();
        }
        public async Task TwcQ100_08()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            contentInput.SendKeys("本次活動內容對我有幫助");

            var optionSelect = TestHelper.FindAndMoveToElement(_driver, "storm-select[label='選項數量'] >div.choices");
            optionSelect!.Click();

            var optionValue = TestHelper.FindAndMoveToElement(_driver, "div[data-value='5']");
            optionValue!.Click();
            Thread.Sleep(1000);

            //var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            //_actions.MoveToElement(optionSelect).Click().Perform();

            //var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            //_actions.MoveToElement(optionValue).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionOneInput.SendKeys("非常同意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionTwoInput.SendKeys("同意");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionThreeInput.SendKeys("普通");

            var optionFourStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4']")));
            var optionFourInput = optionFourStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFourInput.SendKeys("不同意");

            var optionFiveStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5']")));
            var optionFiveInput = optionFiveStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFiveInput.SendKeys("非常不同意");

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();
            Thread.Sleep(1000);

            //var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            //_actions.MoveToElement(addButton).Click().Perform();

            var content = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > h5");
            var optionOne = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div > label");
            var optionTwo = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(2) > label");
            var optionThree = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(3) > label");
            var optionFour = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(4) > label");
            var optionFive = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(5) > label");

            //var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(1) > label")));
            //var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(2) > label")));
            //var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(3) > label")));
            //var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(4) > label")));
            //var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(5) > label")));

            That(content!.Text, Is.EqualTo("本次活動內容對我有幫助"));
            That(optionOne!.Text, Is.EqualTo("非常同意"));
            That(optionTwo!.Text, Is.EqualTo("同意"));
            That(optionThree!.Text, Is.EqualTo("普通"));
            That(optionFour!.Text, Is.EqualTo("不同意"));
            That(optionFive!.Text, Is.EqualTo("非常不同意"));

            var questionTitle = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(2) > div > h5");

            //var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div > h5")));
            That(questionTitle!.Text, Is.EqualTo("本次活動內容對我有幫助"));
        }
        public async Task TwcQ100_09()
        {
            await TwcQ100_04();
        }
        public async Task TwcQ100_10()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            contentInput.SendKeys("本次活動讓我收穫豐富");

            var optionSelect = TestHelper.FindAndMoveToElement(_driver, "storm-select[label='選項數量'] >div.choices");
            optionSelect!.Click();

            var optionValue = TestHelper.FindAndMoveToElement(_driver, "div[data-value='5']");
            optionValue!.Click();
            Thread.Sleep(1000);

            //var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            //_actions.MoveToElement(optionSelect).Click().Perform();

            //var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            //_actions.MoveToElement(optionValue).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionOneInput.SendKeys("非常同意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionTwoInput.SendKeys("同意");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionThreeInput.SendKeys("普通");

            var optionFourStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4']")));
            var optionFourInput = optionFourStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFourInput.SendKeys("不同意");

            var optionFiveStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5']")));
            var optionFiveInput = optionFiveStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFiveInput.SendKeys("非常不同意");

            var addButton = TestHelper.FindAndMoveToElement(_driver, "div.float-end > button");
            addButton!.Click();
            Thread.Sleep(1000);

            //var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            //_actions.MoveToElement(addButton).Click().Perform();

            var content = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > h5");
            var optionOne = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(1) > label");
            var optionTwo = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(2) > label");
            var optionThree = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(3) > label");
            var optionFour = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(4) > label");
            var optionFive = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(5) > label");

            That(content!.Text, Is.EqualTo("本次活動讓我收穫豐富"));
            That(optionOne!.Text, Is.EqualTo("非常同意"));
            That(optionTwo!.Text, Is.EqualTo("同意"));
            That(optionThree!.Text, Is.EqualTo("普通"));
            That(optionFour!.Text, Is.EqualTo("不同意"));
            That(optionFive!.Text, Is.EqualTo("非常不同意"));

            var questionTitle = TestHelper.FindAndMoveToElement(_driver, "div.row.mx-4.mb-3:nth-child(3) > div > h5");
            //var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div > h5")));
            That(questionTitle!.Text, Is.EqualTo("本次活動讓我收穫豐富"));
        }
        public async Task TwcQ100_11()
        {
            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form > div:nth-child(2) button[title='Next']");
            nextPageButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[slot='1']")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div[slot='2'] h5");
            //var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[slot='2'] h5")));
            That(contentTitle!.Text, Is.EqualTo("問卷預覽"));
        }
        public async Task TwcQ100_12()
        {
            var nextPageButton = TestHelper.FindAndMoveToElement(_driver, "form > div:nth-child(3) button[title='Next']");
            nextPageButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[slot='2']")));

            var contentTitle = TestHelper.FindAndMoveToElement(_driver, "div[slot='3'] h5");
            That(contentTitle!.Text, Is.EqualTo("問卷完成"));
        }
        public async Task TwcQ100_13()
        {
            var submitButton = TestHelper.FindAndMoveToElement(_driver, "div.multisteps-form__content > div.button-row.d-flex.mt-0 > button.bg-gradient-dark");
            submitButton!.Click();

            //var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.multisteps-form__content > div.button-row.d-flex.mt-0 > button.bg-gradient-dark")));
            //_actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[slot='3']")));

            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='問卷狀態']");
            //var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
            //var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCard.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcQ100_14()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var viewButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item"));
            //var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='問卷狀態']");
            //var viewButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_6'] storm-button");
            _actions.MoveToElement(viewButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            //var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content div:nth-child(6) span")));
            //That(checkButton!.Text, Is.EqualTo("確定"));

            confirmButton!.Click();
            //_actions.MoveToElement(checkButton).Click().Perform();
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));
        }

        public async Task TwcQ100_15()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var takeDownButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(2)"));
            _actions.MoveToElement(takeDownButton).Click().Perform();

            //var takeDownButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-button:nth-child(2)");
            //_actions.MoveToElement(takeDownButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            That(confirmButton!.Text, Is.EqualTo("確認"));
            //var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack button")));
            //That(checkButton.Text, Is.EqualTo("確認"));
        }
        public async Task TwcQ100_16()
        {
            var cancelButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button:nth-child(2)");
            //var cancelButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-stack button:nth-child(2)")));
            cancelButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button:nth-child(2)")));

            //var checkButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-content button")));
            //_actions.MoveToElement(checkButton).Click().Perform();

            //_wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-dialog-content > div > button")));

            //var planDisableDate = TestHelper.FindShadowElement(_driver, "stormTable", "td[data-field='planDisableDate'] span");
            ////var planDisableDate = TestHelper.WaitStormTableUpload(_driver, "td[data-field='planDisableDate'] span");
            //That(planDisableDate!.Text, Is.EqualTo("-"));
        }
        public async Task TwcQ100_17()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var takeDownButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(2)"));
            _actions.MoveToElement(takeDownButton).Click().Perform();

            DateTime currentDateTime = DateTime.Now;

            var expiryDate = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期']")));
            var expiryDateInput = expiryDate.GetShadowRoot().FindElement(By.CssSelector("div input"));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            That(confirmButton!.Text, Is.EqualTo("確認"));
        }
        public async Task TwcQ100_18()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));
        }
        public async Task TwcQ100_19()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

            var title = TestHelper.FindAndMoveToElement(_driver, "h3");
            //_wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3")));
            That(title!.Text, Is.EqualTo("用水設備各種異動服務申請滿意度問卷調查"));
        }
        public async Task TwcQ100_20()
        {
            var card = TestHelper.FindAndMoveToElement(_driver, "div.card-body");
            //_wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body")));
            That(card, Is.Not.Null);
        }
        public async Task TwcQ100_21()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-sidenav")));

            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
            That(logout, Is.Not.Null);
        }
        public async Task TwcQ100_22()
        {
            var stormCard = TestHelper.FindShadowElement(_driver, "stormCard", "[headline='問卷狀態']");
            That(stormCard.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcQ100_23()
        {
            var stormToolbar = TestHelper.FindShadowElement(_driver, "stormTable", "storm-toolbar");
            var deleteButton = stormToolbar.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-child(4)"));
            _actions.MoveToElement(deleteButton).Click().Perform();
            //var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_6'] storm-button:nth-child(4)");
            //_actions.MoveToElement(deleteButton).Click().Perform();

            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            That(confirmButton!.Text, Is.EqualTo("刪除"));
        }
        public async Task TwcQ100_24()
        {
            var confirmButton = TestHelper.FindAndMoveToElement(_driver, "div.rz-stack button");
            confirmButton!.Click();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));

            _wait.Until(driver => {
                var pageInfo = TestHelper.FindShadowElement(_driver, "stormTable", "div.table-pageInfo");
                return pageInfo!.Text == "共 0 筆";
            });
        }
    }
}