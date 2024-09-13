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
using Newtonsoft.Json.Linq;
using static NUnit.Framework.Assert;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using AngleSharp.Dom;
using NUnit.Framework.Constraints;
using OpenQA.Selenium.DevTools.V113.Preload;
using System.Xml.Linq;
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

    public static async Task Login(IWebDriver webDriver, string userId, string password)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        var action = new Actions(webDriver);
        int retryCount = 3;
        bool isLoginSuccessful = false;

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                webDriver.Navigate().GoToUrl(LoginUrl);

                //Console.WriteLine($"::group::Login---------{LoginUrl}---------");
                //Console.WriteLine($"---------{LoginUrl}---------");
                //Console.WriteLine(webDriver.PageSource);
                //Console.WriteLine("::endgroup::");
                wait.Until(ExpectedConditions.UrlContains("account"));

                //Console.WriteLine($"::group::Login---------{webDriver.Url}---------");
                //Console.WriteLine(webDriver.PageSource);
                //Console.WriteLine("::endgroup::");

                wait.Until(driver =>
                {
                    var usernameElement = driver.FindElement(By.CssSelector("[name=Username]"));
                    return usernameElement != null;
                });

                var usernameElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Username]")));
                var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[name=Password]")));

                usernameElement.SendKeys(userId);
                passwordElement.SendKeys(password);

                var button = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button")));
                action.MoveToElement(button).Click().Perform();

                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("storm-sidenav")));

                isLoginSuccessful = true;
                break;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("未找到登入元素，嘗試重試...");
                webDriver.Navigate().Refresh();
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤訊息：{ex.Message}");
                break;
            }
            if (!isLoginSuccessful)
            {
                throw new Exception("登入失敗。");
            }
        }
    }
    public static async Task WaitForElement(IWebDriver driver, By by, int timeoutSeconds)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(by);
                return element != null;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }

    public static async Task WaitAndClick(IWebDriver driver, By by, int timeoutSeconds) 
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        Actions actions = new Actions(driver);

        await WaitForElement(driver, by, timeoutSeconds);

        var element = driver.FindElement(by);

        if (element.Displayed)
        {
            actions.MoveToElement(element).Click().Perform();
        }
        else
        {
            Thread.Sleep(1000);
            actions.MoveToElement(element).Perform();
            wait.Until(d => element.Displayed);
            element = wait.Until(ExpectedConditions.ElementToBeClickable(by));
            actions.Click().Perform();
        }
    }

    public static async Task EnterText(IWebDriver driver, By by, string text)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        wait.Until( d =>
        {
            try
            {
                var element = d.FindElement(by);
                element.SendKeys(text);
                return true;
            }
            catch (StaleElementReferenceException)
            {
                Thread.Sleep(500);
                return false;
            }
        });
    }

    public static async Task NavigateAndWaitForElement(IWebDriver driver, string url, By by, int timeoutSeconds)
    {
        driver.Navigate().GoToUrl($@"{BaseUrl}{url}");
        await WaitForElement(driver, by, timeoutSeconds);
    }

    public static async Task UploadFileAndCheck(IWebDriver driver, string fileName, string cssSelectorInput)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var actions = new Actions(driver);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", fileName);

        wait.Until(d =>
        {
            try
            {
                var hiddenInput = d.FindElement(By.CssSelector(cssSelectorInput));
                return true;
            }
            catch (NoSuchElementException)
            {
                Thread.Sleep(500);
                return false;
            }
        });

        var hiddenInput = wait.Until(d => d.FindElement(By.CssSelector(cssSelectorInput)));
        hiddenInput.SendKeys(filePath);

        var fileExtension = Path.GetExtension(filePath).ToLower();
        if (fileExtension == ".png")
        {
            var durationInput = wait.Until(d => d.FindElement(By.XPath("//storm-input-group[@label='播放秒數']//input")));
            durationInput.SendKeys("10");
        }

        var fileNameInput = wait.Until(d => d.FindElement(By.XPath("//storm-input-group[@label='名稱']//input")));
        That(fileNameInput.GetAttribute("value"), Is.EqualTo(fileName));

        var uploadButton = wait.Until(d => d.FindElement(By.XPath("//button[text()='上傳']")));
        actions.MoveToElement(uploadButton).Click().Perform();
    }

    public static IWebElement WaitStormEditTableWithText(IWebDriver driver, string cssSelector, string expectedText)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        return wait.Until(_ =>
        {
            IWebElement resultElement = null!;

            var stormEditTable = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-edit-table")));
            var stormTable = stormEditTable.GetShadowRoot().FindElement(By.CssSelector("storm-table"));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var textElement = tr.FindElement(By.CssSelector("td[data-field='name'] span span"));
                return textElement.Text == expectedText;
            });

            if (selectedRow != null)
            {
                resultElement = selectedRow.FindElement(By.CssSelector(cssSelector));
            }

            return resultElement;
        });
    }

    public static IWebElement WaitStormTableWithText(IWebDriver driver, string cssSelector, string expectedText)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        return wait.Until(_ =>
        {
            IWebElement resultElement = null!;

            var stormTable = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("storm-table")));
            var rows = stormTable.GetShadowRoot().FindElements(By.CssSelector("tbody tr"));
            var selectedRow = rows.FirstOrDefault(tr =>
            {
                var element = tr.FindElement(By.CssSelector(cssSelector));
                return element.Text == expectedText;
            });

            if (selectedRow != null)
            {
                resultElement = selectedRow.FindElement(By.CssSelector(cssSelector));
            }

            return resultElement;
        });
    }

    public static async Task ClickButton(IWebDriver driver, By buttonCssSelector)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var actions = new Actions(driver);

        var button = wait.Until(ExpectedConditions.ElementToBeClickable(buttonCssSelector));
        actions.MoveToElement(button).Click().Perform();
    }

    public static async Task WaitElementDisappear(IWebDriver driver, By elementCssSelector)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(elementCssSelector));
    }

    public static async Task NavigateAndWait(IWebDriver driver, string pageUrl)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        int retryCount = 3;
        bool isNavigatedSuccessfully = false;

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                driver.Navigate().GoToUrl($@"{BaseUrl}{pageUrl}");

                wait.Until(driver =>
                {
                    var stormSideNav = driver.FindElement(By.CssSelector("storm-sidenav"));
                    return stormSideNav != null;
                });

                isNavigatedSuccessfully = true;
                break;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("未找到 storm-sidenav 元素，嘗試重試...");
                driver.Navigate().Refresh();
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤訊息：{ex.Message}");
                break;
            }
        }

        if (!isNavigatedSuccessfully)
        {
            throw new Exception("導航到指定頁面失敗。");
        }
    }

    public static async Task ClickRow(IWebDriver webDriver, string caseNo)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        var action = new Actions(webDriver);

        wait.Until(driver =>
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

        var stormTable = webDriver.FindElement(By.CssSelector("storm-table"));
        var stormTableInput = stormTable.GetShadowRoot().FindElements(By.CssSelector("input[placeholder='請輸入關鍵字']")).FirstOrDefault();

        if (stormTableInput != null)
        {
            stormTableInput.Clear();
            stormTableInput.SendKeys(caseNo + Keys.Enter);
        }

        var applyCaseNo = wait.Until(driver =>
        {
            var stormTable = driver.FindElement(By.CssSelector("storm-table"));
            var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
            return applyCaseNoElements.FirstOrDefault(element => element.Text == caseNo);
        });

        action.MoveToElement(applyCaseNo).Click().Perform();
    }

    public static Task ChangeUser(IWebDriver webDriver, string userName)
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

        return Task.CompletedTask;
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

    public static void ClickElementInWindow(IWebDriver driver, string xpath, int windowIndex)
    {
        var _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var _actions = new Actions(driver);

        driver.SwitchTo().Window(driver.WindowHandles[windowIndex]);
        driver.SwitchTo().DefaultContent();

        var element = _wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        _actions.MoveToElement(element).Click().Perform();
    }
    public static void HoverOverElementInWindow(IWebDriver driver, string xpath, int windowIndex)
    {
        var _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var actions = new Actions(driver);

        driver.SwitchTo().Window(driver.WindowHandles[windowIndex]);
        driver.SwitchTo().DefaultContent();

        var element = _wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        actions.MoveToElement(element).Perform();
    }

    

    public static void UploadFile(IWebDriver webDriver, string filePath, string css)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var lastHiddenInput = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(css)));
        lastHiddenInput.SendKeys(filePath);
    }

    public static bool DownloadFileAndVerify(IWebDriver webDriver, string fileName, string xpath)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
        var actions = new Actions(webDriver);
        var downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        var filePath = Path.Combine(downloadDirectory, fileName);

        Directory.CreateDirectory(downloadDirectory);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var downloadButton = webDriver.FindElement(By.XPath(xpath));
        actions.MoveToElement(downloadButton).Click().Perform();

        wait.Until(driver =>
        {
            foreach (var file in Directory.GetFiles(downloadDirectory))
            {
                Console.WriteLine($"-----filename: {file}-----");
            }

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