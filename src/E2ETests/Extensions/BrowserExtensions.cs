using AcademicWork.JobWeb.E2ETests.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.IO;
namespace AcademicWork.JobWeb.E2ETests.Extensions
{
    public static class BrowserExtensions
    {
        public static IWebDriver CreateLocalDriver(this Browser browser)
        {
            var path = Directory.GetCurrentDirectory();

            switch (browser)
            {
                case Browser.DesktopChrome:
                    return new ChromeDriver(Environment.GetEnvironmentVariable("ChromeWebDriver") ?? path);
                case Browser.DesktopFirefox:
                    return new FirefoxDriver(Environment.GetEnvironmentVariable("GeckoWebDriver") ?? path);
                case Browser.DesktopInternetExploder:
                    return new InternetExplorerDriver(Environment.GetEnvironmentVariable("IEWebDriver") ?? path);
                default:
                    return new ChromeDriver(Environment.GetEnvironmentVariable("ChromeWebDriver") ?? path);
            }
        }
    }
}