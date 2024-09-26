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

        WaitElementExists(By.CssSelector("[name=Username]"));
        InputSendkeys(By.CssSelector("[name=Username]"), userId);
        InputSendkeys(By.CssSelector("[name=Password]"), password);
        WaitElementExists(By.CssSelector("button"));
        ElementClick(By.CssSelector("button"));

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));
    }

    public void WaitElementExists(By by)
    {
        _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(by);
                return element != null;
            }
            catch (NoSuchElementException)
            {
                Thread.Sleep(500);
                return false;
            }
        });
    }

    public void ElementClick(By by)
    {
        WaitElementExists(by);

        var element = _driver.FindElement(by);

        if (element.Displayed)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(by));
            _actions.MoveToElement(element).Click().Perform();
        }
        else
        {
            _actions.MoveToElement(element).Perform();
            _wait.Until(d => element.Displayed);

            element = _wait.Until(ExpectedConditions.ElementToBeClickable(by));
            _actions.Click(element).Perform();
        }
    }

    public void NavigateWait(string url, By by)
    {
        _driver.Navigate().GoToUrl($@"{BaseUrl}{url}");
        WaitElementExists(by);
    }

    public void InputSendkeys(By by, string text)
    {
        WaitElementExists(by);

        _wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(by);
                element.SendKeys(text);
                Thread.Sleep(500);
                return true;
            }
            catch (StaleElementReferenceException)
            {
                Thread.Sleep(500);
                return false;
            }
        });
    }

    public void UploadFilesAndCheck(string[] fileNames, string cssSelectorInput)
    {
        WaitElementExists(By.CssSelector(cssSelectorInput));

        var filePaths = fileNames.Select(fileName =>Path.Combine(Directory.GetCurrentDirectory(), "Assets", fileName)).ToArray();
        var currentFileNames = new List<string>();

        foreach (var filePath in filePaths)
        {
            InputSendkeys(By.CssSelector(cssSelectorInput), filePath);
            currentFileNames.Add(Path.GetFileName(filePath));
            CheckFileName(string.Join(",", currentFileNames));

            var fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension == ".png")
            {
                var durationInput = _wait.Until(d => d.FindElement(By.XPath("//storm-input-group[@label='播放秒數']//input")));
                durationInput.SendKeys("10");
            }
        }
        Thread.Sleep(1000);

        var uploadButton = By.XPath("//button[text()='上傳']");
        WaitElementExists(uploadButton);
        ElementClick(uploadButton);
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


    public IWebElement WaitShadowElement(string cssSelector, string expectedText, bool isEditTable = false)
    {
        return _wait.Until(_ =>
        {
            IWebElement resultElement = null!;
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

            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var columns = tr.FindElements(By.CssSelector("td"));
                return columns.Any(td => td.FindElement(By.CssSelector(cssSelector)).Text == expectedText);
            });

            if (selectedRow != null)
            {
                resultElement = selectedRow.FindElement(By.CssSelector(cssSelector));
            }

            return resultElement;
        });
    }
    public void MoveAndCheck(string cssSelector)
    {
        var element = _driver.FindElement(By.CssSelector(cssSelector));
        _actions.MoveToElement(element).Perform();
        That(element.Selected);
    }
    public void CheckElementText(string cssSelector, string expectedText)
    {
        var element = _driver.FindElement(By.CssSelector(cssSelector));
        _actions.MoveToElement(element).Perform();
        That(element.Text, Is.EqualTo(expectedText));
    }
    public void ClickRow(string caseNo)
    {
        _wait.Until(driver =>
        {
            try
            {
                var stormTable = driver.FindElement(By.CssSelector("storm-table"));
                return stormTable != null;
            }
            catch (NoSuchElementException)
            {
                Thread.Sleep(500);
                return false;
            }
        });

        var stormTable = _driver.FindElement(By.CssSelector("storm-table"));
        var stormTableInput = stormTable.GetShadowRoot().FindElements(By.CssSelector("input[placeholder='請輸入關鍵字']")).FirstOrDefault();

        if (stormTableInput != null)
        {
            stormTableInput.Clear();
            stormTableInput.SendKeys(caseNo + Keys.Enter);
        }

        _wait.Until(driver =>
        {
            var stormTable = driver.FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody > tr"));
            return rows.Count == 1;
        });

        var applyCaseNo = _wait.Until(driver =>
        {
            var stormTable = driver.FindElement(By.CssSelector("storm-table"));
            var applyCaseNoElements = stormTable.GetShadowRoot().FindElement(By.CssSelector("td[data-field='__radio_1']"));
            return applyCaseNoElements;
        });

        _actions.MoveToElement(applyCaseNo).Click().Perform();
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