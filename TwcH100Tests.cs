using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcH100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcH100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
                _actions = new Actions(_driver);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task TwcH100_01_M100_01()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/multimedia", By.CssSelector("storm-card[headline='媒體管理']"), 10);

            await TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增檔案']"), 10);
            
            await TestHelper.UploadFileAndCheck(_driver, "台水官網圖.png", "input.dz-hidden-input");

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='媒體管理']"), 10);

            var element = TestHelper.WaitStormEditTableWithText(_driver, "td[data-field='name'] span span", "台水官網圖.png");
            That(element.Text, Is.EqualTo("台水官網圖.png"));
        }

        [Test]
        [Order(1)]
        public async Task TwcH100_01_M100_02()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/multimedia", By.CssSelector("storm-card[headline='媒體管理']"), 10);

            await TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增檔案']"), 10);

            await TestHelper.UploadFileAndCheck(_driver, "testmedia.mp4", "input.dz-hidden-input");

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='媒體管理']"), 10);

            var element = TestHelper.WaitStormEditTableWithText(_driver, "td[data-field='name'] span span", "testmedia.mp4");
            That(element.Text, Is.EqualTo("testmedia.mp4"));
        }

        [Test]
        [Order(2)]
        public async Task TwcH100_02To05()
        {
            await TwcH100_02();
            await TwcH100_03();
            await TwcH100_04();
            await TwcH100_05();

        }
        public async Task TwcH100_02()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
            var playlistManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(playlistManage.Text, Is.EqualTo("節目單管理"));
        }
        public async Task TwcH100_03()
        {
            var addListButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增節目單']"), 10);

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='新增節目單']"), 10);
        }
        public async Task TwcH100_04() 
        {
            var addMediaButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增媒體']"), 10);

            await TestHelper.WaitForElement(_driver, By.CssSelector("div.rz-stack storm-table"), 10);

            var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return element.Text == "台水官網圖.png";
            });
            _actions.MoveToElement(selectedRow).Click().Perform();

            var addButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='加入']"), 10);

            await TestHelper.WaitElementDisappear(_driver, By.XPath("//span[text()='加入']"));
            Thread.Sleep(1000);
        }
        public async Task TwcH100_05() 
        {
            await TestHelper.EnterText(_driver, By.XPath("//storm-input-group[@label='名稱']//input"), "新增測試");

            await TestHelper.EnterText(_driver, By.XPath("//div[@class='ql-editor']"), "新增測試");

            //var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
            //_actions.MoveToElement(submitButton).Click().Perform();
            // 比對Twc
            var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='確定']"), 10);
            Thread.Sleep(3000);

            var programElement = _driver.FindElements(By.CssSelector("storm-card[headline='節目單管理']")).FirstOrDefault();
            if (programElement == null) 
            {
                await TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='確定']"), 10);
            }

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "新增測試");
            That(name.Text, Is.EqualTo("新增測試"));

            var marquee = TestHelper.WaitStormTableWithText(_driver, "td[data-field='marquee'] span span", "<h6>新增測試</h6>");
            That(marquee.Text, Is.EqualTo("<h6>新增測試</h6>"));

            var ownerName = TestHelper.WaitStormTableWithText(_driver, "td[data-field='ownerName'] span span", "第四區管理處");
            That(ownerName.Text, Is.EqualTo("第四區管理處"));
        }

        [Test]
        [Order(3)]
        public async Task TwcH100_06()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "新增測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            await TestHelper.WaitForElement(_driver, By.XPath("//h5[text()='新增測試']"), 10);

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='新增測試']")));
            That(content.Text, Is.EqualTo("新增測試"));
        }

        [Test]
        [Order(4)]
        public async Task TwcH100_07To08() 
        {
            await TwcH100_07();
            await TwcH100_08();
        }
        public async Task TwcH100_07()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "新增測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            await TestHelper.WaitForElement(_driver, By.XPath("//h5[text()='是否確定刪除？']"), 10);

            var check = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='是否確定刪除？']")));
            That(check.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_08()
        {
            var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='刪除']"), 10);

            var content = TestHelper.WaitStormTableWithText(_driver, "p", "沒有找到符合的結果");
            That(content.Text, Is.EqualTo("沒有找到符合的結果"));
        }

        [Test]
        [Order(5)]
        public async Task TwcH100_09To12()
        {
            await TwcH100_09();
            await TwcH100_10();
            await TwcH100_11();
            await TwcH100_12();
        }
        public async Task TwcH100_09() 
        {
            await TwcH100_02();
        }
        public async Task TwcH100_10()
        {
            await TwcH100_03();
        }
        public async Task TwcH100_11()
        {
            var addMediaButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增媒體']"), 10);

            await TestHelper.WaitForElement(_driver, By.CssSelector("div.rz-stack storm-table"), 10);

            var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return element.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRow).Click().Perform();

            var addButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='加入']"), 10);

            await TestHelper.WaitElementDisappear(_driver, By.XPath("//span[text()='加入']"));
            Thread.Sleep(1000);
        }
        public async Task TwcH100_12()
        {
            await TestHelper.EnterText(_driver, By.XPath("//storm-input-group[@label='名稱']//input"), "節目單測試");

            await TestHelper.EnterText(_driver, By.XPath("//div[@class='ql-editor']"), "跑馬燈測試");

            //var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
            //_actions.MoveToElement(submitButton).Click().Perform();
            var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='確定']"), 10);

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");
            That(name.Text, Is.EqualTo("節目單測試"));

            var marquee = TestHelper.WaitStormTableWithText(_driver, "td[data-field='marquee'] span span", "<h6>跑馬燈測試</h6>");
            That(marquee.Text, Is.EqualTo("<h6>跑馬燈測試</h6>"));

            var ownerName = TestHelper.WaitStormTableWithText(_driver, "td[data-field='ownerName'] span span", "第四區管理處");
            That(ownerName.Text, Is.EqualTo("第四區管理處"));
        }

        [Test]
        [Order(6)]
        public async Task TwcH100_13()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            await TestHelper.WaitForElement(_driver, By.XPath("//h5[text()='節目單測試']"), 10);

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='節目單測試']")));
            That(content.Text, Is.EqualTo("節目單測試"));
        }

        [Test]
        [Order(7)]
        public async Task TwcH100_14()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='修改節目單']"), 10);
        }   

        [Test]
        [Order(8)]
        public async Task TwcH100_15To16()
        {
            await TwcH100_15();
            await TwcH100_16();
        }
        public async Task TwcH100_15()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            await TestHelper.WaitForElement(_driver, By.XPath("//h5[text()='是否確定刪除？']"), 10);

            var check = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='是否確定刪除？']")));
            That(check.Text, Is.EqualTo("是否確定刪除？"));
        }
        public async Task TwcH100_16()
        {
            var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='取消']"), 10);

            var content = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");
            That(content.Text, Is.EqualTo("節目單測試"));
        }

        [Test]
        [Order(9)]
        public async Task TwcH100_17To19()
        {
            await TwcH100_17();
            await TwcH100_18();
            await TwcH100_19();
        }
        public async Task TwcH100_17()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

            await TestHelper.NavigateAndWaitForElement(_driver, "/playlist/approve", By.CssSelector("storm-card[headline='節目單審核']"), 10);

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));
            var playlistApprove = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(playlistApprove.Text, Is.EqualTo("節目單審核"));
        }
        public async Task TwcH100_18()
        {
            var name = TestHelper.WaitStormTableWithText(_driver, "td[data-field='name'] span span", "節目單測試");

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(result.Text, Is.EqualTo("節目單測試"));
        }
        public async Task TwcH100_19()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var approveButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(approveButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='請點選審核動作']")));
            That(content.Text, Is.EqualTo("請點選審核動作"));

            var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='核准']"), 10);

            var status = TestHelper.WaitStormTableWithText(_driver, "td[data-field='playListStatus']", "核准");
        }
    }
}