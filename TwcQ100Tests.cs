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
        public async Task TwcQ100_01To07()
        {
            await _01();
            await _02();
            await _03();
            await _04();
            await _05();
            await _06();
            await _07();
        }

        [Test]
        [Order(1)]
        public async Task TwcQ100_08()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            //var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            IWebElement? stormTable = null;
            _wait.Until(_ => {
                Console.WriteLine($"::group::問卷Page------------------");
                Console.WriteLine(_driver.PageSource);
                Console.WriteLine("::endgroup::");
                return stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            });
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(1) > storm-tooltip > div > button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var dialogContent = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper > div.rz-dialog > div > div > div.row.mx-4.mb-5 > div > h4");

            That(dialogContent.Text, Is.EqualTo("頁首說明1120824"));
        }

        [Test]
        [Order(2)]
        public async Task TwcQ100_09()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));

            var updateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button"));
            _actions.MoveToElement(updateButton).Click().Perform();

            var updateButton2 = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper > div.rz-dialog > div.rz-dialog-content > div > div > div > button");
            _actions.MoveToElement(updateButton2).Click().Perform();

            string? planDisableDateCellText = default;
            _wait.Until((_) =>
            {
                planDisableDateCellText = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-responsive > div > table > tbody > tr > td:nth-child(5) > storm-table-cell > span")).Text;
                return planDisableDateCellText != "-";
            });
            

            That(planDisableDateCellText, !Is.EqualTo("-"));
        }

        [Test]
        [Order(3)]
        public async Task TwcQ100_10()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));

            var oldPageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom.p-1.pt-2 > div.table-pageInfo")).Text;

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(4) > storm-tooltip > div > button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var deleteButton2 = TestHelper.FindAndMoveElement(_driver, "div.rz-dialog-wrapper > div.rz-dialog > div.rz-dialog-content > div > div > div > button");
            _actions.MoveToElement(deleteButton2).Click().Perform();

            string? newPageInfo = default;
            _wait.Until((_) =>
            {
                newPageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom.p-1.pt-2 > div.table-pageInfo")).Text;
                return !string.Equals(newPageInfo, oldPageInfo);
            });

            That(newPageInfo, !Is.EqualTo(oldPageInfo));
        }

        private async Task _01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增問卷']");
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
        }

        private async Task _02()
        {
            var nameStormInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷名稱']");
            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            nameInput.SendKeys("問卷名稱1120824");

            var headerContentStorInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷頁首說明']");
            var headerContentInput = headerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            headerContentInput.SendKeys("頁首說明1120824");

            var footerContentStorInputGroup = TestHelper.FindAndMoveElement(_driver, "storm-input-group[label='問卷結尾文字']");
            var footerContentInput = footerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            footerContentInput.SendKeys("結尾文字1120824");

            var nextbutton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.button-row.d-flex.mt-4 > button");
            _actions.MoveToElement(nextbutton).Click().Perform();

            var secondTitle = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div:nth-child(1) > h5");

            That(secondTitle.Text, Is.EqualTo("建立題目"));
        }

        private async Task _03()
        {
            var createButton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button");
            _actions.MoveToElement(createButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline=\"新增題目\"]")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
        }

        private async Task _04()
        {
            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"題目\"]")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            contentInput.SendKeys("A1");

            var countElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#create > div.row.mt-3.mb-7 > div > storm-select > div > div.choices__inner > div > div")));
            _actions.MoveToElement(countElement).Click().Perform();
            var option3Value = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value=\"3\"]")));
            _actions.MoveToElement(option3Value).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 1\"]")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionOneInput.SendKeys("滿意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 2\"]")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 3\"]")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionThreeInput.SendKeys("不滿意");

            var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
            _actions.MoveToElement(createButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > h5")));
            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(2) > div > label")));
            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(3) > div > label")));
            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(4) > div > label")));

            That(content.Text, Is.EqualTo("A1"));
            That(optionOne.Text, Is.EqualTo("滿意"));
            That(optionTwo.Text, Is.EqualTo("普通"));
            That(optionThree.Text, Is.EqualTo("不滿意"));
        }

        private async Task _05()
        {
            var createQuestionButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div > div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button")));
            _actions.MoveToElement(createQuestionButton).Click().Perform();

            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"題目\"]")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            contentInput.SendKeys("B1");

            var countElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#create > div.row.mt-3.mb-7 > div > storm-select > div > div.choices__inner > div > div")));
            _actions.MoveToElement(countElement).Click().Perform();
            var option3Value = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value=\"3\"]")));
            _actions.MoveToElement(option3Value).Click().Perform();

            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 1\"]")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionOneInput.SendKeys("滿意");

            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 2\"]")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 3\"]")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionThreeInput.SendKeys("不滿意");

            var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
            _actions.MoveToElement(createButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > h5")));
            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(2) > div > label")));
            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(3) > div > label")));
            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(4) > div > label")));

            That(content.Text, Is.EqualTo("B1"));
            That(optionOne.Text, Is.EqualTo("滿意"));
            That(optionTwo.Text, Is.EqualTo("普通"));
            That(optionThree.Text, Is.EqualTo("不滿意"));
        }

        private async Task _06()
        {
            var nextButton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next");
            _actions.MoveToElement(nextButton).Click().Perform();

            var headerContent = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div.row.mx-4.mb-5 > div > h4");
            var questionOne = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(4) > div > h5");
            var questionTwo = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(5) > div > h5");
            var footerContent = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div.row.mx-4.mt-5 > div > h4");

            That(headerContent.Text, Is.EqualTo("頁首說明1120824"));
            That(questionOne.Text, Is.EqualTo("A1"));
            That(questionTwo.Text, Is.EqualTo("B1"));
            That(footerContent.Text, Is.EqualTo("結尾文字1120824"));
        }

        private async Task _07()
        {
            var nextButton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next");
            _actions.MoveToElement(nextButton).Click().Perform();

            var submitButton = TestHelper.FindAndMoveElement(_driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.button-row.d-flex.mt-0.mt-md-4 > button.btn.bg-gradient-dark.ms-auto.mb-0");
            _actions.MoveToElement(submitButton).Click().Perform();

            var stormCard = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='問卷狀態']");
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }
    }
}