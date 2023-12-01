//using OpenQA.Selenium;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
//using static NUnit.Framework.Assert;

//namespace DomainStorm.Project.TWC.Tests
//{
//    public class TwcA100Tests
//    {
//        private IWebDriver _driver = null!;
//        private WebDriverWait _wait = null!;
//        private Actions _actions = null!;
//        public TwcA100Tests()
//        {
//            TestHelper.CleanDb();
//        }

//        [SetUp] // 在每個測試方法之前執行的方法
//        public void Setup()
//        {
//            _driver = TestHelper.GetNewChromeDriver();
//            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
//            _actions = new Actions(_driver);
//        }

//        [TearDown] // 在每個測試方法之後執行的方法
//        public void TearDown()
//        {
//            _driver.Quit();
//        }
//        [Test]
//        [Order(0)]
//        public async Task TwcA100_01To11()
//        {
//            await TwcA100_01();
//            await TwcA100_02();
//            await TwcA100_03();
//            await TwcA100_04();
//            await TwcA100_05();
//            await TwcA100_06();
//            await TwcA100_07();
//            await TwcA100_08();
//            await TwcA100_09();
//            await TwcA100_10();
//            await TwcA100_11();
//        }
//        public async Task TwcA100_01()
//        {
//            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

//            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
//            That(logout.Text, Is.EqualTo("logout"));
//        }
//        public async Task TwcA100_02()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

//            var stormCard = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='新增問卷']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

//            That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
//        }
//        public async Task TwcA100_03()
//        {
//            var nameStormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='問卷名稱']")));
//            var nameInput = nameStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
//            nameInput.SendKeys("測試名稱");

//            var headerContentStorInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='問卷頁首說明']")));
//            var headerContentInput = headerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
//            headerContentInput.SendKeys("測試頁首說明");

//            var footerContentStorInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='問卷結尾文字']")));
//            var footerContentInput = footerContentStorInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
//            footerContentInput.SendKeys("測試結尾文字");

//            var nextButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next")));
//            _actions.MoveToElement(nextButton).Click().Perform();

//            var secondTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div:nth-child(1) > h5")));

//            That(secondTitle.Text, Is.EqualTo("建立題目"));
//        }
//        public async Task TwcA100_04()
//        {
//            var createButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

//            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
//        }
//        public async Task TwcA100_05()
//        {
//            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
//            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            contentInput.SendKeys("題目1");

//            var 選項數量 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label = '選項數量'] > div.choices")));
//            _actions.MoveToElement(選項數量).Click().Perform();

//            var countOption = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list [data-value = '3']")));
//            _actions.MoveToElement(countOption).Click().Perform();

//            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
//            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionOneInput.SendKeys("同意");

//            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
//            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionTwoInput.SendKeys("普通");

//            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
//            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionThreeInput.SendKeys("不同意");

//            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
//            _actions.MoveToElement(addButton).Click().Perform();

//            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > h5")));
//            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(2) > div > label")));
//            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(3) > div > label")));
//            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div > div > div:nth-child(4) > div > label")));

//            That(content.Text, Is.EqualTo("題目1"));
//            That(optionOne.Text, Is.EqualTo("同意"));
//            That(optionTwo.Text, Is.EqualTo("普通"));
//            That(optionThree.Text, Is.EqualTo("不同意"));
//        }
//        public async Task TwcA100_06()
//        {
//            var createButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(1) > div > div.text-end.ms-auto > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));

//            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
//        }
//        public async Task TwcA100_07()
//        {
//            var contentStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目']")));
//            var contentInput = contentStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            contentInput.SendKeys("題目2");

//            var 選項數量 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label = '選項數量'] > div.choices")));
//            _actions.MoveToElement(選項數量).Click().Perform();

//            var countOption = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list [data-value = '3']")));
//            _actions.MoveToElement(countOption).Click().Perform();

//            var optionOneStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1']")));
//            var optionOneInput = optionOneStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionOneInput.SendKeys("同意");

//            var optionTwoStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2']")));
//            var optionTwoInput = optionTwoStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionTwoInput.SendKeys("普通");

