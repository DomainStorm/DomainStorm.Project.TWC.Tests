//using OpenQA.Selenium;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
//using static NUnit.Framework.Assert;

//namespace DomainStorm.Project.TWC.Tests
//{
//    public class TwcP100Tests
//    {
//        private IWebDriver _driver = null!;
//        private WebDriverWait _wait = null!;
//        private Actions _actions = null!;
//        public TwcP100Tests()
//        {
//            TestHelper.CleanDb();
//        }

//        [SetUp]
//        public void Setup()
//        {
//            _driver = TestHelper.GetNewChromeDriver();
//            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
//            _actions = new Actions(_driver);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _driver.Quit();
//        }

//        [Test]
//        [Order(0)]
//        public async Task TwcP100_01To11()
//        {
//            await TwcP100_01();
//            await TwcP100_02();
//            await TwcP100_03();
//            await TwcP100_04();
//            await TwcP100_05();
//            await TwcP100_06();
//            await TwcP100_07();
//            await TwcP100_08();
//        }
//        public async Task TwcP100_01()
//        {
//            await TestHelper.Login(_driver, "meizi", TestHelper.Password!);

//            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
//            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
//            That(logout, Is.Not.Null);
//        }
//        public async Task TwcP100_02()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/user/create");

//            var stormCardTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[slot='vertical'] > h5")));
//            That(stormCardTitle.Text, Is.EqualTo("新增用戶"));
//        }
//        public async Task TwcP100_03()
//        {
//            var lastName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='姓']")));
//            var lastNameInput = lastName.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            lastNameInput.SendKeys("王" + Keys.Tab);

//            var firstName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名']")));
//            var firstNameInput = firstName.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            firstNameInput.SendKeys("阿狗" + Keys.Tab);

//            var id = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='編號']")));
//            var idInput = id.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            idInput.SendKeys("000" + Keys.Tab);

//            var phoneNumber = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='電話']")));
//            var phoneNumberInput = phoneNumber.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            phoneNumberInput.SendKeys("1234567" + Keys.Tab);

//            var email = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='信箱']")));
//            var emailInput = email.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            emailInput.SendKeys("woof@gmail.com" + Keys.Tab);

//            var account = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='帳號']")));
//            var accountInput = account.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            accountInput.SendKeys("woof" + Keys.Tab);

//            var password = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='密碼']")));
//            var passwordInput = password.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            passwordInput.SendKeys("password" + Keys.Tab);

//            var createButton = TestHelper.FindAndMoveToElement(_driver, "button[type='submit']");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
//            _actions.MoveToElement(createButton).Click().Perform();
//        }
//        public async Task TwcP100_04()
//        {
//            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/ControlPanel/Post");

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='職位']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("職位"));
//        }
//        public async Task TwcP100_05()
//        {
//            var createButton = TestHelper.FindAndMoveToElement(_driver, "button[type='button']");
//            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='button']")));
//            _actions.MoveToElement(createButton).Click().Perform();

//            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增職位']")));
//            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("div > div > div > h5"));
//            That(stormCardTitle.Text, Is.EqualTo("新增職位"));
//        }
//        public async Task TwcP100_06()
//        {
//            var dropdown = TestHelper.FindAndMoveToElement(_driver, "div.dropdown-select");
//            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.dropdown-select")));
//            _actions.MoveToElement(dropdown).Click().Perform();

//            var stormTreeView = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tree-view")));
//            var departmentIT = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("storm-tree-node > div > storm-tree-node > a"));
//            _actions.MoveToElement(departmentIT).Click().Perform();

//            var position = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='職稱']")));
//            var positionInput = position.GetShadowRoot().FindElement(By.CssSelector("div > input"));
//            positionInput.SendKeys("資訊人員" + Keys.Tab);

//            var role = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='角色'] div.choices")));
//            _actions.MoveToElement(role).Click().Perform();

//            var questionnaireAdministrator = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list [data-value='2918e0d7-fd17-41e1-ac1c-b9f042053b42']")));
//            _actions.MoveToElement(questionnaireAdministrator).Click().Perform();
//            _actions.SendKeys(Keys.Tab).Perform();

//            var saveButton = TestHelper.FindAndMoveToElement(_driver, "storm-button[type='submit']");
//            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-button[type='submit']")));
//            _actions.MoveToElement(saveButton).Click().Perform();

//            var validationMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("li.validation-message")));
//            That(validationMessage.Text, Is.EqualTo("請選擇任職人員"));
//        }
//        public async Task TwcP100_07()
//        {
//            var role = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='任職人員'] div.choices")));
//            _actions.MoveToElement(role).Click().Perform();

//            var roleName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.choices__list [data-value='3ff13141-e1d3-41ad-9b05-f6da2ab56bd2']")));
//            _actions.MoveToElement(roleName).Click().Perform();
//            _actions.SendKeys(Keys.Tab).Perform();

//            var saveButton = TestHelper.FindAndMoveToElement(_driver, "storm-button[type='submit']");
//            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-button[type='submit']")));
//            _actions.MoveToElement(saveButton).Click().Perform();
//        }
//        public async Task TwcP100_08()
//        {
//            var logout = TestHelper.FindAndMoveToElement(_driver, "storm-tooltip > div > a[href='./logout']");
//            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-tooltip > div > a[href='./logout']")));
//            _actions.MoveToElement(logout).Click().Perform();
//        }
//        public async Task TwcP100_09()
//        {
            
//        }
//        public async Task TwcP100_10()
//        {
           
//        }
//        public async Task TwcP100_11()
//        {
            
//        }
//    }
//}