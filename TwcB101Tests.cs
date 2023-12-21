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

            var pTitle = TestHelper.WaitStormEditTableUpload(_driver, "td > p");
            That(pTitle!.Text, Is.EqualTo("沒有找到符合的結果"));
        }
        public async Task TwcB101_04()
        {
            var addFileButton = TestHelper.FindAndMoveElement(_driver, "[id='file'] > div.float-end > button");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='file'] > div.float-end > button")));
            _actions.MoveToElement(addFileButton).Click().Perform();

            var fileOne = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件1.pdf");
            TestHelper.UploadFile(_driver, fileOne, "input.dz-hidden-input:nth-of-type(3)");

            var fileTwo = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "twcweb_01_1_夾帶附件2.pdf");
            TestHelper.UploadFile(_driver, fileTwo, "input.dz-hidden-input:nth-of-type(3)");

            _wait.Until(driver =>
            {
                var fileName = TestHelper.FindAndMoveElement(_driver, "storm-card[headline='新增檔案'] > form > div > storm-input-group");
                return fileName!.GetAttribute("value") == "twcweb_01_1_夾帶附件1.pdf,twcweb_01_1_夾帶附件2.pdf";
            });

            var uploadButton = TestHelper.FindAndMoveElement(_driver, "div.d-flex.justify-content-end.mt-4 button[name='button']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.d-flex.justify-content-end.mt-4 button[name='button']")));
            _actions.MoveToElement(uploadButton).Click().Perform();

            _wait.Until(driver =>
            {
                var target = TestHelper.WaitStormEditTableUpload(_driver, "div.table-bottom > div.table-pageInfo");
                return target!.Text == "顯示第 1 至 2 筆，共 2 筆";            
            }); 

            var deleteButton = TestHelper.WaitStormEditTableUpload(_driver, "td.action > storm-table-cell > storm-table-toolbar > storm-button > storm-tooltip > div > button");
            _actions.MoveToElement(deleteButton).Click().Perform();

            var checkButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-actions > button.swal2-confirm");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(checkButton).Click().Perform();

            _wait.Until(driver =>
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
                var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                return rows.Count == 1;
            });
            That(TestHelper.WaitStormEditTableUpload(_driver, "storm-table-cell > span")!.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
        public async Task TwcB101_05()
        {
            var checkButton = TestHelper.FindAndMoveElement(_driver, "[id='用印或代送件只需夾帶附件']");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB101_06()
        {
            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > h5")));
            That(hintTitle.Text, Is.EqualTo("【受理】未核章"));
        }
        public async Task TwcB101_07()
        {
            var confirmButton = TestHelper.FindAndMoveElement(_driver, "div.swal2-popup > div.swal2-actions > button.swal2-confirm");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
            _actions.MoveToElement(confirmButton).Click().Perform();

            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }
        public async Task TwcB101_08()
        {
            _driver.SwitchTo().DefaultContent();

            var infoButton = TestHelper.FindAndMoveElement(_driver, "button.btn.bg-gradient-info.m-0.ms-2");
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            string targetUrl = $"{TestHelper.BaseUrl}/unfinished";
            _wait.Until(ExpectedConditions.UrlContains(targetUrl));
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var signNumber = TestHelper.FindAndMoveElement(_driver, "[sti-apply-case-no]");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            That(signNumber.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
        }
        public async Task TwcB101_09()
        {
            _driver.SwitchTo().DefaultContent();

            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_1'] > div.d-flex > div.form-check > input")));
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_2'] > div.d-flex > div.form-check > input")));
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='contract_3'] > div.d-flex > div.form-check > input")));
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcB101_10()
        {
            var checkFileName = TestHelper.FindAndMoveElement(_driver, "storm-card[id='file'] > div > a");
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-card[id='file'] > div > a")));
            That(checkFileName.Text, Is.EqualTo("twcweb_01_1_夾帶附件2.pdf"));
        }
    }
}