//            var optionThreeStormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3']")));
//            var optionThreeInput = optionThreeStormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            optionThreeInput.SendKeys("不同意");

//            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#createMultipleChoice > div > div > div > div > div > div > button.btn.btn-primary.me-2")));
//            _actions.MoveToElement(addButton).Click().Perform();

//            var nextButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next")));
//            _actions.MoveToElement(nextButton).Perform();

//            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > h5")));
//            var optionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(2) > div > label")));
//            var optionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(3) > div > label")));
//            var optionThree = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.card-body > form > div.multisteps-form__panel.border-radius-xl.bg-white.js-active > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > div > div:nth-child(4) > div > label")));

//            That(content.Text, Is.EqualTo("題目2"));
//            That(optionOne.Text, Is.EqualTo("同意"));
//            That(optionTwo.Text, Is.EqualTo("普通"));
//            That(optionThree.Text, Is.EqualTo("不同意"));
//        }
//        public async Task TwcA100_08()
//        {
//            var nextButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form.multisteps-form__form > div:nth-child(2) > div.multisteps-form__content > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next")));
//            _actions.MoveToElement(nextButton).Click().Perform();

//            var stormCardTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(3) > div.multisteps-form__content > div > h5")));
//            That(stormCardTitle.Text, Is.EqualTo("問卷預覽"));
//        }
//        public async Task TwcA100_09()
//        {
//            var nextButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form.multisteps-form__form > div:nth-child(3) > div.multisteps-form__content > div.row > div > button.btn.bg-gradient-dark.ms-auto.mb-0.js-btn-next")));
//            _actions.MoveToElement(nextButton).Click().Perform();

//            var stormCardTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.multisteps-form__form > div:nth-child(4) > div.multisteps-form__content > div > h5")));
//            That(stormCardTitle.Text, Is.EqualTo("問卷完成"));
//        }
//        public async Task TwcA100_10()
//        {
//            var sendButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form.multisteps-form__form > div:nth-child(4) > div.multisteps-form__content > div > button.btn.bg-gradient-dark.ms-auto.mb-0")));
//            _actions.MoveToElement(sendButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
//        }
//        public async Task TwcA100_11()
//        {
//            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
//            _actions.MoveToElement(logout).Click().Perform();

//            var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
//            That(button.Text, Is.EqualTo("登入"));
//        }

//        [Test]
//        [Order(1)]
//        public async Task TwcA100_12To23()
//        {
//            await TwcA100_12();
//            await TwcA100_13();
//            await TwcA100_14();
//            await TwcA100_15();
//            await TwcA100_16();
//            await TwcA100_17();
//            await TwcA100_18();
//            await TwcA100_19();
//            await TwcA100_20();
//            await TwcA100_21();
//            await TwcA100_22();
//            await TwcA100_23();
//        }
//        public async Task TwcA100_12()
//        {
//            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

//            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
//            That(logout.Text, Is.EqualTo("logout"));
//        }
//        public async Task TwcA100_13()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

//            var createButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='媒體庫'] > div > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("新增檔案"));
//        }
//        public async Task TwcA100_14()
//        {
//            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("body > input.dz-hidden-input")));
//            string 台水簡介3分鐘精華版 = "台水簡介3分鐘精華版.mp4";
//            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", 台水簡介3分鐘精華版);
//            lastHiddenInput.SendKeys(filePath);

//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='名稱']")));

//            That(stormInputGroup.GetAttribute("value"), Is.EqualTo("台水簡介3分鐘精華版.mp4"));
//        }
//        public async Task TwcA100_15()
//        {
//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='內容']")));
//            var contentInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            contentInput.SendKeys("影片上傳測試");

//            var 上傳 = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 > button.btn.bg-gradient-info.m-0.ms-2")));
//            _actions.MoveToElement(上傳).Click().Perform();

//            var span = _wait.Until(_driver =>
//            {
//                var e = _wait.Until(_driver =>
//                {
//                    var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
//                    var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//                    var span = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name'] > storm-table-cell > span"));
//                    return span;
//                });
//                return !string.IsNullOrEmpty(e.Text) ? e : null;
//            });
//            That(span.Text, Is.EqualTo("台水簡介3分鐘精華版.mp4"));
//        }
//        public async Task TwcA100_16()
//        {
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var watchButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(1)"));
//            _actions.MoveToElement(watchButton).Click().Perform();

