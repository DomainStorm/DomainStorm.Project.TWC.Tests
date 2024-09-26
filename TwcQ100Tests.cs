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
        private TestHelper _testHelper = null!;
        public TwcQ100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            _driver = TestHelper.GetNewChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _actions = new Actions(_driver);
            _testHelper = new TestHelper(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public Task TwcQ100_01To13()
        {
            TwcQ100_01();
            TwcQ100_02();
            TwcQ100_03();
            TwcQ100_04();
            TwcQ100_05();
            TwcQ100_06();
            TwcQ100_07();
            TwcQ100_08();
            TwcQ100_09();
            TwcQ100_10();
            TwcQ100_11();
            TwcQ100_12();
            TwcQ100_13();

            return Task.CompletedTask;
        }

        public Task TwcQ100_01()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("storm-sidenav"));

            return Task.CompletedTask;
        }

        public Task TwcQ100_02()
        {
            _testHelper.NavigateWait("/questionnaire/create", By.CssSelector("storm-card[headline='新增問卷']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增問卷']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_03()
        {
            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='問卷名稱'] input"), "這是問卷名稱");
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷名稱'] input")));
            That(nameInput.GetAttribute("value"), Is.EqualTo("這是問卷名稱"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='問卷頁首說明'] input"), "這是問卷頁首說明");
            var descriptionInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷頁首說明'] input")));
            That(descriptionInput.GetAttribute("value"), Is.EqualTo("這是問卷頁首說明"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='問卷結尾文字'] input"), "這是問卷結尾文字");
            var textInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷結尾文字'] input")));
            That(textInput.GetAttribute("value"), Is.EqualTo("這是問卷結尾文字"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '下一頁')]"));
            _testHelper.WaitElementExists(By.XPath("//h5[contains(text(), '建立題目')]"));

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '建立題目')]")));
            That(contentTitle.Text, Is.EqualTo("建立題目"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_04()
        {
            _testHelper.ElementClick(By.XPath("//button[contains(text(), '新增題目')]"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增題目']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增題目"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_05()
        {
            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='題目'] input"), "整體而言，我對本次活動非常滿意");
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            That(contentInput.GetAttribute("value"), Is.EqualTo("整體而言，我對本次活動非常滿意"));

            _testHelper.ElementClick(By.CssSelector("storm-select[label='選項數量'] >div.choices"));
            _testHelper.ElementClick(By.CssSelector("div[data-value='5']"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 1'] input"), "非常同意");
            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 2'] input"), "同意");
            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 3'] input"), "普通");
            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 4'] input"), "不同意");
            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '加入')]"));

            var validationMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//li[contains(text(), '尚有選項未輸入')]")));
            That(validationMessage.Text, Is.EqualTo("尚有選項未輸入"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_06()
        {
            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 5'] input"), "非常不同意");
            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '加入')]"));
            _testHelper.WaitElementExists(By.XPath("//h5[contains(text(), '整體而言，我對本次活動非常滿意')]"));

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

            return Task.CompletedTask;
        }
        public Task TwcQ100_07()
        {
            TwcQ100_04();

            return Task.CompletedTask;
        }
        public Task TwcQ100_08()
        {
            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='題目'] input"), "本次活動內容對我有幫助");
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            That(contentInput.GetAttribute("value"), Is.EqualTo("本次活動內容對我有幫助"));

            _testHelper.ElementClick(By.CssSelector("storm-select[label='選項數量'] >div.choices"));
            _testHelper.ElementClick(By.CssSelector("div[data-value='5']"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 1'] input"), "非常同意");
            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 2'] input"), "同意");
            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 3'] input"), "普通");
            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 4'] input"), "不同意");
            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 5'] input"), "非常不同意");
            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '加入')]"));

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

            return Task.CompletedTask;
        }
        public Task TwcQ100_09()
        {
            TwcQ100_04();

            return Task.CompletedTask;
        }
        public Task TwcQ100_10()
        {
            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='題目'] input"), "本次活動讓我收穫豐富");
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            That(contentInput.GetAttribute("value"), Is.EqualTo("本次活動讓我收穫豐富"));

            _testHelper.ElementClick(By.CssSelector("storm-select[label='選項數量'] >div.choices"));
            _testHelper.ElementClick(By.CssSelector("div[data-value='5']"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 1'] input"), "非常同意");
            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("非常同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 2'] input"), "同意");
            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 3'] input"), "普通");
            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("普通"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 4'] input"), "不同意");
            var optionFourInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 4'] input")));
            That(optionFourInput.GetAttribute("value"), Is.EqualTo("不同意"));

            _testHelper.InputSendkeys(By.CssSelector("storm-input-group[label='選項 5'] input"), "非常不同意");
            var optionFiveInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 5'] input")));
            That(optionFiveInput.GetAttribute("value"), Is.EqualTo("非常不同意"));

            _testHelper.ElementClick(By.XPath("//button[contains(text(), '加入')]"));

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

            return Task.CompletedTask;
        }
        public Task TwcQ100_11()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '下一頁')])[2]"));
            _testHelper.WaitElementExists(By.XPath("//h5[contains(text(), '問卷預覽')]"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷預覽')]")));
            That(content.Text, Is.EqualTo("問卷預覽"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_12()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '下一頁')])[3]"));
            _testHelper.WaitElementExists(By.XPath("//h5[contains(text(), '問卷完成')]"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷完成')]")));
            That(content.Text, Is.EqualTo("問卷完成"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_13()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '送出')])"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='問卷狀態']"));

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

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task TwcQ100_14To18()
        {
            TwcQ100_14();
            TwcQ100_15();
            TwcQ100_16();
            TwcQ100_17();
            TwcQ100_18();

            return Task.CompletedTask;
        }
        public Task TwcQ100_14()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/questionnaire", By.CssSelector("storm-card[headline='問卷狀態']"));

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1)"));
            _actions.MoveToElement(viewButton).Click().Perform();

            _testHelper.WaitElementExists(By.CssSelector("h4"));

            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h4")));
            That(element.Text, Is.EqualTo("這是問卷頁首說明"));

            _testHelper.ElementClick(By.XPath("//span[contains(text(), '確定')]"));

            return Task.CompletedTask;
        }

        public Task TwcQ100_15()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            _actions.MoveToElement(planDisableDateButton).Click().Perform();

            _testHelper.WaitElementExists(By.XPath("//label[text() = '下架日期']"));

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '確認']")));
            That(submitButton.Text, Is.EqualTo("確認"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_16()
        {
            _testHelper.ElementClick(By.XPath("//span[text() = '取消']"));

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDate = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='planDisableDate'] span span"));
            That(planDisableDate.Text, Is.EqualTo("-"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_17()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDateButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2)"));
            _actions.MoveToElement(planDisableDateButton).Click().Perform();

            _testHelper.WaitElementExists(By.XPath("//label[text() = '下架日期']"));

            DateTime currentDateTime = DateTime.Now;

            var expiryDateInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='下架日期']")));

            string formattedExpiryDate = currentDateTime.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{formattedExpiryDate}'; arguments[0].dispatchEvent(new Event('input')); arguments[0].dispatchEvent(new Event('change'));", expiryDateInput);

            var submitButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '確認']")));
            That(submitButton.Text, Is.EqualTo("確認"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_18()
        {
            _testHelper.ElementClick(By.XPath("//span[text() = '確認']"));
            Thread.Sleep(500);

            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string expectedDateTime = todayDate + "T00:00:00";

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var planDisableDate = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='planDisableDate'] span span"));
            That(planDisableDate.Text, Is.EqualTo(expectedDateTime));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcQ100_19To20()
        {
            TwcQ100_19();
            TwcQ100_20();

            return Task.CompletedTask;
        }
        public Task TwcQ100_19()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.NavigateWait("/questionnaire/now", By.CssSelector("h3"));

            var title = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3")));
            That(title.Text, Is.EqualTo("用水設備各種異動服務申請滿意度問卷調查"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_20()
        {
            _testHelper.WaitElementExists(By.CssSelector("div.card-body"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcQ100_21To24()
        {
            TwcQ100_21();
            TwcQ100_22();
            TwcQ100_23();
            TwcQ100_24();

            return Task.CompletedTask;
        }
        public Task TwcQ100_21()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            _testHelper.WaitElementExists(By.CssSelector("storm-sidenav"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_22()
        {
            _testHelper.NavigateWait("/questionnaire", By.CssSelector("storm-card[headline='問卷狀態']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_23()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(4)"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _testHelper.WaitElementExists(By.XPath("//h5[text()='是否確定刪除？']"));

            var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='刪除']")));
            That(checkButton.Text, Is.EqualTo("刪除"));

            return Task.CompletedTask;
        }
        public Task TwcQ100_24()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='刪除']"));

            var content = _testHelper.WaitShadowElement("p", "沒有找到符合的結果");
            That(content.Text, Is.EqualTo("沒有找到符合的結果"));

            return Task.CompletedTask;
        }
    }
}