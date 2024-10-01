using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    //public class TwcA100Tests
    //{
    //    private IWebDriver _driver = null!;
    //    private WebDriverWait _wait = null!;
    //    private Actions _actions = null!;
    //    public TwcA100Tests()
    //    {
    //        TestHelper.CleanDb();
    //    }

    //    [SetUp] // 在每個測試方法之前執行的方法
    //    public void Setup()
    //    {
    //        _driver = TestHelper.GetNewChromeDriver();
    //        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    //        _actions = new Actions(_driver);
    //    }

    //    [TearDown] // 在每個測試方法之後執行的方法
    //    public void TearDown()
    //    {
    //        _driver.Quit();
    //    }
    //    [Test]
    //    [Order(0)]
    //    public async Task TwcA100_01()
    //    {
    //        await TwcM100();
    //        await TwcH100();
    //    }
    //    public async Task TwcM100()
    //    {
    //        await TestHelper.Login(_driver, "irenewei", TestHelper.Password!);

    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/multimedia");

    //        _wait.Until(driver =>
    //        {
    //            var element = driver.FindElement(By.XPath("//storm-card[@headline='媒體管理']"));
    //            return element != null;
    //        });
    //        //_wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='媒體管理']")));

    //        var addFile = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='新增檔案']")));
    //        _actions.MoveToElement(addFile).Click().Perform();

    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='新增檔案']")));

    //        await TestHelper.UploadFileAndCheck(_driver, "testmedia.mp4", "input.dz-hidden-input");

    //        await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='媒體管理']"), 10);

    //        var element = TestHelper.WaitStormEditTableWithText(_driver, "td[data-field='name'] span span", "testmedia.mp4");
    //        That(element.Text, Is.EqualTo("testmedia.mp4"));

    //        //var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "testmedia.mp4");
    //        //TestHelper.UploadFile(_driver, file, "input.dz-hidden-input");

    //        //var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] storm-input-group")));
    //        //fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input[@name='Name']")));
    //        //That(fileName.GetAttribute("value"), Is.EqualTo("testmedia.mp4"));

    //        //var upload = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[text()='上傳']")));
    //        //((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", upload);

    //        //_wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='上傳']")));
    //        //That(TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 1 筆，共 1 筆"));
    //    }
    //    public async Task TwcH100()
    //    {
    //        await TestHelper.NavigateAndWaitForElement(_driver, "/playlist", By.CssSelector("storm-card[headline='節目單管理']"), 10);

    //        var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='節目單管理']")));
    //        var playlistManage = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
    //        That(playlistManage.Text, Is.EqualTo("節目單管理"));

    //        var addListButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增節目單']"), 10);

    //        await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='新增節目單']"), 10);

    //        var addMediaButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='新增媒體']"), 10);

    //        await TestHelper.WaitForElement(_driver, By.CssSelector("div.rz-stack storm-table"), 10);

    //        var rzStormTable = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.rz-stack storm-table")));
    //        var rows = rzStormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
    //        var selectedRow = rows.FirstOrDefault(tr =>
    //        {
    //            var element = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
    //            return element.Text == "testmedia.mp4";
    //        });
    //        _actions.MoveToElement(selectedRow).Click().Perform();

    //        var addButton = TestHelper.WaitAndClick(_driver, By.XPath("//span[text()='加入']"), 10);

    //        await TestHelper.WaitElementDisappear(_driver, By.XPath("//span[text()='加入']"));
    //        Thread.Sleep(1000);

    //        var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
    //        nameInput.SendKeys("節目單測試" + Keys.Tab);

    //        var editorInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'ql-editor')]")));
    //        editorInput.SendKeys("跑馬燈測試" + Keys.Tab);

    //        var submitButton = TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='確定']"), 10);
    //        Thread.Sleep(3000);

    //        var programElement = _driver.FindElements(By.CssSelector("storm-card[headline='節目單管理']")).FirstOrDefault();
    //        if (programElement == null)
    //        {
    //            await TestHelper.WaitAndClick(_driver, By.XPath("//button[text()='確定']"), 10);
    //        }

    //        await TestHelper.WaitForElement(_driver, By.CssSelector("storm-card[headline='節目單管理']"), 10);

    //        That(TestHelper.WaitStormTableUpload(_driver, "div.table-bottom > div.table-pageInfo")!.Text, Is.EqualTo("顯示第 1 至 1 筆，共 1 筆"));

    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/approve");
    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='節目單審核']")));

    //        var approveButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field ='__action_8'] storm-toolbar-item:nth-child(3)");
    //        _actions.MoveToElement(approveButton).Click().Perform();

    //        var checkButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='核准']")));
    //        _actions.MoveToElement(checkButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var statusElement = TestHelper.WaitStormTableUpload(driver, "div.table-responsive td[data-field='playListStatus'] span");

    //            if (statusElement == null)
    //            {
    //                return false;
    //            }

    //            return statusElement.Text == "核准";
    //        });
    //    }

    //    [Test]
    //    [Order(1)]
    //    public async Task TwcA100_02To11()
    //    {
    //        await TwcA100_02();
    //        await TwcA100_03();
    //        await TwcA100_04();
    //        await TwcA100_05();
    //        await TwcA100_06();
    //        await TwcA100_07();
    //        await TwcA100_08();
    //        await TwcA100_09();
    //        await TwcA100_10();
    //        await TwcA100_11();
    //    }
    //    public async Task TwcA100_02()
    //    {
    //        await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
    //    }
    //    public async Task TwcA100_03()
    //    {
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/create");

    //        _wait.Until(_ =>
    //        {
    //            var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增問卷']"));
    //            return stormCard != null;
    //        });

    //        var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增問卷']")));
    //        var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
    //        That(stormCardTitle.Text, Is.EqualTo("新增問卷"));
    //    }
    //    public async Task TwcA100_04()
    //    {
    //        var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷名稱'] input")));
    //        nameInput.SendKeys("測試名稱");
    //        That(nameInput.GetAttribute("value"), Is.EqualTo("測試名稱"));

    //        var descriptionInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷頁首說明'] input")));
    //        descriptionInput.SendKeys("測試頁首說明");
    //        That(descriptionInput.GetAttribute("value"), Is.EqualTo("測試頁首說明"));

    //        var textInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='問卷結尾文字'] input")));
    //        textInput.SendKeys("測試問卷結尾文字");
    //        That(textInput.GetAttribute("value"), Is.EqualTo("測試問卷結尾文字"));

    //        var nextPageButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '下一頁')]")));
    //        _actions.MoveToElement(nextPageButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var h5Element = _driver.FindElement(By.XPath("//h5[contains(text(), '建立題目')]"));
    //            return h5Element != null;
    //        });

    //        var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '建立題目')]")));
    //        That(contentTitle.Text, Is.EqualTo("建立題目"));
    //    }
    //    public async Task TwcA100_05()
    //    {
    //        var addQuestionButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增題目')]")));
    //        _actions.MoveToElement(addQuestionButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='新增題目']"));
    //            return stormCard != null;
    //        });

    //        var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增題目']")));
    //        var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
    //        That(stormCardTitle.Text, Is.EqualTo("新增題目"));
    //    }

    //    public async Task TwcA100_06()
    //    {
    //        var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
    //        contentInput.SendKeys("題目1");
    //        That(contentInput.GetAttribute("value"), Is.EqualTo("題目1"));

    //        var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
    //        _actions.MoveToElement(optionSelect).Click().Perform();

    //        var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
    //        _actions.MoveToElement(optionValue).Click().Perform();

    //        var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
    //        optionOneInput.SendKeys("同意");
    //        That(optionOneInput.GetAttribute("value"), Is.EqualTo("同意"));

    //        var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
    //        optionTwoInput.SendKeys("普通");
    //        That(optionTwoInput.GetAttribute("value"), Is.EqualTo("普通"));

    //        var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
    //        optionThreeInput.SendKeys("不同意");
    //        That(optionThreeInput.GetAttribute("value"), Is.EqualTo("不同意"));

    //        var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
    //        _actions.MoveToElement(addButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var content = _driver.FindElement(By.XPath("//h5[contains(text(), '題目1')]"));
    //            return content != null;
    //        });

    //        var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '題目1')]")));
    //        That(content.Text, Is.EqualTo("題目1"));
    //    }

    //    public async Task TwcA100_07()
    //    {
    //        Thread.Sleep(1000);
    //        await TwcA100_05();
    //    }

    //    public async Task TwcA100_08()
    //    {
    //        var contentInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='題目'] input")));
    //        contentInput.SendKeys("題目2");
    //        That(contentInput.GetAttribute("value"), Is.EqualTo("題目2"));

    //        var optionSelect = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-select[label='選項數量'] >div.choices")));
    //        _actions.MoveToElement(optionSelect).Click().Perform();

    //        var optionValue = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[data-value='3']")));
    //        _actions.MoveToElement(optionValue).Click().Perform();

    //        var optionOneInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 1'] input")));
    //        optionOneInput.SendKeys("同意");
    //        That(optionOneInput.GetAttribute("value"), Is.EqualTo("同意"));

    //        var optionTwoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 2'] input")));
    //        optionTwoInput.SendKeys("普通");
    //        That(optionTwoInput.GetAttribute("value"), Is.EqualTo("普通"));

    //        var optionThreeInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-input-group[label='選項 3'] input")));
    //        optionThreeInput.SendKeys("不同意");
    //        That(optionThreeInput.GetAttribute("value"), Is.EqualTo("不同意"));

    //        var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '加入')]")));
    //        _actions.MoveToElement(addButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var content = _driver.FindElement(By.XPath("//h5[contains(text(), '題目2')]"));
    //            return content != null;
    //        });

    //        var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '題目2')]")));
    //        That(content.Text, Is.EqualTo("題目2"));
    //    }
    //    public async Task TwcA100_09()
    //    {
    //        var nextPageButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//button[contains(text(), '下一頁')])[2]")));
    //        _actions.MoveToElement(nextPageButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var content = _driver.FindElement(By.XPath("//h5[contains(text(), '問卷預覽')]"));
    //            return content != null;
    //        });

    //        var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷預覽')]")));
    //        That(content.Text, Is.EqualTo("問卷預覽"));
    //    }
    //    public async Task TwcA100_10()
    //    {
    //        var nextPageButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("(//button[contains(text(), '下一頁')])[3]")));
    //        _actions.MoveToElement(nextPageButton).Perform();

    //        _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//button[contains(text(), '下一頁')])[3]")));
    //        _actions.MoveToElement(nextPageButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var content = _driver.FindElement(By.XPath("//h5[contains(text(), '問卷完成')]"));
    //            return content != null;
    //        });

    //        var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[contains(text(), '問卷完成')]")));
    //        That(content.Text, Is.EqualTo("問卷完成"));
    //    }
    //    public async Task TwcA100_11()
    //    {
    //        var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("(//button[contains(text(), '送出')])")));
    //        _actions.MoveToElement(submitButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var stormCard = _driver.FindElement(By.CssSelector("storm-card[headline='問卷狀態']"));
    //            return stormCard != null;
    //        });

    //        var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='問卷狀態']")));
    //        var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
    //        That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));

    //        _wait.Until(_ =>
    //        {
    //            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/questionnaire"));
    //            Thread.Sleep(1000);

    //            var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
    //            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));

    //            return rows.Count >= 1;
    //        });

    //        var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
    //        var name = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='name'] span span"));
    //        That(name.Text, Is.EqualTo("測試名稱"));
    //    }
    //    [Test]
    //    [Order(2)]
    //    public async Task TwcA100_12To13()
    //    {
    //        await TwcA100_12();
    //        await TwcA100_13();
    //    }
    //    public async Task TwcA100_12()
    //    {
    //        TestHelper.AccessToken = await TestHelper.GetAccessToken();
    //        That(TestHelper.AccessToken, Is.Not.Empty);
    //    }

    //    public async Task TwcA100_13()
    //    {
    //        HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A100_bmEnableApply.json"));
    //        That(statusCode, Is.EqualTo(HttpStatusCode.OK));
    //    }

    //    [Test]
    //    [Order(3)]
    //    public async Task TwcA100_14To30()
    //    {
    //        await TwcA100_14();
    //        await TwcA100_15();
    //        await TwcA100_16();
    //        await TwcA100_17();
    //        await TwcA100_18();
    //        await TwcA100_19();
    //        await TwcA100_20();
    //        await TwcA100_21();
    //        await TwcA100_22();
    //        await TwcA100_23();
    //        await TwcA100_24();
    //        await TwcA100_25();
    //        await TwcA100_26();
    //        await TwcA100_27();
    //        await TwcA100_28();
    //        await TwcA100_29();
    //        await TwcA100_30();
    //    }
    //    public async Task TwcA100_14()
    //    {
    //        await TestHelper.Login(_driver, "4e03", TestHelper.Password!);
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
    //        TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

    //        var uuid = TestHelper.GetLastSegmentFromUrl(_driver);
    //        ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/playlist/now");
    //        Thread.Sleep(1000);

    //        var stormCarousel = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-carousel")));
    //        var videoAutoPlay = stormCarousel.GetShadowRoot().FindElement(By.CssSelector("video[autoplay]"));
    //        That(videoAutoPlay, Is.Not.Null, "影片正在撥放");

    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");
    //    }

    //    public async Task TwcA100_15()
    //    {
    //        _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
    //        _driver.SwitchTo().Frame(0);

    //        var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-apply-case-no]")));
    //        That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
    //    }
    //    public async Task TwcA100_16()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);
    //        _driver.SwitchTo().Frame(0);

    //        var idNoInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-trustee-id-no]/input")));
    //        idNoInput.SendKeys("A123456789" + Keys.Tab);
    //        await Task.Delay(1000);

    //        _wait.Until(driver =>
    //        {
    //            var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));

    //            if (idElement == null)
    //            {
    //                return false;
    //            }

    //            return idElement.GetAttribute("value") == "A123456789";
    //        });

    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
    //        _driver.SwitchTo().Frame(0);

    //        var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']")));
    //        That(idElement.Text, Is.EqualTo("A123456789"));
    //    }
    //    public async Task TwcA100_17()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);
    //        _driver.SwitchTo().Frame(0);

    //        var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
    //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
    //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

    //        _wait.Until(driver =>
    //        {
    //            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));

    //            if (signElement == null)
    //            {
    //                return false;
    //            }

    //            return signElement.Text == "李麗花";
    //        });

    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
    //        _driver.SwitchTo().Frame(0);

    //        var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
    //        That(signElement.Text, Is.EqualTo("李麗花"));
    //    }
    //    public async Task TwcA100_18()
    //    {
    //        TestHelper.ClickElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 1);

    //        TestHelper.HoverOverElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 0);
    //    }
    //    public async Task TwcA100_19()
    //    {
    //        TestHelper.ClickElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 1);

    //        TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 0);
    //    }
    //    public async Task TwcA100_20()
    //    {
    //        TestHelper.ClickElementInWindow(_driver, "//label[@for='公司營業章程']", 1);

    //        TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司營業章程']", 0);
    //    }
    //    public async Task TwcA100_21()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);

    //        var signButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='簽名']")));
    //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", signButton);

    //        _wait.Until(driver =>
    //        {
    //            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));

    //            return signElement != null;
    //        });

    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);

    //        var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));
    //        _actions.MoveToElement(signElement).Perform();
    //        That(signElement, Is.Not.Null);
    //    }
    //    public async Task TwcA100_22()
    //    {
    //        var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='啟動掃描證件']")));
    //        _actions.MoveToElement(scanButton).Click().Perform();

    //        _wait.Until(driver =>
    //        {
    //            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));

    //            return imgElement != null;
    //        });

    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);

    //        var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));
    //        That(imgElement, Is.Not.Null, "尚未上傳完成");
    //    }
    //    public async Task TwcA100_23()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);

    //        var addAttachment = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[text()='新增文件']")));
    //        _actions.MoveToElement(addAttachment).Click().Perform();

    //        var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
    //        TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

    //        var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
    //        TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(3)");

    //        _wait.Until(driver =>
    //        {
    //            var attachmentName = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-input-group[@label='名稱']//input")));
    //            return attachmentName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
    //        });

    //        var upload = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[text()='上傳']")));
    //        _actions.MoveToElement(upload).Click().Perform();

    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='上傳']")));

    //        var fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
    //        _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
    //        That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));

    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);

    //        fileCount = TestHelper.WaitStormEditTableUpload(_driver, "div.table-pageInfo");
    //        _wait.Until(driver => fileCount!.Text == "顯示第 1 至 2 筆，共 2 筆");
    //        That(fileCount!.Text, Is.EqualTo("顯示第 1 至 2 筆，共 2 筆"));
    //    }
    //    public async Task TwcA100_24()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);

    //        var submitButton = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//button[text()='確認受理']")));
    //        _actions.MoveToElement(submitButton).Click().Perform();

    //        var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
    //        _wait.Until(ExpectedConditions.UrlContains(targetUrl));
    //        TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

    //        _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
    //        _driver.SwitchTo().Frame(0);

    //        var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-apply-case-no]")));
    //        That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
    //    }

    //    public async Task TwcA100_25()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire/now");

    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h3[text()='用水設備各種異動服務申請滿意度問卷調查']")));
    //    }
    //    public async Task TwcA100_26()
    //    {
    //        var questionOne = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='題目1']/following-sibling::div//label[text()='同意']")));
    //        _actions.MoveToElement(questionOne).Click().Perform();

    //        var questionTwo = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='題目2']/following-sibling::div//label[text()='同意']")));
    //        _actions.MoveToElement(questionTwo).Click().Perform();
    //    }
    //    public async Task TwcA100_27()
    //    {
    //        var nextPage = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='下一頁']")));
    //        _actions.MoveToElement(nextPage).Click().Perform();

    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='下一頁']")));

    //        var contentTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@slot='1']")));
    //        That(contentTitle.Text, Is.EqualTo("填寫無誤後，提交問卷"));
    //    }
    //    public async Task TwcA100_28()
    //    {
    //        var sumbitButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='送出']")));
    //        _actions.MoveToElement(sumbitButton).Click().Perform();
    //    }
    //    public async Task TwcA100_29()
    //    {
    //        //會自動關閉，不用點確定
    //    }
    //    public async Task TwcA100_30()
    //    {
    //        _driver.SwitchTo().Window(_driver.WindowHandles[0]);

    //        var logout = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//i[text()='logout']")));
    //        _actions.MoveToElement(logout).Click().Perform();

    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='登入']")));
    //    }
    //    [Test]
    //    [Order(4)]
    //    public async Task TwcA100_31To37()
    //    {
    //        await TwcA100_31();
    //        await TwcA100_32();
    //        await TwcA100_33();
    //        await TwcA100_34();
    //        await TwcA100_35();
    //        await TwcA100_36();
    //        await TwcA100_37();
    //    }

    //    public async Task TwcA100_31()
    //    {
    //        await TestHelper.Login(_driver, "meizi", TestHelper.Password!);
    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h6[text()='首頁']")));
    //    }
    //    public async Task TwcA100_32()
    //    {
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");

    //        var stormCard = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='問卷狀態']")));
    //        var stormCardTitle = stormCard.GetShadowRoot().FindElement(By.CssSelector("h5"));
    //        That(stormCardTitle.Text, Is.EqualTo("問卷狀態"));
    //    }
    //    public async Task TwcA100_33()
    //    {
    //        var deleteButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-toolbar-item:nth-child(4)");
    //        _actions.MoveToElement(deleteButton).Click().Perform();

    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='刪除']")));
    //    }
    //    public async Task TwcA100_34()
    //    {
    //        var checkDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='刪除']")));
    //        _actions.MoveToElement(checkDelete).Click().Perform();

    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='刪除']")));

    //        var hintContent = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='已有問卷資料不得刪除']")));
    //        That(hintContent.Text, Is.EqualTo("已有問卷資料不得刪除"));

    //        var checkButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='確定']")));
    //        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", checkButton);
    //    }
    //    public async Task TwcA100_35()
    //    {
    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));

    //        var chartButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-toolbar-item:nth-child(3)");
    //        _actions.MoveToElement(chartButton).Click().Perform();

    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='測試名稱']")));

    //        var stormChart = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-chart")));
    //        That(stormChart, Is.Not.Null);
    //    }
    //    public async Task TwcA100_36()
    //    {
    //        _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/questionnaire");
    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//storm-card[@headline='問卷狀態']")));

    //        var takeDownButton = TestHelper.WaitStormTableUpload(_driver, "td[data-field='__action_6'] storm-toolbar-item:nth-child(2)");
    //        _actions.MoveToElement(takeDownButton).Click().Perform();

    //        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text()='確認']")));
    //    }
    //    public async Task TwcA100_37()
    //    {
    //        var takeDownButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='確認']")));
    //        _actions.MoveToElement(takeDownButton).Click().Perform();

    //        _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//span[text()='確認']")));

    //        var date = TestHelper.WaitStormTableUpload(_driver, "td[data-field='planDisableDate'] span");
    //        That(date!.Text, Is.Not.Empty);
    //    }
    //}
}