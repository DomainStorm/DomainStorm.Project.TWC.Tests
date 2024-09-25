using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcE100Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcE100Tests()
        {
            TestHelper.CleanDb();
        }

        [SetUp]
        public void Setup()
        {
            var testMethod = TestContext.CurrentContext.Test.MethodName;
            var methodInfo = typeof(TwcE100Tests).GetMethod(testMethod!);
            var noBrowser = methodInfo?.GetCustomAttribute<NoBrowserAttribute>() != null;

            if (!noBrowser)
            {
                _driver = TestHelper.GetNewChromeDriver();
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                _actions = new Actions(_driver);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }

        [Test]
        [Order(0)]
        [NoBrowser]
        public async Task TwcE100_01To02()
        {
            await TwcE100_01();
            await TwcE100_02();
        }
        public async Task TwcE100_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcE100_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmTransferApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-E100_bmTransferApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(1)]
        public async Task TwcE100_03To16()
        {
            await TwcE100_03();
            await TwcE100_04();
            await TwcE100_05();
            await TwcE100_06();
            await TwcE100_07();
            await TwcE100_08();
            await TwcE100_09();
            await TwcE100_10();
            await TwcE100_11();
            await TwcE100_12();
            await TwcE100_13();
            await TwcE100_14();
            await TwcE100_15();
            await TwcE100_16();
        }
        public async Task TwcE100_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            await TestHelper.NavigateAndWait(_driver, "/draft");
            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var uuid = TestHelper.GetLastSegmentFromUrl(_driver);
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft/second-screen/{uuid}");

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcE100_04()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var idNoInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-trustee-id-no]/input")));
            idNoInput.SendKeys("A123456789" + Keys.Tab);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var idElement = driver.FindElement(By.XPath("//span[@id='身分證號碼']/input"));
                return idElement.GetAttribute("value") == "A123456789";
            });

            var idElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@id='身分證號碼']/input")));
            That(idElement.GetAttribute("value"), Is.EqualTo("A123456789"));

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var idElement = driver.FindElement(By.XPath("//span[@id='身分證號碼']"));
                return idElement != null;
            });

            idElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@id='身分證號碼']")));
            That(idElement.Text, Is.EqualTo("A123456789"));
        }
        public async Task TwcE100_05()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiNoteInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]/input")));
            stiNoteInput.SendKeys("備註內容" + Keys.Tab);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]/input")));
                return stiNote.GetAttribute("value") == "備註內容";
            });

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            _wait.Until(driver =>
            {
                Thread.Sleep(1000);
                var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-note]")));
                return stiNote != null;
            });

            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='備註內容']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcE100_06()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", stiEnd);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", stiEnd);
            stiEnd.SendKeys(Keys.Tab);

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#中結")));
            That(stiEnd.Selected);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);
            await Task.Delay(1000);

            stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            That(stiEnd.Selected);
        }
        public async Task TwcE100_07()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[@id='受理']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));

                return signElement != null;
            });
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);
            _driver.SwitchTo().Frame(0);

            var signElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@sti-post-user-full-name='']")));
            That(signElement.Text, Is.EqualTo("張博文"));
        }
        public async Task TwcE100_08()
        {   
            Thread.Sleep(1000);
            TestHelper.ClickElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='消費性用水服務契約']", 0);
        }
        public async Task TwcE100_09()
        {
            TestHelper.ClickElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司個人資料保護告知事項']", 0);
        }
        public async Task TwcE100_10()
        {
            TestHelper.ClickElementInWindow(_driver, "//label[@for='公司營業章程']", 1);

            TestHelper.HoverOverElementInWindow(_driver, "//label[@for='公司營業章程']", 0);
        }
        public async Task TwcE100_11()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//span[text()='簽名']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var signButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='簽名']")));
            _actions.MoveToElement(signButton).Click().Perform();

            _wait.Until(driver =>
            {
                var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));

                return signElement != null;
            });
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='簽名_001.tiff']")));
            That(signElement, Is.Not.Null);
        }
        public async Task TwcE100_12()
        {
            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//span[text()='啟動掃描證件']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var scanButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='啟動掃描證件']")));
            _actions.MoveToElement(scanButton).Click().Perform();

            _wait.Until(driver =>
            {
                var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));

                return imgElement != null;
            });
            await Task.Delay(1000);

            _driver.SwitchTo().Window(_driver.WindowHandles[1]);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//img[@alt='證件_005.tiff']"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//img[@alt='證件_005.tiff']")));
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcE100_13()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//button[contains(text(), '新增文件')]"));
                _actions.MoveToElement(element).Perform();

                return element.Displayed;
            });

            var addFileButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '新增文件')]")));
            That(addFileButton!.Displayed, Is.True);

            _actions.MoveToElement(addFileButton).Click().Perform();

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("storm-card[headline='新增檔案']"));

                return element;
            });

            var firstFile = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, firstFile, "input.dz-hidden-input:nth-of-type(3)");

            var secondFile = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, secondFile, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var fileName = _driver.FindElement(By.CssSelector("storm-input-group[label='名稱'] input"));
                return fileName.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var upload = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '上傳')]")));
            _actions.MoveToElement(upload).Click().Perform();

            _wait.Until(driver =>
            {
                var stormEditTable = driver.FindElement(By.CssSelector("storm-edit-table"));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

                return fileRows.Count >= 2;
            });

            var stormEditTable = _driver.FindElement(By.CssSelector("storm-edit-table"));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var fileRows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));

            var firstFileName = fileRows.Any(row =>
            {
                var fileNameElement = row.FindElement(By.CssSelector("td[data-field='name'] storm-table-cell span span"));
                return fileNameElement.Text == "twcweb_01_1_夾帶附件1.pdf";
            });
            That(firstFileName, Is.True);

            var secondFileName = fileRows.Any(row =>
            {
                var fileNameElement = row.FindElement(By.CssSelector("td[data-field='name'] storm-table-cell span span"));
                return fileNameElement.Text == "twcweb_01_1_夾帶附件2.pdf";
            });
            That(secondFileName, Is.True);
        }
        public async Task TwcE100_14()
        {
            _driver.SwitchTo().Window(_driver.WindowHandles[0]);

            var submitButton = _driver.FindElement(By.XPath("//button[contains(text(), '確認受理')]"));
            _actions.MoveToElement(submitButton).Click().Perform();

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            await TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(driver =>
            {
                var element = driver.FindElement(By.CssSelector("iframe"));
                return element != null;
            });

            _driver.SwitchTo().Frame(0);

            var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
            That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcE100_15()
        {
            var stiNote = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[text()='備註內容']")));
            That(stiNote.Text, Is.EqualTo("備註內容"));
        }
        public async Task TwcE100_16()
        {
            var checkStiEnd = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@id='中結']")));
            That(checkStiEnd.Selected);
        }
    }
}