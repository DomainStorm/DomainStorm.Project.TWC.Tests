﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcS101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcS101Tests()
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
        public async Task TwcS101_01() // 15次皆無錯誤。
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            for (int i = 0; i < 15; i++)
            {
                TestHelper.AccessToken = await TestHelper.GetAccessToken();
                HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json"));
                _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
                _driver.SwitchTo().Frame(0);

                var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
                _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id='受理']")));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

                var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
                That(chceckSign != null, "未受理");

                _driver.SwitchTo().DefaultContent();

                var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
                _actions.MoveToElement(checkButton).Click().Perform();
                That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));

                var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
                _actions.MoveToElement(addFileButton).Click().Perform();

                var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
                var twcweb_01_1_夾帶附件1 = "twcweb_01_1_夾帶附件1.pdf";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", twcweb_01_1_夾帶附件1);
                lastHiddenInput.SendKeys(filePath);

                var fileName = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
                That(fileName.GetAttribute("value"), Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

                var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
                _actions.MoveToElement(uploadButton).Click().Perform();
                That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span"), Is.Not.Null);

                _wait.Until(_ =>
                {
                    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                    var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
                    _actions.MoveToElement(infoButton).Click().Perform();
                    try
                    {
                        if (infoButton.Displayed)
                        {
                            _actions.MoveToElement(infoButton).Click().Perform();
                            return false;
                        }
                        return true;
                    }
                    catch
                    {
                        return true;
                    }
                });

                var targetUrl = $"{TestHelper.BaseUrl}/unfinished";
                _wait.Until(ExpectedConditions.UrlContains(targetUrl));
                TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

                var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
                That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
            }

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/unfinished");
            That(TestHelper.WaitStormTableUpload(_driver, "tr > td"), Is.Not.Null);
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-bottom > div.table-pageInfo"));
            That(pageInfo.Text, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }

        [Test]
        [Order(1)]
        public async Task TwcS101_02To04()
        {
            await TwcS101_02();
        }
        public async Task TwcS101_02() // 查詢後資料清單列表顯示有10筆，畫面下方顯示有第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 6, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
        public async Task TwcS101_03() // 顯示清單畫面切換為5筆，下方顯示第 11 至 5 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 4, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 11 至 5 筆，共 15 筆"));
        }
        public async Task TwcS101_04() // 顯示清單畫面切換至第一頁10筆，下方顯示第 1 至 10 筆，共 15 筆。
        {
            ChromeDriver driver =TestHelper.GetNewChromeDriver();

            await TestHelper.Login(driver, "0511", TestHelper.Password!);
            driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            var stormMainContent = driver.FindElement(By.CssSelector("storm-main-content"));
            var stormCard = stormMainContent.FindElement(By.CssSelector("storm-card"));
            var div = stormCard.FindElement(By.CssSelector("div.row"));
            var stormInputGroup = div.FindElement(By.CssSelector("storm-input-group"));
            var inputElement = stormInputGroup.GetShadowRoot().FindElement(By.CssSelector("input"));
            inputElement.Click();
            Thread.Sleep(500);

            var monthDropdown = driver.FindElement(By.ClassName("flatpickr-monthDropdown-months"));
            SelectElement selectMonth = new SelectElement(monthDropdown);
            selectMonth.SelectByText("March");

            var span = driver.FindElement(By.CssSelector("span[aria-label='March 5, 2023']"));
            span.Click();

            var divElement = stormCard.FindElement(By.CssSelector("div.d-flex.justify-content-end.mt-4"));
            var 查詢 = divElement.FindElement(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2"));

            Actions actions = new(driver);
            actions.MoveToElement(查詢).Click().Perform();

            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            var stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            var stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));

            var stormPagination = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-pagination"));
            span = stormPagination.GetShadowRoot().FindElement(By.CssSelector("span.material-icons"));

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            actions.MoveToElement(span).Click().Perform();
            Thread.Sleep(500);

            stormMainContent = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-main-content")));
            stormCard = stormMainContent.FindElements(By.CssSelector("storm-card"))[1];
            stormDocumentListDetail = stormCard.FindElement(By.CssSelector("storm-document-list-detail"));
            stormTable = stormDocumentListDetail.FindElement(By.CssSelector("storm-table"));
            var pageInfo = stormTable.GetShadowRoot().FindElement(By.CssSelector("div.table-pageInfo"));
            string pageInfoText = pageInfo.Text;

            //That(pageInfoText, Is.EqualTo("顯示第 1 至 10 筆，共 15 筆"));
        }
    }
}