//            var closeButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-actions > button.swal2-cancel")));

//            That(closeButton.Text, Is.EqualTo("關閉"));
//        }
//        public async Task TwcA100_17()
//        {
//            Thread.Sleep(1000);
//            var closeButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-actions > button.swal2-cancel")));
//            _actions.MoveToElement(closeButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='媒體庫']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div.card-header.pb-3.pt-2 > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("媒體庫"));
//        }
//        public async Task TwcA100_18()
//        {
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button"));
//            _actions.MoveToElement(editButton).Click().Perform();
//            Thread.Sleep(1000);

//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
//            var nameInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            nameInput.Clear();
//            nameInput.SendKeys("台水簡介");

//            That(nameInput.GetAttribute("value"), Is.EqualTo("台水簡介"));
//        }
//        public async Task TwcA100_19()
//        {
//            var updateButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button.bg-gradient-info")));
//            _actions.MoveToElement(updateButton).Click().Perform();

//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(3) > storm-tooltip > div > button"));
//            _actions.MoveToElement(deleteButton).Click().Perform();

//            var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-actions > button.swal2-confirm")));

//            That(checkButton.Text, Is.EqualTo("刪除"));
//        }
//        public async Task TwcA100_20()
//        {
//            Thread.Sleep(1000);
//            var checkButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.swal2-actions > button.swal2-confirm")));
//            _actions.MoveToElement(checkButton).Click().Perform();

//            Thread.Sleep(1000);
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var p = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td > p"));

//            That(p.Text, Is.EqualTo("沒有找到符合的結果"));
//        }
//        public async Task TwcA100_21()
//        {
//            var createButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='媒體庫'] > div > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("新增檔案"));
//        }
//        public async Task TwcA100_22()
//        {
//            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("body > input.dz-hidden-input:nth-of-type(2)")));
//            string 台水簡介3分鐘精華版 = "台水簡介3分鐘精華版.mp4";
//            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", 台水簡介3分鐘精華版);
//            lastHiddenInput.SendKeys(filePath);

//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-input-group[label='名稱']")));

//            That(stormInputGroup.GetAttribute("value"), Is.EqualTo("台水簡介3分鐘精華版.mp4"));
//        }
//        public async Task TwcA100_23()
//        {
//            await TwcA100_15();
//        }

//        [Test]
//        [Order(2)]
//        public async Task TwcA100_24To35()
//        {
//            await TwcA100_24();
//            await TwcA100_25();
//            //await TwcA100_26();
//            await TwcA100_27();
//            //await TwcA100_28();
//            //await TwcA100_29();
//            //await TwcA100_30();
//            //await TwcA100_31();
//            await TwcA100_32();
//            //await TwcA100_33();
//            //await TwcA100_34();
//            await TwcA100_35();
//        }
//        public async Task TwcA100_24()
//        {
//            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("節目單管理"));
//        }
//        public async Task TwcA100_25()
//        {
//            var createButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理'] > div > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增節目單']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("新增節目單"));
//        }
//        public async Task TwcA100_26()
//        {
//            var createButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增節目單'] > div > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
//            var stormTableCellInput = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > div > input"));
//            _actions.MoveToElement(stormTableCellInput).Click().Perform();

//            var addButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.modal-footer > button")));
//            _actions.MoveToElement(addButton).Click().Perform();
//        }
//        public async Task TwcA100_27()
//        {
//            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱']")));
//            var nameInput = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            nameInput.SendKeys("節目單測試");

//            var stormTextEditor = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-text-editor[label='跑馬燈']")));
//            var stormTextEditorInput = stormTextEditor.GetShadowRoot().FindElement(By.CssSelector("div.ql-editor > p"));
//            stormTextEditorInput.SendKeys("跑馬燈測試");
//            Thread.Sleep(1000);

