﻿using OpenQA.Selenium;
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

    public static void ScrollToElement(IWebDriver driver, By by)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        IWebElement element = wait.Until(drv =>
        {
            var foundElement = drv.FindElements(by).FirstOrDefault();
            return foundElement!;
        });
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        wait.Until(ExpectedConditions.ElementIsVisible(by));
    }

    public static async Task ClickRow(IWebDriver webDriver, string caseNo)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        var action = new Actions(webDriver);
        int retryCount = 3;
        bool isElementFound = false;

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                wait.Until(driver =>
                {
                    var stormTable = driver.FindElement(By.CssSelector("storm-table"));
                    if (driver.PageSource.Contains("Page Not Found") || string.IsNullOrWhiteSpace(driver.PageSource))
                    {
                        throw new Exception("頁面為空白，系統可能掛掉。");
                    }
                    return stormTable != null;
                });

                var stormTableElement = wait.Until(driver =>
                {
                    var stormTable = driver.FindElement(By.CssSelector("storm-table"));
                    var input = stormTable.GetShadowRoot().FindElement(By.CssSelector("input[placeholder='請輸入關鍵字']"));
                    return input;
                });

                stormTableElement.Clear();
                stormTableElement.SendKeys(caseNo + Keys.Enter);

                var applyCaseNo = wait.Until(driver =>
                {
                    var stormTable = driver.FindElement(By.CssSelector("storm-table"));
                    var applyCaseNoElements = stormTable.GetShadowRoot().FindElements(By.CssSelector("td[data-field='applyCaseNo'] span"));
                    return applyCaseNoElements.FirstOrDefault(element => element.Text == caseNo);
                });

                if (applyCaseNo != null)
                {
                    action.MoveToElement(applyCaseNo).Click().Perform();
                    isElementFound = true;
                    break;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("未找到元素，嘗試刷新...");
                webDriver.Navigate().Refresh();
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤訊息：{ex.Message}");
                break;
            }
        }

        if (!isElementFound)
        {
            throw new Exception("目標元素未找到。");
        }
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
        var _actions = new Actions(driver);

        driver.SwitchTo().Window(driver.WindowHandles[windowIndex]);
        driver.SwitchTo().DefaultContent();

        var element = _wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        _actions.MoveToElement(element).Perform();
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

    public static IWebElement FindAndMoveElement(IWebDriver webDriver, string xpath)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
        var action = new Actions(webDriver);
        var element = wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));

        action.MoveToElement(element).Perform();
        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));

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