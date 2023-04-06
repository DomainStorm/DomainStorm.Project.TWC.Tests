using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using static NUnit.Framework.Assert;

namespace DomainStorm.Project.TWC.Tests
{
    public class Twc01_1Tests
    {
        private IWebDriver _driver = null!;
        private string? _accessToken;

        public Twc01_1Tests()
        {
        }

        [SetUp]
        public Task Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return Task.CompletedTask;
        }
        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        [Order(0)]
        public async Task Twc01_01()
        {
            _accessToken ??= await TestHelper.GetAccessToken();
            That(_accessToken, Is.Not.Empty);
        }

        [Test]
        [Order(1)]
        public async Task Twc01_02()
        {
            var client = new RestClient($"{TestHelper.BaseUrl}/api/v1/bmEnableApply/confirm");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

            using var r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/tpcweb_01_1�ҥ�_bmEnableApply.json"));
            var json = await r.ReadToEndAsync();

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.PostAsync(request);
            That(response.IsSuccessful, Is.True);
        }

        [Test]
        [Order(2)]
        public async Task Twc01_03()
        {
            await TestHelper.Login(_driver, "admin", "admin");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");

            TestHelper.ClickRow(_driver, "111123");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var ���z�s�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-case-no]")));
            var ���� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-no]")));
            var ���z��� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-apply-date]")));
            var �����Ҹ��X = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-id-no]")));
            var ��Q�Ʒ~�Τ@�s�� = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-uni-no]")));

            That(���z�s��.Text, Is.EqualTo("111123"));
            That(����.Text, Is.EqualTo("41101234568"));
            That(���z���.Text, Is.EqualTo("41101234568"));
            That(�����Ҹ��X.Text, Is.EqualTo("A123456789"));
            That(��Q�Ʒ~�Τ@�s��.Text, Is.EqualTo("16753217"));
        }

        [Test]
        [Order(3)]
        public async Task Twc01_04()
        {
            await TestHelper.Login(_driver, "admin", "admin");

            _driver.Navigate().GoToUrl($@"{TestHelper.BaseUrl}/draft");

            TestHelper.ClickRow(_driver, "111123");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe")));

            _driver.SwitchTo().Frame(0);

            var waterBuildLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-build-lic]")));
            var waterUseLic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[sti-water-use-lic]")));

            That(waterBuildLic.Text, Is.EqualTo("�ؿv���Ӹ��X109�����u�ثئr00322��\r\n�طӵo�Ӥ��111�~12��13��"));
            That(waterUseLic.Text, Is.EqualTo("�ؿv�ϥΰ���111�����u�بϦr000990��\r\nA01���ҥγ�Ϸӵo�Ӥ��111�~06��28��"));
        }


    }
}