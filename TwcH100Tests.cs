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
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增檔案')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));
                return stormCard != null;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "台水官網圖.png");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("台水官網圖.png"));

            var stormInputGroupDurationInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='播放秒數'] input")));
            stormInputGroupDurationInput.SendKeys("10");

            var uploadButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var element = _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var textElement = stormTable.GetShadowRoot().FindElement(By.CssSelector("span span"));
                return textElement.Text == "台水官網圖.png" ? textElement : null;
            });

            That(element!.Text, Is.EqualTo("台水官網圖.png"));
        }

        [Test]
        [Order(1)]
        public async Task TwcH100_01_M100_02()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增檔案')]")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));
                return stormCard != null;
            });

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
            TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

            _wait.Until(_driver =>
            {
                var input = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return input != null;
            });

            var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='名稱'] input")));
            That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

            var uploadButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='媒體管理']"));
                return stormCard != null;
            });

            var stormEditTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var textElement = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return textElement.Text == "testmedia.mp4";
            });

            var textElement = selectedRow!.FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(textElement!.Text, Is.EqualTo("testmedia.mp4"));
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
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='節目單管理']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
            var playlistManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(playlistManage.Text, Is.EqualTo("節目單管理"));
        }

        public async Task TwcH100_03()
        {
            var addListButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增節目單')]")));
            _actions.MoveToElement(addListButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增節目單']"));
                return stormCard != null;
            });
        }

        public async Task TwcH100_04() 
        {
            var addMediaButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增媒體')]")));
            _actions.MoveToElement(addMediaButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "台水官網圖.png";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='加入']")));
        }

        public async Task TwcH100_05() 
        {
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
            nameInput.SendKeys("新增測試" + Keys.Tab);

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'ql-editor')]")));
            editorInput.SendKeys("新增測試" + Keys.Tab);
            Thread.Sleep(1000);

            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);

            Console.WriteLine($"::group::");
            Console.WriteLine($"---------Current URL: {_driver.Url}---------");
            Console.WriteLine(_driver.PageSource);
            Console.WriteLine("::endgroup::");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='節目單管理']")));

            Console.WriteLine($"::group::");
            Console.WriteLine($"---------Current URL: {_driver.Url}---------");
            Console.WriteLine(_driver.PageSource);
            Console.WriteLine("::endgroup::");

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
                var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
                return name.Text == "新增測試";
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(name.Text, Is.EqualTo("新增測試"));

            var marquee = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='marquee'] span span"));
            That(marquee.Text, Is.EqualTo("<h6>新增測試 </h6>"));

            var ownerName = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='ownerName'] span span"));
            That(ownerName.Text, Is.EqualTo("第四區管理處"));
        }

        [Test]
        [Order(3)]
        public async Task TwcH100_06()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var textCentet = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '新增測試')]")));
            That(textCentet.Text, Is.EqualTo("新增測試"));
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
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var deleteButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(deleteButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '是否確定刪除？')]")));
            That(hint.Text, Is.EqualTo("是否確定刪除？"));
        }

        public async Task TwcH100_08()
        {
            var submitButton = _driver.FindElement(By.XPath("//span[contains(text(), '刪除')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            Thread.Sleep(1000);
            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/playlist"));

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count == 1;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("p"));
            That(result.Text, Is.EqualTo("沒有找到符合的結果"));
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
            var addMediaButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增媒體')]")));
            _actions.MoveToElement(addMediaButton).Click().Perform();

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
            var tbody = stormTable.GetShadowRoot().FindElement(By.CssSelector("tbody"));
            var trList = tbody!.FindElements(By.CssSelector("tr"));
            var selectedRows = trList.FirstOrDefault(tr =>
            {
                var nameCell = tr.FindElement(By.CssSelector("td[data-field='name'] span"));
                return nameCell.Text == "testmedia.mp4";
            });
            _actions.MoveToElement(selectedRows).Click().Perform();

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='加入']")));
        }

        public async Task TwcH100_12()
        {
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
            nameInput.SendKeys("節目單測試" + Keys.Tab);

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'ql-editor')]")));
            editorInput.SendKeys("跑馬燈測試" + Keys.Tab);
            Thread.Sleep(2000);

            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='節目單管理']")));

            _wait.Until(driver =>
            {
                var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
                var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
                return name.Text == "節目單測試";
            });

            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
            var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(name.Text, Is.EqualTo("節目單測試"));

            var marquee = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='marquee'] span span"));
            That(marquee.Text, Is.EqualTo("<h6>跑馬燈測試 </h6>"));

            var ownerName = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='ownerName'] span span"));
            That(ownerName.Text, Is.EqualTo("第四區管理處"));
        }

        [Test]
        [Order(6)]
        public async Task TwcH100_13()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var viewButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(1) button"));
            _actions.MoveToElement(viewButton).Click().Perform();

            var textCentet = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '節目單測試')]")));
            That(textCentet.Text, Is.EqualTo("節目單測試"));
        }
        [Test]
        [Order(7)]
        public async Task TwcH100_14()
        {
            await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist");

            _wait.Until(_ =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var editButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(2) button"));
            _actions.MoveToElement(editButton).Click().Perform();

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='修改節目單']"));
                return stormCard != null;
            });
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
            await TwcH100_07();
        }
        public async Task TwcH100_16()
        {
            var submitButton = _driver.FindElement(By.XPath("//span[contains(text(), '取消')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(_ =>
            {
                _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/playlist"));
                Thread.Sleep(1000);

                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.Count == 1;
            });

            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(result.Text, Is.EqualTo("節目單測試"));
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
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");

            _wait.Until(_ =>
            {
                var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='節目單審核']"));
                return stormCard != null;
            });

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單審核']")));
            var playlistApprove = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(playlistApprove.Text, Is.EqualTo("節目單審核"));
        }
        public async Task TwcH100_18()
        {
            //var searchButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '查詢')]")));
            //_actions.MoveToElement(searchButton).Click().Perform();
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var result = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
            That(result.Text, Is.EqualTo("節目單測試"));
        }
        public async Task TwcH100_19()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
            var approveButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-toolbar-item:nth-of-type(3) button"));
            _actions.MoveToElement(approveButton).Click().Perform();

            var hint = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '請點選審核動作')]")));
            That(hint.Text, Is.EqualTo("請點選審核動作"));

            var submitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), '核准')]")));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(driver =>
            {
                var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
                var status = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='playListStatus']"));
                return status.Text == "核准";
            });
        }
    }
}