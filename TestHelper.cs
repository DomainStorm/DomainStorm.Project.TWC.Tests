using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SeleniumExtras.WaitHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using System.Data.SqlClient;
using Dapper;
using static NUnit.Framework.Assert;
using AngleSharp.Dom;
using System.Xml.Linq;
using System;

namespace DomainStorm.Project.TWC.Tests;
public class TestHelper
{
    private static List<ChromeDriver> _chromeDriverList = new List<ChromeDriver>();
    public static ChromeDriver GetNewChromeDriver()
    {
        var option = new ChromeOptions();
        option.AddArgument("--start-maximized");
        option.AddArgument("--disable-gpu");
        option.AddArgument("--enable-javascript");
        option.AddArgument("--allow-running-insecure-content");
        option.AddArgument("--ignore-urlfetcher-cert-requests");
        option.AddArgument("--disable-web-security");
        option.AddArgument("--ignore-certificate-errors");

        if (GetChromeConfig().Headless)
        {
            option.AddArgument("--headless");
            option.AddArgument("--start-maximized");
        }

        var downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        option.AddUserProfilePreference("download.default_directory", downloadDirectory);

        new DriverManager().SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig());
        var driver = new ChromeDriver(option);

        _chromeDriverList.Add(driver);

        //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);

        return driver;
    }
    public static void CloseChromeDrivers()
    {
        foreach (var driver in _chromeDriverList)
        {
            driver.Quit();
        }
    }
    private static TestConfig GetTestConfig()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<TestHelper>()
            .AddEnvironmentVariables()
            .Build()
            .GetSection("TestConfig")
            .Get<TestConfig>();
    }
    private static string? _baseUrl;
    public static string? BaseUrl
    {
        get
        {
            _baseUrl ??= GetTestConfig().BaseUrl;
            return _baseUrl;
        }
    }

    private static string? _tokenUrl;
    public static string? TokenUrl
    {
        get
        {
            _tokenUrl ??= GetTestConfig().TokenUrl;
            return _tokenUrl;
        }
    }
    private static string? _loginUrl;
    public static string? LoginUrl
    {
        get
        {
            _loginUrl ??= GetTestConfig().LoginUrl;
            return _loginUrl;
        }
    }
    private static string? _applyCaseNo;
    private static string? _applyEmailAddr;
    public static string? ApplyCaseNo
    {
        get
        {
            _applyCaseNo ??= GetTestConfig().ApplyCaseNo;
            return _applyCaseNo;
        }
    }

    private static string? _password;
    public static string? Password
    {
        get
        {
            _password ??= GetTestConfig().Password;
            return _password;
        }
    }
    private static string? _accessToken;
    public static string? AccessToken
    {
        get
        {
            _accessToken ??= GetTestConfig().AccessToken;
            return _accessToken;
        }
        set => _accessToken = value;
    }
    public static ChromeConfig GetChromeConfig()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<TestHelper>()
            .AddEnvironmentVariables()
            .Build()
            .GetSection("ChromeConfig")
            .Get<ChromeConfig>();
    }
    public class TokenResponse
    {
        public string? Access_token { get; set; }
    }
    public static async Task<string> GetAccessToken()
    {
        var client = new RestClient(TokenUrl!);
        var request = new RestRequest();
        const string clientId = "bmuser";
        const string clientSecret = "4xW8KpkKkeFc";
        var encodedData = Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(clientId + ":" + clientSecret));
        request.AddHeader("Authorization", $"Basic {encodedData}");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("scope", "template_read post_read serialNumber_write dublinCore_write");

        var response = await client.PostAsync<TokenResponse>(request);
        var accessToken = response?.Access_token ?? throw new InvalidOperationException("Failed to get access token.");

        Console.WriteLine($"Access Token: {accessToken}");

        return accessToken;
    }
    public static async Task<HttpStatusCode> CreateForm(string accessToken, string apiUrl, string jsonFilePath, bool modifyApplyDate = false)
    {
        var client = new RestClient(apiUrl);
        var request = new RestRequest
        {
            Method = Method.Post
        };
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        using var r = new StreamReader(jsonFilePath);
        var json = await r.ReadToEndAsync();
        var update = JsonConvert.DeserializeObject<WaterForm>(json)!;

        if (modifyApplyDate)
        {
            DateTime ApplyDate = DateTime.Now.AddDays(-2);
            string formattedApplyDate = ApplyDate.ToString("yyyy-MM-dd");
            update.ApplyDate = formattedApplyDate;
        }

        _applyCaseNo = DateTime.Now.ToString("yyyyMMddHHmmss");
        update.ApplyCaseNo = _applyCaseNo;

        var updatedJson = JsonConvert.SerializeObject(update);
        Console.WriteLine(updatedJson);
        request.AddParameter("application/json", updatedJson, ParameterType.RequestBody);

        var response = await client.ExecuteAsync(request);

        if (response.ErrorException != null)
            Console.WriteLine(response.Content);

        return response.StatusCode;
    }

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly Actions _actions;
    public TestHelper(IWebDriver webDriver)
    {
        _driver = webDriver;
        _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        _actions = new Actions(webDriver);
    }

    public void Login(string userId, string password)
    {
        _driver.Navigate().GoToUrl(LoginUrl);

        //Console.WriteLine($"::group::Login---------{LoginUrl}---------");
        //Console.WriteLine($"---------{LoginUrl}---------");
        //Console.WriteLine(webDriver.PageSource);
        //Console.WriteLine("::endgroup::");
        _wait.Until(ExpectedConditions.UrlContains("account"));

        //Console.WriteLine($"::group::Login---------{webDriver.Url}---------");
        //Console.WriteLine(webDriver.PageSource);
        //Console.WriteLine("::endgroup::");

        WaitElementVisible(By.CssSelector("[name=Username]"));
        InputSendKeys(By.CssSelector("[name=Username]"), userId);
        InputSendKeys(By.CssSelector("[name=Password]"), password);
        WaitElementVisible(By.CssSelector("button"));
        ElementClick(By.CssSelector("button"));

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
    }
    public void CheckPageException()
    {
        try
        {
            var webElement = new WebDriverWait(_driver, TimeSpan.FromSeconds(3)).Until(ExpectedConditions.ElementExists(By.CssSelector("div.alert-danger")));
            throw new Exception(webElement.Text);
        }
        catch(WebDriverTimeoutException)
        {

        }
    }

    public IWebElement WaitElementExists(By by)
    {
        return _wait.Until(driver =>
        {
            CheckPageException();
            return ExpectedConditions.ElementExists(by).Invoke(driver);
        });
    }

    public IWebElement WaitElementVisible(By by)
    {
        return _wait.Until(driver =>
        {
            CheckPageException();
            return ExpectedConditions.ElementIsVisible(by).Invoke(driver);
        });
    }

    public IWebElement ElementClick(By by)
    {
        return _wait.Until(_ =>
        {
            _actions.ScrollToElement(WaitElementExists(by)).Perform();
            var element = WaitElementVisible(by);
            _actions.MoveToElement(element).Perform();
            _wait.Until(_ => element.Enabled);
            _actions.Click(element).Perform();

            return element;
        }); ;
    }

    public void NavigateWait(string url, By by, int retryCount = 1)
    {

        _driver.Navigate().GoToUrl($"{BaseUrl}{url}");
        _wait.Until(ExpectedConditions.UrlToBe($"{BaseUrl}{url}"));

        try
        {
            WaitElementExists(by);
        }
        catch(Exception)
        {
            if (retryCount > 0)
            {
                Console.Write($"NavigateWait retry retryCount: {retryCount}, url: {_driver.Url}");
                _driver.Navigate().Refresh();
                NavigateWait(url, by, retryCount -1);
            }
        }
    }

    public IWebElement InputSendKeys(By by, string text)
    {
        var element = WaitElementExists(by);
        element.SendKeys(text);

        return element;
    }
    public void UploadFilesAndCheck(string[] fileNames, string cssSelectorInput)
    {
        WaitElementExists(By.CssSelector(cssSelectorInput));

        var filePaths = fileNames.Select(fileName => Path.Combine(Directory.GetCurrentDirectory(), "Assets", fileName)).ToArray();
        var currentFileNames = new List<string>();

        foreach (var filePath in filePaths)
        {
            InputSendKeys(By.CssSelector(cssSelectorInput), filePath);
            currentFileNames.Add(Path.GetFileName(filePath));
            CheckFileName(string.Join(",", currentFileNames));

            var fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension == ".png")
            {
                var durationInput = _wait.Until(d => d.FindElement(By.XPath("//storm-input-group[@label='播放秒數']//input")));
                durationInput.SendKeys("10");
            }
        }
        // Thread.Sleep(1000);
        WaitElementVisible(By.XPath("//a[@class='dz-remove']"));

        var uploadButton = By.XPath("//button[text()='上傳']");
        WaitElementVisible(uploadButton);
        ElementClick(uploadButton);
        //dz-remove
    }

    private void CheckFileName(string expectedFileName)
    {
        var fileNameInputSelector = By.XPath("//storm-input-group[@label='名稱']//input");
        WaitElementExists(fileNameInputSelector);

        _wait.Until(driver =>
        {
            var fileNameInput = driver.FindElement(fileNameInputSelector);
            return fileNameInput.GetAttribute("value") == expectedFileName;
        });

        var fileNameInput = _driver.FindElement(fileNameInputSelector);
        That(fileNameInput.GetAttribute("value"), Is.EqualTo(expectedFileName));
    }


    public IWebElement? WaitShadowElement(string cssSelector, string? expectedText = null, bool isEditTable = false)
    {
        IWebElement GetStormTable()
        {
            IWebElement stormTable;

            if (isEditTable)
            {
                var stormEditTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            }
            else
            {
                stormTable = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            }

            return stormTable;
        }

        try
        {
            _wait.Until(_ => GetStormTable().Displayed);
            _wait.Until(_ => GetStormTable().GetShadowRoot().FindElements(By.CssSelector(cssSelector)).Any());
            _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(GetStormTable().GetShadowRoot().FindElements(By.CssSelector(cssSelector))));

            IWebElement? ExpectedText(int retryCount = 1)
            {
                try
                {
                    return _wait.Until(_ =>
                    {
                        var targetElements = GetStormTable().GetShadowRoot().FindElements(By.CssSelector(cssSelector));
                        var targetElement = targetElements.FirstOrDefault();
                        if (targetElement == null)
                            return null;

                        if (expectedText != null)
                            return targetElement?.Text == expectedText ? targetElement : null;

                        return targetElement;
                    });
                }
                catch (Exception)
                {
                    if (retryCount <= 0) throw;

                    Console.Write($"ExpectedText retry retryCount: {retryCount}");
                    return ExpectedText(retryCount);

                }
            }

            return ExpectedText();
        }
        catch (Exception e)
        {
            var blazor_error_ui = _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#blazor-error-ui")));
            if (blazor_error_ui.Displayed)
                throw new Exception("blazor_error");

            Console.WriteLine("::group::--------------------WaitShadowElement Exception start --------------------");
            Console.WriteLine("--------------------");
            Console.WriteLine(e);
            Console.WriteLine(_wait.Until(ExpectedConditions.ElementExists(By.CssSelector("body"))).GetAttribute("innerHTML"));
            Console.WriteLine("::endgroup::--------------------WaitShadowElement Exception end --------------------");
            throw;
        }

    }
    public void MoveAndCheck(string cssSelector)
    {
        var element = _driver.FindElement(By.CssSelector(cssSelector));
        _actions.ScrollToElement(element).MoveToElement(element).Perform();
        That(element.Selected);
    }
    public void CheckElementText(string cssSelector, string expectedText)
    {
        //var element = _driver.FindElement(By.CssSelector(cssSelector));
        //_actions.MoveToElement(element).Perform();
        //That(element.Text, Is.EqualTo(expectedText));

        CheckElementText(By.CssSelector(cssSelector), expectedText);
    }

    public void CheckElementText(By cssSelector, string expectedText)
    {
        var element = WaitElementExists(cssSelector);
        _actions.ScrollToElement(element).MoveToElement(element).Perform();
        _wait.Until(_ => element.Text == expectedText);
    }

    public void ClickRow(string caseNo)
    {
        WaitElementExists(By.CssSelector("storm-table"));
        WaitElementVisible(By.CssSelector("storm-table"));
        IWebElement? selectedRow = null;

        var url = _driver.Url;
        if (!url.Contains("/search"))
        {
            // 如果 URL 不包含 "/search"，處理有輸入框的情況
            var stormTableInput = _wait.Until(_ =>
            {
                var elements = WaitElementVisible(By.CssSelector("storm-table")).GetShadowRoot().FindElements(By.CssSelector("input[placeholder='請輸入關鍵字']"));
                if (!elements.Any()) return null;
                var element = elements.First();
                _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(elements));
                return element;
            });

            if (stormTableInput != null)
            {
                stormTableInput.Clear();
                stormTableInput.SendKeys(caseNo + Keys.Enter);

                _wait.Until(_ => WaitElementVisible(By.CssSelector("storm-table")).GetShadowRoot().FindElements(By.CssSelector("tbody > tr")).Count == 1);

                var row = WaitElementVisible(By.CssSelector("storm-table")).GetShadowRoot().FindElement(By.CssSelector("tbody > tr"));
                _wait.Until(_ => row.Displayed);
                _wait.Until(ExpectedConditions.TextToBePresentInElement(row, caseNo));
                selectedRow = row;
            }
        }
        else
        {
            selectedRow = _wait.Until(_ =>
            {
                var rows = WaitElementVisible(By.CssSelector("storm-table")).GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
                _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(rows));

                foreach (var row in rows)
                {
                    var applyCaseNoElement = _wait.Until(_ => row.FindElement(By.CssSelector("td[data-field='applyCaseNo']")));
                    if (applyCaseNoElement.Text == caseNo)
                        return row;
                }

                return null;
            });
        }

        if (selectedRow != null)
        {
            var radioButton = _wait.Until(_ => selectedRow.FindElement(By.CssSelector("td[data-field='__radio_1']")));
            _actions.MoveToElement(radioButton).Click().Perform();

            _wait.Until(ExpectedConditions.StalenessOf(radioButton));
            
            _wait.Until(webDriver => !ExpectedConditions.UrlToBe(url).Invoke(webDriver));
        }
        else
        {
            throw new Exception($"找不到匹配的 applyCaseNo: {caseNo}");
        }
    }

    public void DownloadFileAndVerify(string fileName, string xpath)
    {
        var downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        var filePath = Path.Combine(downloadDirectory, fileName);

        Directory.CreateDirectory(downloadDirectory);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        ElementClick(By.XPath(xpath));

        bool fileDownloaded = _wait.Until(driver =>
        {
            foreach (var file in Directory.GetFiles(downloadDirectory))
            {
                Console.WriteLine($"-----filename: {file}-----");
            }
            return File.Exists(filePath);
        });

        if (!fileDownloaded)
        {
            throw new Exception($"File '{fileName}' was not downloaded successfully.");
        }
    }
    public void OpenNewWindowWithLastSegmentUrlAndVerify()
    {
        string[] segments = _driver.Url.Split('/');
        string uuid = segments[^1];

        ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
        _driver.Navigate().GoToUrl($"{BaseUrl}/draft/second-screen/{uuid}");

        WaitElementExists(By.CssSelector("iframe"));

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe")));
        _driver.SwitchTo().Frame(0);

        var applyCaseNo = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[sti-apply-case-no]")));
        That(applyCaseNo.Text, Is.EqualTo(TestHelper.ApplyCaseNo));
    }

    public void SwitchWindowAndClick(string xpath)
    {
        ElementClick(By.XPath(xpath));

        _driver.SwitchTo().Window(_driver.WindowHandles[0]);
        _driver.SwitchTo().DefaultContent();

        WaitElementExists(By.XPath(xpath));

        var element = _driver.FindElement(By.XPath(xpath));
        _actions.ScrollToElement(element).MoveToElement(element).Perform();

        _driver.SwitchTo().Window(_driver.WindowHandles[1]);
        _driver.SwitchTo().DefaultContent();
    }

    public static void CleanDb()
    {
        if (GetChromeConfig().CleanDbable)
        {
            var client = new RestClient();
            var request = new RestRequest("http://localhost:9200/dublincore", Method.Delete);
            client.Execute(request);

            request.Method = Method.Put;
            client.Execute(request);
            using var cn = new SqlConnection("Server=localhost,5434;Database=TWCWeb;User Id=sa;Password=Pass@word");
            cn.Query("delete MainFile");
            cn.Query("delete WaterRegisterChangeForm");
            cn.Query("delete WaterRegisterLog");
            cn.Query("delete AttachmentFile");
            cn.Query("delete FormAttachment");
            cn.Query("delete Form");
            cn.Query("delete MediaFile");
            cn.Query("delete PlayList");
            cn.Query("delete PlayListItem");
            cn.Query("delete Question");
            cn.Query("delete QuestionOption");
            cn.Query("delete Questionnaire");
            cn.Query("delete QuestionnaireForm");
            cn.Query("delete QuestionnaireFormAnswer");
        }
    }
    public class WaterForm
    {
        public string? ApplyCaseNo { get; set; }
        public string? ApplyDate { get; set; }
        public string? OperatingArea { get; set; }
        public string? WaterNo { get; set; }
        public string? TypeChange { get; set; }
        public string? UserCode { get; set; }
        public string? ChangeAddress { get; set; }
        public string? CancelPayAccount { get; set; }
        public string? CancelEbill { get; set; }
        public string? ApplyEbill { get; set; }
        public string? CancelSmsBill { get; set; }
        public string? ApplyEmailAddr { get; set; }
        public string? DeviceLocation { get; set; }
        public string? Applicant { get; set; }
        public string? IdNo { get; set; }
        public string? Unino { get; set; }
        public string? TelNo { get; set; }
        public string? MobileNo { get; set; }
        public string? PipeDiameter { get; set; }
        public string? WaterType { get; set; }
        public string? ScoreSheet { get; set; }
        public string? WaterBuildLic { get; set; }
        public string? WaterUseLic { get; set; }
        public string? BillAddress { get; set; }
    }
    public class TestConfig
    {
        public string? BaseUrl { get; set; }
        public string? TokenUrl { get; set; }
        public string? LoginUrl { get; set; }
        public string? AccessToken { get; set; }
        public string? ApplyCaseNo { get; set; }
        public string? Password { get; set; }
    }
    public class ChromeConfig
    {
        public bool Headless { get; set; }
        public bool CleanDbable { get; set; }
    }
}