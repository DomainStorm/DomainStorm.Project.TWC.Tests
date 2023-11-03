﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcB101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcB101Tests()
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
        public async Task TwcB101_01To10()
        {
            await TwcB101_01();
            await TwcB101_02();
            await TwcB101_03();
            await TwcB101_04();
            await TwcB101_05();
            await TwcB101_06();
            await TwcB101_07();
            await TwcB101_08();
            await TwcB101_09();
            await TwcB101_10();
        }
        public async Task TwcB101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();

            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcB101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmRecoverApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-B101_bmRecoverApply.json"));

            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task TwcB101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var pTitle = stormTable.GetShadowRoot().FindElement(By.CssSelector("td > p"));
            That(pTitle.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcB101_04()
        {
            var addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(3)")));
            var firstFile = "twcweb_01_1_夾帶附件1.pdf";
            var firstFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", firstFile);
            lastHiddenInput.SendKeys(firstFilePath);

            var stormInputGroup = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[headline='新增檔案'] > form > div > storm-input-group")));
            var fileName = stormInputGroup.GetAttribute("value");
            That(fileName, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            var uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();
            That(WaitStormEditTableUpload(_driver), Is.Not.Null);

            addFileButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            lastHiddenInput = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input.dz-hidden-input:nth-of-type(4)")));
            var secondFile = "twcweb_01_1_夾帶附件2.pdf";
            var secondFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", secondFile);
            lastHiddenInput.SendKeys(secondFilePath);

            uploadButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            That(WaitStormEditTableUpload(_driver), Is.Not.Null);

            var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var deleteStormButton = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-button > storm-tooltip > div > button"));
            _actions.MoveToElement(deleteStormButton).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-confirm");
            checkButton.Click();

            Thread.Sleep(1000);
            var spanTitle = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
            That(spanTitle.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public async Task TwcB101_05()
        {
            var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            var finishedButton = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));
            _actions.MoveToElement(finishedButton).Click().Perform();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB101_06()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-html-container > h5")));
            That(hintTitle.Text, Is.EqualTo("【受理】未核章"));
        }
        public async Task TwcB101_07()
        {
            var closeButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            closeButton.Click();

            _driver.SwitchTo().Frame(0);

            var 受理 = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", 受理);
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#受理")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", 受理);

            var signElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            var signElementExists = signElement != null;
            That(signElementExists, Is.True, "未受理");
        }
        public async Task TwcB101_08()
        {
            _driver.SwitchTo().DefaultContent();

            //var stormVerticalNavigation = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            //var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));
            //var finishedButton = stormTreeView.GetShadowRoot().FindElement(By.CssSelector("a[href='#finished']"));
            //_actions.MoveToElement(finishedButton).Click().Perform();

            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "storm-card:nth-child(9) > div.row > div.col-sm-7");
            That(signNumber.GetAttribute("textContent"), Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcB101_09()
        {
            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(消費性用水服務契約).Click().Perform();

            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司個人資料保護告知事項).Click().Perform();

            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _actions.MoveToElement(公司營業章程).Click().Perform();

            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.Not.Null);

        }
        public async Task TwcB101_10()
        {
            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            That(checkFileName.GetAttribute("download"), Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public static IWebElement? WaitStormEditTableUpload(IWebDriver _driver)
        {
            WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            return _wait.Until(_ =>
            {
                var e = _wait.Until(_ =>
                {
                    var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                    var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

                    try
                    {
                        return stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-table-cell > span"));
                    }
                    catch
                    {
                        // ignored
                    }
                    return null;
                });
                return !string.IsNullOrEmpty(e.Text) ? e : null;
            });
        }
    }
}