﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class TwcA101Tests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private Actions _actions = null!;
        private TestHelper _testHelper =null!;
        public TwcA101Tests()
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
        [NoBrowser]

        public Task TwcA101_01()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(1)]
        [NoBrowser]

        public Task TwcA101_02()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(2)]
        public Task TwcA101_03To04()
        {
            TwcA101_03();
            TwcA101_04();

            return Task.CompletedTask;
        }

        public Task TwcA101_03()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-table"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.ElementClick(By.XPath("//button[text()='捨棄草稿']"));
            _testHelper.WaitElementExists(By.XPath("//h2[text()='是否刪除？']"));

            var check = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[text()='刪除']")));
            That(check.Text, Is.EqualTo("刪除"));

            return Task.CompletedTask;
        }

        public Task TwcA101_04()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='刪除']"));

            var content = _testHelper.WaitShadowElement("p", "沒有找到符合的結果");
            That(content!.Text, Is.EqualTo("沒有找到符合的結果"));

            return Task.CompletedTask;
        }

        [Test]
        [Order(3)]
        [NoBrowser]
        public Task TwcA101_05()
        {
            TestHelper.AccessToken = TestHelper.GetAccessToken().Result;
            That(TestHelper.AccessToken, Is.Not.Empty);

            return Task.CompletedTask;
        }

        [Test]
        [Order(4)]
        [NoBrowser]
        public Task TwcA101_06()
        {
            var statusCode = TestHelper.CreateForm(TestHelper.AccessToken!, $"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/twcweb-A101_bmEnableApply.json")).Result;
            That(statusCode, Is.EqualTo(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        [Test]
        [Order(5)]
        public Task TwcA101_07To13()
        {
            TwcA101_07();
            TwcA101_08();
            TwcA101_09();
            TwcA101_10();
            TwcA101_11();
            TwcA101_12();
            TwcA101_13();

            return Task.CompletedTask;
        }
        public Task TwcA101_07()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/draft", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.ElementClick(By.XPath("//button[text()='新增文件']"));

            return Task.CompletedTask;
        }
        public Task TwcA101_08()
        {
            _testHelper.UploadFilesAndCheck(new[] { "twcweb_01_1_夾帶附件1.pdf" }, "input.dz-hidden-input:nth-of-type(3)");

            return Task.CompletedTask;
        }
        public Task TwcA101_09()
        {
            var content = _testHelper.WaitShadowElement("td[data-field='name'] span span", "twcweb_01_1_夾帶附件1.pdf", isEditTable: true);
            That(content!.Text, Is.EqualTo("twcweb_01_1_夾帶附件1.pdf"));

            return Task.CompletedTask;
        }
        public Task TwcA101_10()
        {
             _testHelper.ElementClick(By.CssSelector("#用印或代送件只需夾帶附件"));

            _wait.Until(ExpectedConditions.ElementToBeSelected(By.CssSelector("#用印或代送件只需夾帶附件")));

            return Task.CompletedTask;
        }
        public Task TwcA101_11()
        {
            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            var content = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h5[text()='【受理】未核章']")));
            That(content.Text, Is.EqualTo("【受理】未核章"));

            _testHelper.ElementClick(By.XPath("//button[text()='確定']"));

            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[text()='確定']")));

            return Task.CompletedTask;
        }
        public Task TwcA101_12()
        {
            _driver.SwitchTo().Frame(0);

            var acceptSign = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#accept-sign")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", acceptSign);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", acceptSign);

            That(_wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@sti-post-user-full-name][text()='張博文']"))), Is.Not.Null);

            return Task.CompletedTask;
        }
        public Task TwcA101_13()
        {
            _driver.SwitchTo().DefaultContent();

            _testHelper.ElementClick(By.XPath("//button[text()='確認受理']"));

            _wait.Until(ExpectedConditions.UrlContains($"{TestHelper.BaseUrl}/unfinished"));

            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().Frame(0);

            return Task.CompletedTask;
        }

        [Test]
        [Order(6)]
        public Task TwcA101_14()
        {
            _testHelper.Login("0511", TestHelper.Password!);
            _testHelper.NavigateWait("/unfinished", By.CssSelector("storm-sidenav"));
            _testHelper.ClickRow(TestHelper.ApplyCaseNo!);
            _testHelper.WaitElementExists(By.CssSelector("iframe"));

            _driver.SwitchTo().DefaultContent();

            _testHelper.MoveAndCheck("#消費性用水服務契約");
            _testHelper.MoveAndCheck("#公司個人資料保護告知事項");
            _testHelper.MoveAndCheck("#公司營業章程");
            _testHelper.CheckElementText("a[download='twcweb_01_1_夾帶附件1.pdf']", "twcweb_01_1_夾帶附件1.pdf");

            return Task.CompletedTask;
        }
    }
}