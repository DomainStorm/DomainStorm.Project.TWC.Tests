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
        private List<ChromeDriver> _chromeDriverList;
        public TwcQ100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
        public Task Setup()
        {
            _chromeDriverList = new List<ChromeDriver>();

            return Task.CompletedTask;
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
        public async Task TwcQ100_01To07()
        {
            var driver = TestHelper.GetNewChromeDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var actions = new Actions(driver);

            await _01(driver, wait, actions);
            await _02(driver, wait, actions);
            await _03(driver, wait, actions);
            await _04(driver, wait, actions);
            await _05(driver, wait, actions);
            await _06(driver, wait, actions);
            await _07(driver, wait, actions);
        }

        [Test]
        [Order(1)]
        public async Task TwcQ100_08()
        {
            var driver = TestHelper.GetNewChromeDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var actions = new Actions(driver);

            await TestHelper.Login(driver, "meizi", TestHelper.Password!);

            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormTable = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(1) > storm-tooltip > div > button"));
            actions.MoveToElement(viewButton).Click().Perform();

            var dialogContent = TestHelper.FindAndMoveElement(driver, "div.rz-dialog-wrapper > div.rz-dialog > div > div > div.row.mx-4.mb-5 > div > h4");

            That(dialogContent.Text, Is.EqualTo("頁首說明1120824"));
        }

        [Test]
        [Order(2)]
        public async Task TwcQ100_09()
        {
            var driver = TestHelper.GetNewChromeDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var actions = new Actions(driver);

            await TestHelper.Login(driver, "meizi", TestHelper.Password!);

            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormTable = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));

            var updateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button"));
            actions.MoveToElement(updateButton).Click().Perform();

            var updateButton2 = TestHelper.FindAndMoveElement(driver, "div.rz-dialog-wrapper > div.rz-dialog > div.rz-dialog-content > div > div > div > button");
            actions.MoveToElement(updateButton2).Click().Perform();

            string? planDisableDateCellText = default;
            wait.Until((_) =>
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
            var driver = TestHelper.GetNewChromeDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var actions = new Actions(driver);

            await TestHelper.Login(driver, "meizi", TestHelper.Password!);

            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            var stormTable = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));

            var oldPageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom.p-1.pt-2 > div.table-pageInfo")).Text;

            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-toolbar > storm-button:nth-child(4) > storm-tooltip > div > button"));
            actions.MoveToElement(deleteButton).Click().Perform();

            var deleteButton2 = TestHelper.FindAndMoveElement(driver, "div.rz-dialog-wrapper > div.rz-dialog > div.rz-dialog-content > div > div > div > button");
            actions.MoveToElement(deleteButton2).Click().Perform();

            string? newPageInfo = default;
            wait.Until((_) =>
            {
                newPageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom.p-1.pt-2 > div.table-pageInfo")).Text;
                return !string.Equals(newPageInfo, oldPageInfo);
            });

            That(newPageInfo, !Is.EqualTo(oldPageInfo));
        }

        private async Task _01(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            await TestHelper.Login(driver, "meizi", TestHelper.Password!);

            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            var stormCard = TestHelper.FindAndMoveElement(driver, "storm-card[headline='新增問卷']");
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
        }

        private async Task _02(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var nameStormInputGroup = TestHelper.FindAndMoveElement(driver, "storm-input-group[label='問卷名稱']");
            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            nameInput.SendKeys("問卷名稱1120824");

            var headerContentStorInputGroup = TestHelper.FindAndMoveElement(driver, "storm-input-group[label='問卷頁首說明']");
            var headerContentInput = headerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            headerContentInput.SendKeys("頁首說明1120824");

            var footerContentStorInputGroup = TestHelper.FindAndMoveElement(driver, "storm-input-group[label='問卷結尾文字']");
            var footerContentInput = footerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            footerContentInput.SendKeys("結尾文字1120824");

            var nextbutton = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.button-row.d-flex.mt-4 > button");
            actions.MoveToElement(nextbutton).Click().Perform();

            var secondTitle = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div:nth-child(1) > h5");

            That(secondTitle.Text, Is.EqualTo("建立題目"));
        }

        private async Task _03(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var createButton = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button");
            actions.MoveToElement(createButton).Click().Perform();

            var stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline=\"新增題目\"]")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
        }

        private async Task _04(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var contentStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"題目\"]")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            contentInput.SendKeys("A1");

            var countElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#create > div.row.mt-3.mb-7 > div > storm-select > div > div.choices__inner > div > div")));
            actions.MoveToElement(countElement).Click().Perform();
            var option3Value = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value=\"3\"]")));
            actions.MoveToElement(option3Value).Click().Perform();

            var optionOneStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 1\"]")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionOneInput.SendKeys("滿意");

            var optionTwoStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 2\"]")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 3\"]")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionThreeInput.SendKeys("不滿意");

            var createButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
            actions.MoveToElement(createButton).Click().Perform();

            var content = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > h5")));
            var optionOne = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(2) > div > label")));
            var optionTwo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(3) > div > label")));
            var optionThree = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(4) > div > label")));

            That(content.Text, Is.EqualTo("A1"));
            That(optionOne.Text, Is.EqualTo("滿意"));
            That(optionTwo.Text, Is.EqualTo("普通"));
            That(optionThree.Text, Is.EqualTo("不滿意"));
        }

        private async Task _05(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var createQuestionButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div > div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button")));
            actions.MoveToElement(createQuestionButton).Click().Perform();

            var contentStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"題目\"]")));
            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            contentInput.SendKeys("B1");

            var countElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#create > div.row.mt-3.mb-7 > div > storm-select > div > div.choices__inner > div > div")));
            actions.MoveToElement(countElement).Click().Perform();
            var option3Value = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value=\"3\"]")));
            actions.MoveToElement(option3Value).Click().Perform();

            var optionOneStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 1\"]")));
            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionOneInput.SendKeys("滿意");

            var optionTwoStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 2\"]")));
            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionTwoInput.SendKeys("普通");

            var optionThreeStormInputGroup = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label=\"選項 3\"]")));
            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
            optionThreeInput.SendKeys("不滿意");

            var createButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
            actions.MoveToElement(createButton).Click().Perform();

            var content = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > h5")));
            var optionOne = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(2) > div > label")));
            var optionTwo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(3) > div > label")));
            var optionThree = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(4) > div > label")));

            That(content.Text, Is.EqualTo("B1"));
            That(optionOne.Text, Is.EqualTo("滿意"));
            That(optionTwo.Text, Is.EqualTo("普通"));
            That(optionThree.Text, Is.EqualTo("不滿意"));
        }

        private async Task _06(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var nextButton = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next");
            actions.MoveToElement(nextButton).Click().Perform();

            var headerContent = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div.row.mx-4.mb-5 > div > h4");
            var questionOne = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(4) > div > h5");
            var questionTwo = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(5) > div > h5");
            var footerContent = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div.row.mx-4.mt-5 > div > h4");

            That(headerContent.Text, Is.EqualTo("頁首說明1120824"));
            That(questionOne.Text, Is.EqualTo("A1"));
            That(questionTwo.Text, Is.EqualTo("B1"));
            That(footerContent.Text, Is.EqualTo("結尾文字1120824"));
        }

        private async Task _07(ChromeDriver driver, WebDriverWait wait, Actions actions)
        {
            var nextButton = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next");
            actions.MoveToElement(nextButton).Click().Perform();

            var submitButton = TestHelper.FindAndMoveElement(driver, "div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div.button-row.d-flex.mt-0.mt-md-4 > button.btn.bg-gradient-dark.ms-auto.mb-0");
            actions.MoveToElement(submitButton).Click().Perform();

            var stormCard = TestHelper.FindAndMoveElement(driver, "storm-card[headline='問卷狀態']");
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }
    }
}