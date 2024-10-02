using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper = null!;
        public TwcA100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp] // 在每個測試方法之前執行的方法
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

        [TearDown] // 在每個測試方法之後執行的方法
        public void TearDown()
        {
            _driver?.Quit();
        }
        [Test]
        [Order(0)]
        public async Task TwcA100_01()
        {
            await TwcM100();
            await TwcH100();
        }
        public Task TwcM100()
        {
            _testHelper.Login("irenewei", TestHelper.Password!);

            _testHelper.NavigateWait("/multimedia", By.XPath("//storm-card[@headline='媒體管理']"));

            var addFile = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='新增檔案']")));
            _actions.MoveToElement(addFile).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='新增檔案']")));

            _testHelper.UploadFilesAndCheck(new[] { "testmedia.mp4"}, "input.dz-hidden-input");

            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='媒體管理']"));

            _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[text()='新增檔案']")));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "testmedia.mp4", isEditTable: true), Is.Not.Null);
            return Task.CompletedTask;
        }
        public async Task TwcH100()
        {
            _testHelper.NavigateWait("/playlist", By.CssSelector("storm-card[headline='節目單管理']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增節目單']"));

            _testHelper.NavigateWait("/playlist/create", By.CssSelector("storm-card[headline='新增節目單']"));

            _testHelper.ElementClick(By.XPath("//button[text()='新增媒體']"));

            var selectedRow= _wait.Until(_ =>
            {
                var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
                var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

                return rows.First(tr =>
                {
                    var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                    return element.Text == "testmedia.mp4";
                });
            });

            var input = selectedRow.FindElement(By.XPath("..")).FindElement(By.CssSelector("input"));
            _actions.MoveToElement(input).Click().Perform();
            _wait.Until(ExpectedConditions.ElementToBeSelected(input));

            var elementClick = _testHelper.ElementClick(By.XPath("//span[text()='加入']"));
            _wait.Until(ExpectedConditions.StalenessOf(elementClick));

            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
            nameInput.SendKeys("節目單測試" + Keys.Tab);

            var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'ql-editor')]")));
            editorInput.SendKeys("跑馬燈測試" + Keys.Tab);

            Thread.Sleep(1000);

            _testHelper.ElementClick(By.XPath("//button[@form='create']"));

            _wait.Until(ExpectedConditions.UrlToBe($"{TestHelper.BaseUrl}/playlist"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='節目單管理']"));

            That(_testHelper.WaitShadowElement( "div.table-bottom > div.table-pageInfo", "顯示第 1 至 1 筆，共 1 筆", false), Is.Not.Null);

            _testHelper.NavigateWait("/playlist/approve", By.XPath("//storm-card[@headline='節目單審核']"));

            var approveButton = _testHelper.WaitShadowElement( "td[data-field ='__action_8'] storm-toolbar-item:nth-child(3)");
            _actions.MoveToElement(approveButton).Click().Perform();

            var checkButton =_testHelper.ElementClick(By.XPath("//span[text()='核准']"));

            That(_testHelper.WaitShadowElement("div.table-responsive td[data-field='playListStatus'] span", "核准", false), Is.Not.Null);
        }

        [Test]
        [Order(1)]
        public async Task TwcA100_02To11()
        {
            await TwcA100_02();
            await TwcA100_03();
            await TwcA100_04();
            await TwcA100_05();
            await TwcA100_06();
            await TwcA100_07();
            await TwcA100_08();
            await TwcA100_09();
            await TwcA100_10();
            await TwcA100_11();
        }
        public Task TwcA100_02()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            return Task.CompletedTask;
        }
        public Task TwcA100_03()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");
            _testHelper.NavigateWait("/questionnaire/create", By.CssSelector("storm-card[headline='新增問卷']"));
            return Task.CompletedTask;
        }
        public Task TwcA100_04()
        {
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷名稱'] input")));
            nameInput.SendKeys("測試名稱");
            That(nameInput.GetAttribute("value"), Is.EqualTo("測試名稱"));

            var descriptionInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷頁首說明'] input")));
            descriptionInput.SendKeys("測試頁首說明");
            That(descriptionInput.GetAttribute("value"), Is.EqualTo("測試頁首說明"));

            var textInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷結尾文字'] input")));
            textInput.SendKeys("測試問卷結尾文字");
            That(textInput.GetAttribute("value"), Is.EqualTo("測試問卷結尾文字"));

            var nextPageButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '下一頁')]")));
            _actions.MoveToElement(nextPageButton).Click().Perform();

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '建立題目')]")));
            That(contentTitle.Text, Is.EqualTo("建立題目"));
            return Task.CompletedTask;
        }
        public Task TwcA100_05()
        {
            var addQuestionButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增題目')]")));
            _actions.MoveToElement(addQuestionButton).Click().Perform();

            var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
            var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
            That(stormCardTitle.Text, Is.EqualTo("新增題目"));
            return Task.CompletedTask;
        }

        public async Task TwcA100_06()
        {
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            contentInput.SendKeys("題目1");
            That(contentInput.GetAttribute("value"), Is.EqualTo("題目1"));

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            optionOneInput.SendKeys("同意");
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("同意"));

            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            optionTwoInput.SendKeys("普通");
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("普通"));

            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            optionThreeInput.SendKeys("不同意");
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '題目1')]")));
            That(content.Text, Is.EqualTo("題目1"));
        }

        public async Task TwcA100_07()
        {
            Thread.Sleep(1000);
            await TwcA100_05();
        }

        public Task TwcA100_08()
        {
            var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
            contentInput.SendKeys("題目2");
            That(contentInput.GetAttribute("value"), Is.EqualTo("題目2"));

            var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
            _actions.MoveToElement(optionSelect).Click().Perform();

            var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
            _actions.MoveToElement(optionValue).Click().Perform();

            var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
            optionOneInput.SendKeys("同意");
            That(optionOneInput.GetAttribute("value"), Is.EqualTo("同意"));

            var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
            optionTwoInput.SendKeys("普通");
            That(optionTwoInput.GetAttribute("value"), Is.EqualTo("普通"));

            var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
            optionThreeInput.SendKeys("不同意");
            That(optionThreeInput.GetAttribute("value"), Is.EqualTo("不同意"));

            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
            _actions.MoveToElement(addButton).Click().Perform();

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '題目2')]")));
            That(content.Text, Is.EqualTo("題目2"));
            return Task.CompletedTask;
        }
        public Task TwcA100_09()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '下一頁')])[2]"));
            _testHelper.WaitElementVisible(By.XPath("//h5[contains(text(), '問卷預覽')]"));

            return Task.CompletedTask;
        }
        public Task TwcA100_10()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '下一頁')])[3]"));
            _testHelper.WaitElementVisible(By.XPath("//h5[contains(text(), '問卷完成')]"));

            return Task.CompletedTask;
        }
        public Task TwcA100_11()
        {
            _testHelper.ElementClick(By.XPath("(//button[contains(text(), '送出')])"));
            _wait.Until(ExpectedConditions.UrlToBe($"{TestHelper.BaseUrl}/questionnaire"));
            _testHelper.WaitElementVisible(By.XPath("//storm-card[@headline='問卷狀態']"));

            That(_testHelper.WaitShadowElement("td[data-field='name'] span span", "測試名稱"), Is.Not.Null);

            return Task.CompletedTask;
        }
        [Test]
        [Order(2)]
        public async Task TwcA100_12To13()
        {
            await TwcA100_12();
            await TwcA100_13();
        }
        public async Task TwcA100_12()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }

        public async Task TwcA100_13()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A100_bmEnableApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(3)]
        public async Task TwcA100_14To30()
        {
            await TwcA100_14();
            await TwcA100_15();
            await TwcA100_16();
            await TwcA100_17();
            await TwcA100_18();
            await TwcA100_19();
            await TwcA100_20();
            await TwcA100_21();
            await TwcA100_22();
            await TwcA100_23();
            await TwcA100_24();
            await TwcA100_25();
            await TwcA100_26();
            await TwcA100_27();
            await TwcA100_28();
            await TwcA100_29();
            await TwcA100_30();
        }
        public Task TwcA100_14()
        {
            _testHelper.Login( "4e03", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            string[] segments = _driver.Url.Split('/');
            string uuid = segments[^1];

            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.NavigateWait("/playlist/now", By.CssSelector("storm-carousel"));
            var stormCarousel = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-carousel")));
            var videoAutoPlay = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video[autoplay]"));
            That(videoAutoPlay, Is.Not.Null, "影片正在撥放");


            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");
            return Task.CompletedTask;
        }

        public async Task TwcA100_15()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public Task TwcA100_16()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            _testHelper.InputSendKeys(By.XPath("//span[@sti-trustee-id-no]/input"), "A123456789" + Keys.Tab);
            //var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));
            //That(idElement.GetAttribute("value"), Is.EqualTo("A123456789"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _testHelper.WaitElementExists(By.XPath("//span[@id='身分證號碼']"));
            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼'][text()='A123456789']"))), Is.Not.Null);

            return Task.CompletedTask;
        }

        public Task TwcA100_17()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-post-user-full-name][text()='李麗花']"))), Is.Not.Null);

            return Task.CompletedTask;
        }

        public Task TwcA100_18()
        {
            _driver.SwitchTo().DefaultContent();
            _testHelper.SwitchWindowAndClick("//input[@id='消費性用水服務契約']");
            _wait.Until(ExpectedConditions.ElementToBeSelected(By.XPath("//input[@id='消費性用水服務契約']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_19()
        {
            _testHelper.SwitchWindowAndClick("//input[@id='公司個人資料保護告知事項']");
            _wait.Until(ExpectedConditions.ElementToBeSelected(By.XPath("//input[@id='公司個人資料保護告知事項']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_20()
        {
            _testHelper.SwitchWindowAndClick("//input[@id='公司營業章程']");
            _wait.Until(ExpectedConditions.ElementToBeSelected(By.XPath("//input[@id='公司營業章程']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_21()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='簽名']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().DefaultContent();

            _testHelper.WaitElementExists(By.XPath("//img[@alt='簽名_001.tiff']"));

            return Task.CompletedTask;
        }
        public Task TwcA100_22()
        {
            _testHelper.ElementClick(By.XPath("//span[text()='啟動掃描證件']"));
            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _testHelper.WaitElementExists(By.XPath("//img[@alt='證件_005.tiff']"));

            return Task.CompletedTask;
        }

        public Task TwcA100_23()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));
            _testHelper.WaitElementExists(By.CssSelector("storm-card[headline='新增檔案']"));
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf", "twcweb_01_1_夾帶附件2.pdf" }, "input.dz-hidden-input:nth-of-type(3)");
            _testHelper.WaitElementExists(By.CssSelector("storm-edit-table"));

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

            _wait.Until(driver =>
            {
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 2;
            });

            return Task.CompletedTask;
        }
        public Task TwcA100_24()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));
            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            //var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            //That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));

            return Task.CompletedTask;
        }

        public Task TwcA100_25()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h3[text()='用水設備各種異動服務申請滿意度問卷調查']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_26()
        {
            var questionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='題目1']/following-sibling::div//label[text()='同意']")));
            _actions.MoveToElement(questionOne).Click().Perform();

            var questionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='題目2']/following-sibling::div//label[text()='同意']")));
            _actions.MoveToElement(questionTwo).Click().Perform();
            return Task.CompletedTask;
        }
        public Task TwcA100_27()
        {
            var nextPage = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='下一頁']")));
            _actions.MoveToElement(nextPage).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='下一頁']")));

            var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@slot='1']")));
            That(contentTitle.Text, Is.EqualTo("填寫無誤後，提交問卷"));
            return Task.CompletedTask;
        }
        public Task TwcA100_28()
        {
            var button = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='送出']")));
            _actions.MoveToElement(button).Click().Perform();
            return Task.CompletedTask;
        }
        public Task TwcA100_29()
        {
            return Task.CompletedTask;
            //會自動關閉，不用點確定
        }
        public Task TwcA100_30()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var logout = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//i[text()='logout']")));
            _actions.MoveToElement(logout).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='登入']")));
            return Task.CompletedTask;
        }
        [Test]
        [Order(4)]
        public async Task TwcA100_31To37()
        {
            await TwcA100_31();
            await TwcA100_32();
            await TwcA100_33();
            await TwcA100_34();
            await TwcA100_35();
            await TwcA100_36();
            await TwcA100_37();
        }

        public Task TwcA100_31()
        {
            _testHelper.Login("meizi", TestHelper.Password!);
            return Task.CompletedTask;
        }
        public Task TwcA100_32()
        {
            _testHelper.NavigateWait("/questionnaire", By.XPath("//storm-card[@headline='問卷狀態']"));
            return Task.CompletedTask;
        }
        public Task TwcA100_33()
        {
            var deleteButton = _testHelper.WaitShadowElement( "td[data-field='__action_6'] storm-toolbar-item:nth-child(4)");
            _actions.MoveToElement(deleteButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='刪除']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_34()
        {
            var checkDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='刪除']")));
            _actions.MoveToElement(checkDelete).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='刪除']")));

            var hintContent = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='已有問卷資料不得刪除']")));
            That(hintContent.Text, Is.EqualTo("已有問卷資料不得刪除"));

            var checkButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", checkButton);
            return Task.CompletedTask;
        }
        public Task TwcA100_35()
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));

            var chartButton = _testHelper.WaitShadowElement("td[data-field='__action_6'] storm-toolbar-item:nth-child(3)");
            _actions.MoveToElement(chartButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='測試名稱']")));

            var stormChart = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-chart")));
            That(stormChart, Is.Not.Null);
            return Task.CompletedTask;
        }
        public Task TwcA100_36()
        {
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='問卷狀態']")));

            var takeDownButton = _testHelper.WaitShadowElement("td[data-field='__action_6'] storm-toolbar-item:nth-child(2)");
            _actions.MoveToElement(takeDownButton).Click().Perform();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='確認']")));
            return Task.CompletedTask;
        }
        public Task TwcA100_37()
        {
            var takeDownButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='確認']")));
            _actions.MoveToElement(takeDownButton).Click().Perform();

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='確認']")));

            var date = _testHelper.WaitShadowElement("td[data-field='planDisableDate'] span");
            That(date!.Text, Is.Not.Empty);
            return Task.CompletedTask;
        }
    }
}