//            var createButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[slot='footer'] > button")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("節目單管理"));
//        }
//        public async Task TwcA100_28()
//        {
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var watchButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(1)"));
//            _actions.MoveToElement(watchButton).Click().Perform();
//        }
//        public async Task TwcA100_29()
//        {
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(2) > storm-tooltip > div > button"));
//            _actions.MoveToElement(editButton).Click().Perform();
//        }
//        public async Task TwcA100_30()
//        {
//            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
//            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td.text-start.align-middle.action > storm-table-cell > storm-table-toolbar > storm-button:nth-child(3) > storm-tooltip > div > button"));
//            _actions.MoveToElement(deleteButton).Click().Perform();
//        }
//        public async Task TwcA100_31()
//        {

//        }
//        public async Task TwcA100_32()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5.mt-2"));

//            That(stormCardTitle.Text, Is.EqualTo("節目單審核"));
//        }
//        public async Task TwcA100_33()
//        {
//            var searchButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[headline='節目單審核'] > div.row > div.d-flex > button")));
//            _actions.MoveToElement(searchButton).Click().Perform();
//        }
//        public async Task TwcA100_34()
//        {

//        }
//        public async Task TwcA100_35()
//        {
//            var logout = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='./logout']")));
//            _actions.MoveToElement(logout).Click().Perform();

//            var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
//            That(button.Text, Is.EqualTo("登入"));
//        }

//        //[Test]
//        //[Order(0)]
//        //public async Task TwcA100_01() 
//        //{ 
//        //    TestHelper.AccessToken = await TestHelper.GetAccessToken();

//        //    That(TestHelper.AccessToken, Is.Not.Empty);
//        //}

//        //[Test]
//        //[Order(1)]
//        //public async Task TwcA100_02() // 呼叫bmEnableApply/confirm
//        //{
//        //    HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A100_bmEnableApply.json"));

//        //    That(statusCode, Is.EqualTo(HttpStatusCode.OK));
//        //}

//        //[Test]
//        //[Order(2)]
//        //public async Task TwcA100_03() // driver_2中看到申請之表單內容
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    //string id = TestHelper.GetLastSegmentFromUrl(driver);

//        //    //((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
//        //    //driver.SwitchTo().Window(driver.WindowHandles[1]);
//        //    //driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{id}");

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    driver.SwitchTo().Frame(0);

//        //    IWebElement stiApplyCaseNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));;
//        //    string 受理編號 = stiApplyCaseNo.Text;
//        //    That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));

//        //    IWebElement stiWaterNo = driver.FindElement(By.CssSelector("[sti-water-no]"));
//        //    string 水號 = stiWaterNo.Text;
//        //    That(水號, Is.EqualTo("41101202191"));

//        //    IWebElement stiApplyDate = driver.FindElement(By.CssSelector("[sti-apply-date]"));
//        //    string 申請日期 = stiApplyDate.Text;
//        //    That(申請日期, Is.EqualTo("2023年03月06日"));
//        //}

//        //[Test]
//        //[Order(3)]
//        //public async Task TwcA100_04() // driver_2中看到身分證字號欄位出現A123456789
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    driver.SwitchTo().Frame(0);

//        //    var stiTrusteeIdNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiTrusteeIdNo);
//        //    var stiTrusteeIdNoInput = stiTrusteeIdNo.FindElement(By.TagName("input"));

//        //    stiTrusteeIdNoInput.SendKeys("A123456789");
//        //    stiTrusteeIdNoInput.SendKeys(Keys.Tab);

//        //    driver.SwitchTo().DefaultContent();

//        //    var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));

//        //    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);
//        //    driver.SwitchTo().Frame(0);

//        //    stiTrusteeIdNo = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-trustee-id-no]")));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", stiTrusteeIdNo);

//        //    string 身分證號碼 = stiTrusteeIdNo.Text;

//        //    That(身分證號碼, Is.EqualTo("A123456789"));
//        //}

//        //[Test]
//        //[Order(4)]
//        //public async Task TwcA100_05() // driver_2看到受理欄位有落章
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);
//        //    driver.SwitchTo().Frame(0);

