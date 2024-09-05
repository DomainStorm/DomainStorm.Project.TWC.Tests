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
        public async Task TwcQ100_01To13()
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
        }

        public async Task TwcQ100_01()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
        }

        public async Task TwcQ100_02()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增問卷']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增問卷']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
        }
        public async Task TwcQ100_03()
        {
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷名稱'] input")));
            nameInput.SendKeys("這是問卷名稱");
            That(nameInput.GetAttribute("value"), Is.EqualTo("這是問卷名稱"));

            var descriptionInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷頁首說明'] input")));
            descriptionInput.SendKeys("這是問卷頁首說明");
            That(descriptionInput.GetAttribute("value"), Is.EqualTo("這是問卷頁首說明"));

            var textInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷結尾文字'] input")));
            textInput.SendKeys("這是問卷結尾文字");
            That(textInput.GetAttribute("value"), Is.EqualTo("這是問卷結尾文字"));


            var nextPageButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '下一頁')]")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            _wait.Until(driver =>
            {
                var h5Element = _driver.FindElement(By.XPath("//h5[contains(text(), '建立題目')]"));
                return h5Element != null;
            });

            //var nextPageButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form > div:nth-child(1) > div > div.button-row > button")));
            //_actions.MoveToElement(nextPageButton).Click().Perform();

            //_wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[slot='0']")));

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '建立題目')]")));
            That(contentTitle.Text, Is.EqualTo("建立題目"));
        }
        public async Task TwcQ100_04()
        {
            //var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("form.multisteps-form__form > div:nth-child(2) > div.multisteps-form__content > div[slot='1'] > div.row > div.col >div.text-end > button")));
            //_actions.MoveToElement(createButton).Click().Perform();

            var addQuestionButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增題目')]")));
            _actions.MoveToElement(addQuestionButton).Click().Perform();

            _wait.Until(driver =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增題目']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
        }
        public async Task TwcQ100_05()
        {
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            contentInput.SendKeys("整體而言，我對本次活動非常滿意");
            That(contentInput.GetAttribute("value"), Is.EqualTo("整體而言，我對本次活動非常滿意"));

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            optionOneInput.SendKeys("非常同意");
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            optionTwoInput.SendKeys("同意");
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            optionThreeInput.SendKeys("普通");
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            optionFourInput.SendKeys("不同意");
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            var validationMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//li[contains(text(), '尚有選項未輸入')]")));
            That(validationMessage.Text, Is.EqualTo("尚有選項未輸入"));
        }
        public async Task TwcQ100_06()
        {
            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            optionFiveInput.SendKeys("非常不同意");
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            _wait.Until(driver =>
            {
                var content = _driver.FindElement(By.XPath("//h5[contains(text(), '整體而言，我對本次活動非常滿意')]"));
                return content != null;
            });

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '整體而言，我對本次活動非常滿意')]")));
            That(content.Text, Is.EqualTo("整體而言，我對本次活動非常滿意"));

            var stronglyAgree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '非常同意']")));
            That(stronglyAgree.Text, Is.EqualTo("非常同意"));

            var agree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '同意']")));
            That(agree.Text, Is.EqualTo("同意"));

            var neutral = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '普通']")));
            That(neutral.Text, Is.EqualTo("普通"));

            var disagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '不同意']")));
            That(disagree.Text, Is.EqualTo("不同意"));

            var stronglyDisagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '非常不同意']")));
            That(stronglyDisagree.Text, Is.EqualTo("非常不同意"));
            //var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(1) > label")));
            //var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(2) > label")));
            //var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(3) > label")));
            //var optionFour = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(4) > label")));
            //var optionFive = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.flex-wrap > div:nth-child(5) > label")));
        }
        public async Task TwcQ100_07()
        {
            await TwcQ100_04();
        }
        public async Task TwcQ100_08()
        {
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            contentInput.SendKeys("本次活動內容對我有幫助");
            That(contentInput.GetAttribute("value"), Is.EqualTo("本次活動內容對我有幫助"));

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            optionOneInput.SendKeys("非常同意");
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            optionTwoInput.SendKeys("同意");
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            optionThreeInput.SendKeys("普通");
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            optionFourInput.SendKeys("不同意");
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            optionFiveInput.SendKeys("非常不同意");
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '本次活動內容對我有幫助')]")));
            That(content.Text, Is.EqualTo("本次活動內容對我有幫助"));

            var stronglyAgree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[contains(text(), '非常同意')]")));
            That(stronglyAgree.Text, Is.EqualTo("非常同意"));

            var agree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '同意']")));
            That(agree.Text, Is.EqualTo("同意"));

            var neutral = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '普通']")));
            That(neutral.Text, Is.EqualTo("普通"));

            var disagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '不同意']")));
            That(disagree.Text, Is.EqualTo("不同意"));

            var stronglyDisagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '非常不同意']")));
            That(stronglyDisagree.Text, Is.EqualTo("非常不同意"));
        }
        public async Task TwcQ100_09()
        {
            await TwcQ100_04();
        }
        public async Task TwcQ100_10()
        {
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            contentInput.SendKeys("本次活動讓我收穫豐富");
            That(contentInput.GetAttribute("value"), Is.EqualTo("本次活動讓我收穫豐富"));

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='5']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            optionOneInput.SendKeys("非常同意");
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            optionTwoInput.SendKeys("同意");
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            optionThreeInput.SendKeys("普通");
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            optionFourInput.SendKeys("不同意");
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            optionFiveInput.SendKeys("非常不同意");
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '本次活動讓我收穫豐富')]")));
            That(content.Text, Is.EqualTo("本次活動讓我收穫豐富"));

            var stronglyAgree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[contains(text(), '非常同意')]")));
            That(stronglyAgree.Text, Is.EqualTo("非常同意"));

            var agree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '同意']")));
            That(agree.Text, Is.EqualTo("同意"));

            var neutral = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '普通']")));
            That(neutral.Text, Is.EqualTo("普通"));

            var disagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '不同意']")));
            That(disagree.Text, Is.EqualTo("不同意"));

            var stronglyDisagree = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[text() = '非常不同意']")));
            That(stronglyDisagree.Text, Is.EqualTo("非常不同意"));
        }
        public async Task TwcQ100_11()
        {
            var nextPageButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//button[contains(text(), '下一頁')])[2]")));
            _actions.MoveToElement(nextPageButton).Click().Perform();
            //var nextPageButton = TestHelper.FindAndMoveElement(_driver, "form > div:nth-child(2) button[title='Next']");
            //_actions.MoveToElement(nextPageButton).Click().Perform();

            _wait.Until(driver =>
            {
                var content = _driver.FindElement(By.XPath("//h5[contains(text(), '問卷預覽')]"));
                return content != null;
            });

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷預覽')]")));
            That(content.Text, Is.EqualTo("問卷預覽"));
        }
        public async Task TwcQ100_12()
        {
            var nextPageButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("(//button[contains(text(), '下一頁')])[3]")));
            _actions.MoveToElement(nextPageButton).Perform();

            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//button[contains(text(), '下一頁')])[3]")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            _wait.Until(driver =>
            {
                var content = _driver.FindElement(By.XPath("//h5[contains(text(), '問卷完成')]"));
                return content != null;
            });

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷完成')]")));
            That(content.Text, Is.EqualTo("問卷完成"));
        }
        public async Task TwcQ100_13()
        {
            var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("(//button[contains(text(), '送出')])")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(driver =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='問卷狀態']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/questionnaire"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count >= 1;
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(name.Text, Is.EqualTo("這是問卷名稱"));
        }

        [Test]
        [Order(1)]
        public async Task TwcQ100_14To18()
        {
            await TwcQ100_14();
            await TwcQ100_15();
            await TwcQ100_16();
            await TwcQ100_17();
            await TwcQ100_18();
        }
        public async Task TwcQ100_14()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            _wait.Until(driver =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='問卷狀態']"));
                return stormCard != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            _actions.MoveToElement(viewButton).Click().Perform();

            _wait.Until(_ =>
            {
                var element = _driver.FindElement(By.CssSelector("h4"));
                return element != null;
            });

            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h4")));
            That(element.Text, Is.EqualTo("這是問卷頁首說明"));

            var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), '確定')]")));
            _actions.MoveToElement(closeButton).Click().Perform();
        }

        public async Task TwcQ100_15()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            _actions.MoveToElement(planDisableDateButton).Click().Perform();

            _wait.Until(_ =>
            {
                var element = _driver.FindElement(By.XPath("//label[text() = '下架日期']"));
                return element != null;
            });

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '確認']")));
            That(submitButton.Text, Is.EqualTo("確認"));
        }
        public async Task TwcQ100_16()
        {
            var cancelButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text() = '取消']")));
            _actions.MoveToElement(cancelButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDate = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='planDisableDate'] span span"));
            That(planDisableDate.Text, Is.EqualTo("-"));
        }
        public async Task TwcQ100_17()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            _actions.MoveToElement(planDisableDateButton).Click().Perform();

            _wait.Until(_ =>
            {
                var element = _driver.FindElement(By.XPath("//label[text() = '下架日期']"));
                return element != null;
            });

            DateTime currentDateTime = DateTime.Now;

            var expiryDateInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期']")));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '確認']")));
            That(submitButton.Text, Is.EqualTo("確認"));
        }
        public async Task TwcQ100_18()
        {   
            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text() = '確認']")));
            _actions.MoveToElement(submitButton).Click().Perform();
            await Task.Delay(1000);

            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string expectedDateTime = todayDate + "T00:00:00";

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDate = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='planDisableDate'] span span"));
            That(planDisableDate.Text, Is.EqualTo(expectedDateTime));
            //That(planDisableDate.Text, Is.Not.EqualTo("-"));
        }

        [Test]
        [Order(2)]
        public async Task TwcQ100_19To20()
        {
            await TwcQ100_19();
            await TwcQ100_20();
        }
        public async Task TwcQ100_19()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

            _wait.Until(_ =>
            {
                var element = _driver.FindElement(By.CssSelector("h3"));
                return element != null;
            });

            var title = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3")));
            That(title.Text, Is.EqualTo("用水設備各種異動服務申請滿意度問卷調查"));
        }
        public async Task TwcQ100_20()
        {
            var card = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body")));
            That(card, Is.Not.Null);
        }

        [Test]
        [Order(3)]
        public async Task TwcQ100_21To24()
        {
            await TwcQ100_21();
            await TwcQ100_22();
            await TwcQ100_23();
            await TwcQ100_24();
        }
        public async Task TwcQ100_21()
        {
            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-sidenav")));
        }
        public async Task TwcQ100_22()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

            _wait.Until(driver =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='問卷狀態']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
        }
        public async Task TwcQ100_23()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(4)"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = _driver.FindElement(By.XPath("//h5[text()='是否確定刪除？']"));
                return element != null;
            });

            var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='刪除']")));
            That(checkButton.Text, Is.EqualTo("刪除"));
        }
        public async Task TwcQ100_24()
        {
            var deleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='刪除']")));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.rz-stack button")));

            _wait.Until(driver => 
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var element = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
                return element.Text == "沒有找到符合的結果";
            });
        }
    }
}