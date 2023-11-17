﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcD101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        public TwcD101Tests()
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
        public async Task TwcD101_01To09()
        {
            await TwcD101_01();
            await TwcD101_02();
            await TwcD101_03();
            await TwcD101_04();
            await TwcD101_05();
            await TwcD101_06();
            await TwcD101_07();
            await TwcD101_08();
            await TwcD101_09();
        }
        public async Task TwcD101_01()
        {
            TestHelper.AccessToken = await TestHelper.GetAccessToken();
            That(TestHelper.AccessToken, Is.Not.Empty);
        }
        public async Task TwcD101_02()
        {
            HttpStatusCode statusCode = await TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmAolishedApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-D101_bmAolishedApply.json"));
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        public async Task TwcD101_03()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");
            TestHelper.ClickRow(_driver, TestHelper.ApplyCaseNo!);

            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));
            _driver.SwitchTo().Frame(0);

            var stiEnd = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='中結']")));
            stiEnd.Click();
            That(stiEnd.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD101_04()
        {
            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='受理'] > span")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);

            var chceckSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[class='sign']")));
            That(chceckSign!, Is.Not.Null);
        }
        public async Task TwcD101_05()
        {
            _driver.SwitchTo().DefaultContent();

            var checkButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id='用印或代送件只需夾帶附件']")));
            _actions.MoveToElement(checkButton).Click().Perform();
            That(checkButton.GetAttribute("checked"), Is.EqualTo("true"));
        }
        public async Task TwcD101_06()
        {
            var infoButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.btn.bg-gradient-info.m-0.ms-2")));
            _actions.MoveToElement(infoButton).Click().Perform();

            var hintTitle = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-html-container > div.mx-6 > h5")));
            That(hintTitle.Text, Is.EqualTo("【夾帶附件】或【掃描拍照】未上傳"));
        }
        public async Task TwcD101_07()
        {
            _wait.Until(_ =>
            {
                try
                {
                    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1));
                    var confirmButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.swal2-popup > div.swal2-actions > button.swal2-confirm")));
                    if (confirmButton.Displayed)
                    {
                        _actions.MoveToElement(confirmButton).Click().Perform();
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return true;
                }
            });

            _driver.SwitchTo().DefaultContent();

            var scanButton = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card[id='credential'] > form > div > div > button.btn-primary")));
            _actions.MoveToElement(scanButton).Click().Perform();

            var scanSuccess = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.dropzone-container > div.dropzone > div:nth-child(6) > div.dz-success-mark")));
            That(scanSuccess, Is.Not.Null, "未上傳");
        }
        public async Task TwcD101_08()
        {
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
        public async Task TwcD101_09()
        {
            var 消費性用水服務契約 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_1'] > div.d-flex > div.form-check > input");
            That(消費性用水服務契約.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司個人資料保護告知事項 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_2'] > div.d-flex > div.form-check > input");
            That(公司個人資料保護告知事項.GetAttribute("checked"), Is.EqualTo("true"));

            var 公司營業章程 = TestHelper.FindAndMoveElement(_driver, "storm-card[id='contract_3'] > div.d-flex > div.form-check > input");
            That(公司營業章程.GetAttribute("checked"), Is.EqualTo("true"));

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));
            That(imgElement, Is.Not.Null);
        }

        [Test]
        [Order(1)]
        public async Task TwcD101_10To12()
        {
            await TwcD101_10();
            await TwcD101_11();
            await TwcD101_12();
        }
        public async Task TwcD101_10()
        {
            await TestHelper.Login(_driver, "0511", TestHelper.Password!);
            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/search");

            var applyDateBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[label='受理日期起']")));
            var input = applyDateBegin.GetShadowRoot().FindElement(By.CssSelector("input"));
            applyDateBegin = _wait.Until(ExpectedConditions.ElementToBeClickable(input));
            _actions.MoveToElement(applyDateBegin).Click().Perform();

            var select = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-current-month select")));
            var applyMonthBegin = new SelectElement(select);
            applyMonthBegin.SelectByText("June");

            var applyDayBegin = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.flatpickr-calendar.open div.flatpickr-innerContainer div.flatpickr-days span[aria-label='June 3, 2023']")));
            _actions.MoveToElement(applyDayBegin).Click().Perform();

            var search = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("storm-card.mb-3.hydrated > div.d-flex.justify-content-end.mt-4 > button")));
            _actions.MoveToElement(search).Click().Perform();
            That(TestHelper.WaitStormTableUpload(_driver, "td[data-field='applyCaseNo'] > storm-table-cell > span"), Is.Not.Null);
        }
        public async Task TwcD101_11()
        {
            var stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var applyCaseNo = stormTable.GetShadowRoot().FindElement(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo'] > storm-table-cell > span"));
            _actions.MoveToElement(applyCaseNo).Click().Perform();

            var imgElement = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-card:nth-child(6) > img")));
            That(imgElement, Is.Not.Null);
        }
        public async Task TwcD101_12()
        {
            That(TestHelper.DownloadFileAndVerify(_driver, "41104433664.pdf", "storm-card[id='finished'] > div.float-end > div:nth-child(3) > button"), Is.True);
        }
    }
}