//        //    //IWebElement 受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
//        //    //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
//        //    //wait.Until(ExpectedConditions.ElementToBeClickable(受理));
//        //    //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);

//        //    IWebElement 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
//        //    //先加入延遲1秒，不然會還沒scroll完就click
//        //    Thread.Sleep(1000);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
//        //    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

//        //    driver.SwitchTo().DefaultContent();

//        //    var 同步狀態 = driver.FindElement(By.CssSelector("p.d-none"));

//        //    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerText;", 同步狀態) as string == "同步完成");

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);
//        //    driver.SwitchTo().Frame(0);

//        //    受理 = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("受理")));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
//        //    //wait.Until(driver =>
//        //    //{
//        //    //    try
//        //    //    {
//        //    //        IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));
//        //    //        return true;
//        //    //    }
//        //    //    catch (StaleElementReferenceException)
//        //    //    {
//        //    //        return false;
//        //    //    }
//        //    //});

//        //    IReadOnlyList<IWebElement> signElement = driver.FindElements(By.CssSelector("[class='sign']"));
//        //    That(signElement, Is.Not.Empty, "未受理");
//        //}

//        //[Test]
//        //[Order(5)]
//        //public async Task TwcA100_06() // driver_2中勾選消費性用水服務契約，driver_1看到■已詳閱
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement secondStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
//        //    IWebElement firstStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node"));
//        //    IWebElement 消費性用水服務契約 = firstStormTreeNode.FindElement(By.CssSelector("a[href='#contract_1']"));

//        //    Actions actions = new(driver);
//        //    actions.MoveToElement(消費性用水服務契約).Click().Perform();

//        //    消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    wait.Until(driver =>
//        //    {
//        //        IWebElement 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
//        //        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
//        //        return 消費性用水服務契約.GetAttribute("checked") == "true";
//        //    });
//        //    消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));

//        //    That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));
//        //}

//        //[Test]
//        //[Order(6)]
//        //public async Task TwcA100_07() // driver_2中勾選公司個人資料保護告知事項，driver_1看到■已詳閱
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement secondStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
//        //    secondStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
//        //    IWebElement 公司個人資料保護告知事項 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#contract_2']"));

//        //    Actions actions = new(driver);
//        //    actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

//        //    公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    wait.Until(driver =>
//        //    {
//        //        IWebElement 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
//        //        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
//        //        return 公司個人資料保護告知事項.GetAttribute("checked") == "true";
//        //    });
//        //    公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));

//        //    That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));
//        //}

