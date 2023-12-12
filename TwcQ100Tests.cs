using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Globalization;
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
        }

        public async Task TwcQ100_01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
            That(logout.Text, Is.EqualTo("logout"));
        }

        public async Task TwcQ100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增問卷']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
        }
        public async Task TwcQ100_03()
        {
            var nameStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷名稱']");
            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            nameInput.SendKeys("這是問卷說明");

            var descriptionStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷頁首說明']");
            var descriptionInput = descriptionStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            descriptionInput.SendKeys("這是問卷頁首說明");

            var textStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷結尾文字']");
            var textInput = textStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            textInput.SendKeys("這是問卷結尾文字");

            var nextButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form > div:nth-child(1) > div > div.button-row > button")));
            _actions.MoveToElement(nextButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content > div > div > div > div > h5")));
            That(contentTitle.Text, Is.EqualTo("建立題目"));
        }
        public async Task TwcQ100_04()
        {
            var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("form.multisteps-form__form > div:nth-child(2) > div.multisteps-form__content > div[slot='1'] > div.row > div.col >div.text-end > button")));
            _actions.MoveToElement(createButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
        }
        public async Task TwcQ100_05()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            contentInput.SendKeys("整體而言，我對本次活動非常滿意");

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

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

            var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();

            var validationMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("li.validation-message")));
            That(validationMessage.Text, Is.EqualTo("尚有選項未輸入"));
        }
        public async Task TwcQ100_06()
        {
            var optionFiveStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5']")));
            var optionFiveInput = optionFiveStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            optionFiveInput.SendKeys("非常不同意");

            var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3 > div.col > h5")));
            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(1) > label")));
            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(2) > label")));
            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(3) > label")));
            var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(4) > label")));
            var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(5) > label")));

            That(content.Text, Is.EqualTo("整體而言，我對本次活動非常滿意"));
            That(optionOne.Text, Is.EqualTo("非常同意"));
            That(optionTwo.Text, Is.EqualTo("同意"));
            That(optionThree.Text, Is.EqualTo("普通"));
            That(optionFour.Text, Is.EqualTo("不同意"));
            That(optionFive.Text, Is.EqualTo("非常不同意"));

            var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3 > div > h5")));
            That(questionTitle.Text, Is.EqualTo("整體而言，我對本次活動非常滿意"));
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

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

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

            var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > h5")));
            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(1) > label")));
            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(2) > label")));
            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(3) > label")));
            var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(4) > label")));
            var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div.col > div.d-flex.flex-wrap > div:nth-child(5) > label")));

            That(content.Text, Is.EqualTo("本次活動內容對我有幫助"));
            That(optionOne.Text, Is.EqualTo("非常同意"));
            That(optionTwo.Text, Is.EqualTo("同意"));
            That(optionThree.Text, Is.EqualTo("普通"));
            That(optionFour.Text, Is.EqualTo("不同意"));
            That(optionFive.Text, Is.EqualTo("非常不同意"));

            var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(2) > div > h5")));
            That(questionTitle.Text, Is.EqualTo("本次活動內容對我有幫助"));
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

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

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

            var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.float-end > button")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > h5")));
            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(1) > label")));
            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(2) > label")));
            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(3) > label")));
            var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(4) > label")));
            var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div.col > div.d-flex.flex-wrap > div:nth-child(5) > label")));

            That(content.Text, Is.EqualTo("本次活動讓我收穫豐富"));
            That(optionOne.Text, Is.EqualTo("非常同意"));
            That(optionTwo.Text, Is.EqualTo("同意"));
            That(optionThree.Text, Is.EqualTo("普通"));
            That(optionFour.Text, Is.EqualTo("不同意"));
            That(optionFive.Text, Is.EqualTo("非常不同意"));

            var questionTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row.mx-4.mb-3:nth-child(3) > div > h5")));
            That(questionTitle.Text, Is.EqualTo("本次活動讓我收穫豐富"));
        }
        public async Task TwcQ100_11()
        {
            var nextButton = TestHelper.FindAndMoveElement(_driver, "form > div:nth-child(2) button[title='Next']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("form > div:nth-child(2) button[title='Next']")));
            _actions.MoveToElement(nextButton).Click().Perform();

            var contentTitle = TestHelper.FindAndMoveElement(_driver, "div.mt-5 > div.col > h4");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.mt-5 > div.col > h4")));
            That(contentTitle.Text, Is.EqualTo("這是問卷結尾文字"));
        }
        public async Task TwcQ100_12()
        {
            var nextButton = TestHelper.FindAndMoveElement(_driver, "form > div:nth-child(3) button[title='Next']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("form > div:nth-child(3) button[title='Next']")));
            _actions.MoveToElement(nextButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.multisteps-form__content > div[slot='3'] > h5")));
            That(contentTitle.Text, Is.EqualTo("問卷完成"));
        }
        public async Task TwcQ100_13()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.multisteps-form__content > div.button-row.d-flex.mt-0 > button.bg-gradient-dark")));
            _actions.MoveToElement(submitButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcQ100_14()
        {
            TestHelper.WaitStormTableUpload(_driver, "table > tbody > tr > td[data-field='name'] > storm-table-cell > span");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var watchButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr > td[data-field='__action_6'] > storm-table-cell > storm-table-toolbar > storm-button:nth-child(1) > storm-tooltip > div > button"));
            _actions.MoveToElement(watchButton).Click().Perform();

            var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content > div > div:nth-child(6) > div > button > span")));
            That(checkButton.Text, Is.EqualTo("確定"));
            _actions.MoveToElement(checkButton).Click().Perform();
        }

        public async Task TwcQ100_15()
        {
            var takeDownButton = TestHelper.WaitStormTableUpload(_driver, "tbody > tr > td[data-field='__action_6'] > storm-table-cell > storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button");
            _actions.MoveToElement(takeDownButton).Click().Perform();

            var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content > div > button > span > span")));
            That(checkButton.Text, Is.EqualTo("確認"));
        }
        public async Task TwcQ100_16()
        {
            var cancelButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-content > div > button:nth-child(2) > span > span");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-content > div > button:nth-child(2) > span > span")));
            _actions.MoveToElement(cancelButton).Click().Perform();

            var planDisableDate = TestHelper.WaitStormTableUpload(_driver, "tbody > tr > td[data-field='planDisableDate'] > storm-table-cell > span");
            That(planDisableDate.Text, Is.EqualTo("-"));
        }
        public async Task TwcQ100_17()
        {
            var takeDownButton = TestHelper.WaitStormTableUpload(_driver, "tbody > tr > td[data-field='__action_6'] > storm-table-cell > storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button");
            _actions.MoveToElement(takeDownButton).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-content > div > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-content > div > button")));
            _actions.MoveToElement(checkButton).Click().Perform();

            var takeDownDateChoose = TestHelper.FindAndMoveElement(_driver, "[label='下架日期']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[label='下架日期']")));
            var input = takeDownDateChoose.GetShadowRoot().FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();

            var cultureInfo = new CultureInfo("zh-TW");
            var monthInChinese = TestHelper.GetChineseMonth(DateTime.Now.Month);
            var today = $"{monthInChinese}月 {DateTime.Now.Day}, {DateTime.Now.Year}";

            var takeDownDate = TestHelper.FindAndMoveElement(_driver, $"div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='{today}']");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='{today}']")));
            _actions.MoveToElement(takeDownDate).Click().Perform();
        }
        public async Task TwcQ100_18()
        {
            var confirmButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-confirm-buttons > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-dialog-confirm-buttons > button")));
            _actions.MoveToElement(confirmButton).Click().Perform();

            var takeDownButton = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper > div > div > div > div > div > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.rz-stack button")));
            _actions.MoveToElement(takeDownButton).Click().Perform();
            //var deleteButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-dialog-content > div > div > div > button > span > span")));
            //_actions.MoveToElement(deleteButton).Click().Perform();

            //var checkDetele = TestHelper.WaitStormTableUpload(_driver, "td > p");
            //That(checkDetele!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
    }
}