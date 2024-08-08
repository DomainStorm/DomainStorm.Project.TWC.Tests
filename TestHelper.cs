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
            option.AddArgument("--headless");

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
        return response?.Access_token ?? throw new InvalidOperationException("Failed to get access token.");
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
    public static Task Login(IWebDriver webDriver, string userId, string password)
    {
        ((IJavaScriptExecutor)webDriver).ExecuteScript($"window.location.href = '{LoginUrl}';");

        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));

        Console.WriteLine($"::group::Login---------{LoginUrl}---------");
        Console.WriteLine($"---------{LoginUrl}---------");
        Console.WriteLine(webDriver.PageSource);
        Console.WriteLine("::endgroup::");
        wait.Until(ExpectedConditions.UrlContains("account"));

        Console.WriteLine($"::group::Login---------{webDriver.Url}---------");
        Console.WriteLine(webDriver.PageSource);
        Console.WriteLine("::endgroup::");

        var usernameElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
        var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));

        usernameElement.SendKeys(userId);
        passwordElement.SendKeys(password);

        var button = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
        button.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

        return Task.CompletedTask;
    }
    public static void ChangeUser(IWebDriver webDriver, string userName)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        Actions actions = new Actions(webDriver);

        var usernameElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
        usernameElement.SendKeys(userName);

        var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));
        passwordElement.SendKeys(TestHelper.Password!);

        var submitButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
        actions.MoveToElement(submitButton).Click().Perform();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
    }

    public static void ClickRow(IWebDriver webDriver, string applyCaseNo)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));

        Console.WriteLine($"::group::ClickRow---------{webDriver.Url}---------");
        Console.WriteLine(webDriver.PageSource);
        Console.WriteLine("::endgroup::");

        wait.Until(_ =>
        {
            var stormTable = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            return stormTable != null;
        });

        var stormTable = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-table")));
        var stormInputGroup = stormTable.GetShadowRoot().FindElement(By.CssSelector("storm-input-group"));
        var keyInput = stormInputGroup.FindElement(By.CssSelector("input"));
        keyInput.SendKeys(applyCaseNo);

        IWebElement? element = null;

        wait.Until(_ =>
        {
            var findElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("table > tbody > tr > td[data-field='applyCaseNo']"));
            Thread.Sleep(500);
            element = findElements.FirstOrDefault(e => e.Text == applyCaseNo);
            return element != null && !string.IsNullOrEmpty(element.Text) && element is { Displayed: true, Enabled: true };
        });

        var action = new Actions(webDriver);
        action.MoveToElement(element).Click().Perform();
    }
    public static string GetLastSegmentFromUrl(IWebDriver webDriver)
    {
        string targetUrl = webDriver.Url;

        WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        wait.Until(webDriver => webDriver.Url != targetUrl);

        string[] segments = webDriver.Url.Split('/');
        string uuid = segments[^1];

        return uuid;
    }
    public static void UploadFile(IWebDriver webDriver, string filePath, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var lastHiddenInput = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(css)));
        lastHiddenInput.SendKeys(filePath);
    }

    public static bool DownloadFileAndVerify(IWebDriver webDriver, string fileName, string css)
    {
        WebDriverWait _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
        Actions _actions = new Actions(webDriver);
        var _downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        if (!Directory.Exists(_downloadDirectory))
        {
            Directory.CreateDirectory(_downloadDirectory);
        }

        var filePath = Path.Combine(_downloadDirectory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var downloadButton = FindAndMoveElement(webDriver, css);
        downloadButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(css)));
        _actions.MoveToElement(downloadButton).Click().Perform();

        Console.WriteLine($"-----檢查檔案完整路徑|: {filePath}-----");

        _wait.Until(webDriver =>
        {
            Console.WriteLine($"-----{_downloadDirectory} GetFiles-----");
            foreach (var fn in Directory.GetFiles(_downloadDirectory))
            {
                Console.WriteLine($"-----filename: {fn}-----");
            }
            Console.WriteLine($"-----{_downloadDirectory} GetFiles end-----");
            return File.Exists(filePath);
        });
        return File.Exists(filePath);
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

public static IWebElement? WaitStormTableUpload(IWebDriver webDriver, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));

        return wait.Until(driver =>
        {
            IWebElement e = null;

            wait.Until(_ =>
            {
                try
                {
                    var stormTable = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));

                    if (stormTable != null)
                    {
                        e = stormTable.GetShadowRoot().FindElement(By.CssSelector(css));
                        return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            });

            return e;
        });
    }

    public static IWebElement? WaitStormEditTableUpload(IWebDriver webDriver, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));

        return wait.Until(_ =>
        {
            IWebElement e = null;

            wait.Until(_ =>
            {
                try
                {
                    var stormEditTable = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
                    var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));

                    if (stormTable != null)
                    {
                        e = stormTable.GetShadowRoot().FindElement(By.CssSelector(css));

                        return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            });

            return e;
        });
    }
    public static IWebElement? FindNavigationBySpan(IWebDriver webDriver, string spanText)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var action = new Actions(webDriver);

        return wait.Until(driver =>
        {
            // 定位到 storm-tree-view 的 ShadowRoot
            var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
            var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));

            if (stormTreeView != null)
            {
                var treeShadowRoot = stormTreeView.GetShadowRoot();

                // 查找所有 storm-tree-node 元素
                var treeNodes = treeShadowRoot.FindElements(By.CssSelector("storm-tree-node"));

                foreach (var treeNode in treeNodes)
                {
                    // 查找每個 storm-tree-node 中的 a 標籤的所有 span 標籤
                    var spans = treeNode.FindElements(By.CssSelector("a span"));

                    foreach (var span in spans)
                    {
                        // 檢查 span 標籤的文本是否包含指定的文本
                        if (span.Text.Contains(spanText))
                        {
                            return span; // 返回第一个匹配的 span 元素
                        }
                    }
                }
            }

            return null;
        });
    }

    public static IWebElement? FindShadowRootElement(IWebDriver webDriver, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        var action = new Actions(webDriver);

        return wait.Until(_ =>
        {
            IWebElement e = null;

            wait.Until(_ =>
            {
                try
                {
                    var stormVerticalNavigation = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-vertical-navigation")));
                    var stormTreeView = stormVerticalNavigation.GetShadowRoot().FindElement(By.CssSelector("storm-tree-view"));

                    if (stormTreeView != null)
                    {
                        e = stormTreeView.GetShadowRoot().FindElement(By.CssSelector(css));

                        return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            });

            return e;
        });
    }
    public static IWebElement FindAndMoveElement(IWebDriver webDriver, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var action = new Actions(webDriver);
        var element = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(css)));

        action.MoveToElement(element).Perform();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(css)));

        return element;
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