using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcH100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcH100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcA101Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
                _testHelper = new TestHelper(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
        }

        [Test]
        [Order(0)]
        public Task TwcH100_01_M100_01()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/multimedia", By.CssSelector("storm-card[headline='媒體管理']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增檔案']"));

            _testHelper.UploadFilesAndCheck(new[] { "台水官網圖.png"}, "input.dz-hidden-input");

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='媒體管理']"));

            That(_testHelper.WaitShadowElement( "td[data-field='name'] span span", "台水官網圖.png", isEditTable: true), Is.Not.Null);
            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        public Task TwcH100_01_M100_02()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/multimedia", By.CssSelector("storm-card[headline='媒體管理']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增檔案']"));

            _testHelper.UploadFilesAndCheck(new[] { "testmedia.mp4" }, "input.dz-hidden-input");

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='媒體管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "testmedia.mp4", isEditTable: true), Is.Not.Null);
            return Task.CompletedTask;
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
        public Task TwcH100_02()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
            var playlistManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(playlistManage.Text, Is.EqualTo("節目單管理"));
            return Task.CompletedTask;
        }
        public Task TwcH100_03()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增節目單']"));

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='新增節目單']"));
            return Task.CompletedTask;
        }
        public Task TwcH100_04()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增媒體']"));

            _testHelper.WaitElementVisible(By.CssSelector("div.rz-stack storm-table"));

            var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return element.Text == "台水官網圖.png";
            });
            _actions.MoveToElement(selectedRow).Click().Perform();
            _wait.Until(ExpectedConditions.StalenessOf(_testHelper.ElementClick(By.XPath("//span[text()='加入']"))));
            return Task.CompletedTask;
        }
        public Task TwcH100_05()
        {
            _testHelper.InputSendKeys(By.XPath("//storm-input-group[@label='名稱']//input"), "新增測試");
            
            _testHelper.InputSendKeys(By.XPath("//div[@class='ql-editor']"), "新增測試");

            _wait.Until(ExpectedConditions.StalenessOf(_testHelper.ElementClick(By.XPath("//button[text()='確定']"))));

            _wait.Until(ExpectedConditions.UrlToBe($"{TestHelper.BaseUrl}/playlist"));

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "新增測試"), Is.Not.Null);

            That(_testHelper.WaitShadowElement("td[data-field='marquee'] span span", "<h6>新增測試</h6>"), Is.Not.Null);

            That(_testHelper.WaitShadowElement( "td[data-field='ownerName'] span span", "第四區管理處"), Is.Not.Null);
            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        public Task TwcH100_06()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "新增測試"), Is.Not.NaN);

            var viewButton = _testHelper.WaitShadowElement("storm-toolbar-item:nth-of-type(1) button");
            _actions.MoveToElement(viewButton).Click().Perform();

            _testHelper.WaitElementVisible(By.XPath("//h5[text()='新增測試']"));
            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        public async Task TwcH100_07To08()
        {
            await TwcH100_07();
            await TwcH100_08();
        }
        public Task TwcH100_07()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "新增測試"), Is.Not.NaN);


            var deleteButton = _testHelper.WaitShadowElement("storm-toolbar-item:nth-of-type(3) button");
            _actions.MoveToElement(deleteButton).Click().Perform();

            _testHelper.WaitElementVisible(By.XPath("//h5[text()='是否確定刪除？']"));
            return Task.CompletedTask;
        }
        public Task TwcH100_08()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='刪除']"));

            That(_testHelper.WaitShadowElement( "p", "沒有找到符合的結果"), Is.Not.Null);

            return Task.CompletedTask;
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
        public Task TwcH100_11()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='新增媒體']"));

            _testHelper.WaitElementVisible(By.CssSelector("div.rz-stack storm-table"));

            var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return element.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRow).Click().Perform();

            _wait.Until(ExpectedConditions.StalenessOf(_testHelper.ElementClick(By.XPath("//span[text()='加入']"))));
            return Task.CompletedTask;
        }
        public Task TwcH100_12()
        {
            _testHelper.InputSendKeys(By.XPath("//storm-input-group[@label='名稱']//input"), "節目單測試");

            _testHelper.InputSendKeys(By.XPath("//div[@class='ql-editor']"), "跑馬燈測試");

            _testHelper.ElementClick(By.XPath("//button[text()='確定']"));

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "節目單測試"), Is.Not.Null);

            That(_testHelper.WaitShadowElement("td[data-field='marquee'] span span", "<h6>跑馬燈測試</h6>"), Is.Not.Null);

            That(_testHelper.WaitShadowElement("td[data-field='ownerName'] span span", "第四區管理處"), Is.Not.Null);
            return Task.CompletedTask;
        }

        [Test]
        [Order(6)]
        public async Task TwcH100_13()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "節目單測試"), Is.Not.Null);

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            _testHelper.WaitElementVisible(By.XPath("//h5[text()='節目單測試']"));
        }

        [Test]
        [Order(7)]
        public async Task TwcH100_14()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "節目單測試"), Is.Not.Null);

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _testHelper.WaitElementVisible(By.CssSelector("storm-card[headline='修改節目單']"));
        }

        [Test]
        [Order(8)]
        public async Task TwcH100_15To16()
        {
            await TwcH100_15();
            await TwcH100_16();
        }
        public Task TwcH100_15()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "節目單測試"), Is.Not.Null);

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            _testHelper.WaitElementVisible(By.XPath("//h5[text()='是否確定刪除？']"));
            return Task.CompletedTask;
        }
        public Task TwcH100_16()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='取消']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "節目單測試"), Is.Not.Null);
            return Task.CompletedTask;
        }

        [Test]
        [Order(9)]
        public async Task TwcH100_17To19()
        {
            await TwcH100_17();
            await TwcH100_18();
            await TwcH100_19();
        }
        public Task TwcH100_17()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/playlist/approve", By.CssSelector("storm-card[headline='節目單審核']"));

            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='節目單審核']"));
            return Task.CompletedTask;
        }
        public Task TwcH100_18()
        {
            That(_testHelper.WaitShadowElement( "td[data-field='name'] span span", "節目單測試"), Is.Not.Null);
            return Task.CompletedTask;
        }
        public Task TwcH100_19()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var approveButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(approveButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='請點選審核動作']")));
            That(content.Text, Is.EqualTo("請點選審核動作"));

            _testHelper.ElementClick(By.XPath("//span[text()='核准']"));

            That(_testHelper.WaitShadowElement("td[data-field='playListStatus']", "核准"), Is.Not.Null);
            return Task.CompletedTask;
        }
    }
}