//        //[Test]
//        //[Order(7)]
//        //public async Task TwcA100_08() // driver_2中勾選公司營業章程，driver_1看到■已詳閱
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    Actions actions = new(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement secondStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(2)"));
//        //    IWebElement thirdStormTreeNode = secondStormTreeNode.FindElement(By.CssSelector("storm-tree-node:nth-child(3)"));
//        //    IWebElement 公司營業章程 = thirdStormTreeNode.FindElement(By.CssSelector("a[href='#contract_3']"));

//        //    actions.MoveToElement(公司營業章程).Click().Perform();

//        //    公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    wait.Until(driver =>
//        //    {
//        //        IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
//        //        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
//        //        return 公司營業章程.GetAttribute("checked") == "true";
//        //    });
//        //    公司營業章程 = driver.FindElement(By.Id("公司營業章程"));

//        //    That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
//        //}

//        //[Test]
//        //[Order(8)]
//        //public async Task TwcA100_09() // driver_2中表單畫面完整呈現簽名內容，並於driver中看到相容內容
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    driver.SwitchTo().Frame(0);

//        //    var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
//        //    //先加入延遲1秒，不然會還沒scroll完就click
//        //    Thread.Sleep(1000);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
//        //    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

//        //    driver.SwitchTo().DefaultContent();

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    IWebElement 消費性用水服務契約 = driver.FindElement(By.Id("消費性用水服務契約"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 消費性用水服務契約);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 消費性用水服務契約);

//        //    IWebElement 公司個人資料保護告知事項 = driver.FindElement(By.Id("公司個人資料保護告知事項"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司個人資料保護告知事項);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司個人資料保護告知事項);

//        //    IWebElement 公司營業章程 = driver.FindElement(By.Id("公司營業章程"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 公司營業章程);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 公司營業章程);

//        //    IWebElement 簽名 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 簽名);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 簽名);

//        //    string src_driver_2 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    IWebElement imgElement = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container img")));
//        //    wait.Until(driver =>
//        //    {
//        //        try
//        //        {
//        //            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", imgElement);
//        //            string src = imgElement.GetAttribute("src");
//        //            return src != null;
//        //        }
//        //        catch (StaleElementReferenceException)
//        //        {
//        //            return false;
//        //        }
//        //    });

//        //    string src_driver_1 = driver.FindElement(By.CssSelector("div.dropzone-container img")).GetAttribute("src");

//        //    That(src_driver_1, Is.EqualTo(src_driver_2));
//        //}

//        //[Test]
//        //[Order(9)]
//        //public async Task TwcA100_10() // driver_2中看到掃描拍照證件圖像
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    Actions actions = new(driver);

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    IWebElement stormVerticalNavigation = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-vertical-navigation")));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
//        //    IWebElement stormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("storm-tree-node"));
//        //    IWebElement 掃描拍照 = stormTreeNode.FindElement(By.CssSelector("a[href='#credential']"));

//        //    actions.MoveToElement(掃描拍照).Click().Perform();

//        //    IWebElement 啟動掃描證件 = driver.FindElement(By.CssSelector("button.btn.btn-primary.ms-2"));
//        //    actions.MoveToElement(啟動掃描證件).Perform();
//        //    啟動掃描證件.Click();

//        //    IWebElement imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
//        //    string src_driver_1 = imgElement.GetAttribute("src");

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    imgElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropzone-container img")));
//        //    string src_driver_2 = imgElement.GetAttribute("src");

//        //    That(src_driver_2, Is.EqualTo(src_driver_1));
//        //}

//        //[Test]
//        //[Order(10)]
//        //public async Task TwcA100_11() // driver_2中看到夾帶附件資訊
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    string id = TestHelper.OpenNewWindowAndNavigateToUrlWithLastSegment(driver);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

//        //    driver.SwitchTo().Window(driver.WindowHandles[0]);

//        //    IWebElement stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
//        //    IWebElement secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
//        //    IWebElement 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

//        //    Actions actions = new(driver);
//        //    actions.MoveToElement(夾帶附件).Click().Perform();

//        //    IWebElement 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
//        //    actions.MoveToElement(新增文件).Perform();
//        //    新增文件.Click();

//        //    wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

//        //    IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
//        //    IWebElement lastHiddenInput = hiddenInputs[^1];

//        //    string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
//        //    string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

//        //    lastHiddenInput.SendKeys(附件1Path);

//        //    hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

//        //    lastHiddenInput = hiddenInputs[^1];

//        //    string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
//        //    string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

//        //    lastHiddenInput.SendKeys(附件2Path);

//        //    IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
//        //    actions.MoveToElement(上傳).Perform();
//        //    上傳.Click();

//        //    IWebElement stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
//        //    IWebElement stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
//        //    IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

//        //    IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
//        //    IWebElement spanElement = element.FindElement(By.CssSelector("span"));
//        //    wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

//        //    driver.SwitchTo().Window(driver.WindowHandles[1]);

//        //    stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
//        //    stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
//        //    stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
//        //    stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stormTable);

//        //    IWebElement fileName1 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(1) > td[data-field='name']"));
//        //    IWebElement spanName1 = fileName1.FindElement(By.CssSelector("span"));
//        //    string spanText1 = spanName1.Text;

//        //    IWebElement fileName2 = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody > tr:nth-child(2) > td[data-field='name']"));
//        //    IWebElement spanName2 = fileName2.FindElement(By.CssSelector("span"));
//        //    string spanText2 = spanName2.Text;

//        //    That(spanText1, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));
//        //    That(spanText2, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
//        //}

//        //[Test]
//        //[Order(11)]
//        //public async Task TwcA100_12() // 該申請案件進入未結案件中等待後續排程資料於結案後消失
//        //{
//        //    ChromeDriver driver = TestHelper.GetNewChromeDriver();

//        //    await TestHelper.Login(driver, "0511", TestHelper.Password!);
//        //    driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));
//        //    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

//        //    driver.SwitchTo().Frame(0);

//        //    var 受理 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#受理")));

//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
//        //    //先加入延遲1秒，不然會還沒scroll完就click
//        //    Thread.Sleep(1000);
//        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", 受理);
//        //    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理 .sign")));

//        //    driver.SwitchTo().DefaultContent();

//        //    IWebElement stormVerticalNavigation = driver.FindElement(By.CssSelector("storm-vertical-navigation"));
//        //    IWebElement stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
//        //    IWebElement fourthStormTreeNode = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node:nth-child(4)"));
//        //    IWebElement secondStormTreeNode = fourthStormTreeNode.FindElement(By.CssSelector("div storm-tree-node:nth-child(2)"));
//        //    IWebElement 夾帶附件 = secondStormTreeNode.FindElement(By.CssSelector("a[href='#file']"));

//        //    Actions actions = new(driver);
//        //    actions.MoveToElement(夾帶附件).Click().Perform();

//        //    IWebElement 新增文件 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-primary"));
//        //    actions.MoveToElement(新增文件).Perform();
//        //    新增文件.Click();

//        //    wait.Until(_ => driver.FindElements(By.CssSelector("body > .dz-hidden-input")).Count == 3);

//        //    IList<IWebElement> hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));
//        //    IWebElement lastHiddenInput = hiddenInputs[^1];

//        //    string twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
//        //    string 附件1Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);

//        //    lastHiddenInput.SendKeys(附件1Path);

//        //    hiddenInputs = driver.FindElements(By.CssSelector("body > .dz-hidden-input"));

//        //    lastHiddenInput = hiddenInputs[^1];

//        //    string twcweb_01_1_夾帶附件2 = "twcweb_01_1_夾帶附件2.pdf";
//        //    string 附件2Path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件2);

//        //    lastHiddenInput.SendKeys(附件2Path);

//        //    IWebElement 上傳 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
//        //    actions.MoveToElement(上傳).Perform();
//        //    上傳.Click();

//        //    IWebElement stormCard_Seventh = stormVerticalNavigation.FindElements(By.CssSelector("storm-card"))[6];
//        //    IWebElement stormEditTable = stormCard_Seventh.FindElement(By.CssSelector("storm-edit-table"));
//        //    IWebElement stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

//        //    IWebElement element = wait.Until(driver => stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='name']")));
//        //    IWebElement spanElement = element.FindElement(By.CssSelector("span"));
//        //    wait.Until(driver => !string.IsNullOrEmpty(spanElement.Text));

//        //    IWebElement 用印或代送件只需夾帶附件 = driver.FindElement(By.Id("用印或代送件只需夾帶附件"));
//        //    actions.MoveToElement(用印或代送件只需夾帶附件).Perform();
//        //    用印或代送件只需夾帶附件.Click();

//        //    IWebElement 確認受理 = driver.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));
//        //    actions.MoveToElement(確認受理).Perform();
//        //    確認受理.Click();

//        //    string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
//        //    wait.Until(ExpectedConditions.UrlContains(targetUrl));
//        //    TestHelper.ClickRow(driver, TestHelper.ApplyCaseNo!);

//        //    IWebElement stormCard = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body > storm-main-content > main > div.container-fluid.py-4.position-relative > storm-card")));
//        //    IWebElement stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
//        //    stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

//        //    ReadOnlyCollection<IWebElement> applyCaseNoElements = wait.Until(driver => stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']")));
//        //    element = applyCaseNoElements.SingleOrDefault(e => e.Text == TestHelper.ApplyCaseNo)!;
//        //    wait.Until(driver =>
//        //    {
//        //        try
//        //        {
//        //            string 受理編號 = element.Text;
//        //            That(受理編號, Is.EqualTo(TestHelper.ApplyCaseNo));
//        //            return true;
//        //        }
//        //        catch (StaleElementReferenceException)
//        //        {
//        //            return false;
//        //        }
//        //    });
//        //}
//    }
//}