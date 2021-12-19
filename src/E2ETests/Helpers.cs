using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AkCore.E2ETests
{
    public static class Helpers
    {
        public static IWebElement FindVisible(this ReadOnlyCollection<IWebElement> elements)
        {
            foreach (var element in elements)
            {
                if (element.Displayed)
                {
                    return element;
                }
            }

            return null;
        }

        public static IWebElement WaitAndGetElement(this IWebDriver driver, Func<string, By> by, string elementName)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementIsVisible(by(elementName)));

            return driver.FindElement(by(elementName));
        }

        public static bool WaitForValue(this IWebDriver driver, Func<string, By> by, string elementName, string value)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            return wait.Until<bool>(d => d.FindElement(by(elementName)).GetAttribute("value") == value);
        }

        public static bool WaitForValue(this IWebElement element, IWebDriver driver, string value)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            return wait.Until<bool>(d => element.GetAttribute("value") == value);
        }

        public static bool WaitForValueNotNull(this IWebElement element, IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            return wait.Until(d => element.GetAttribute("value") != "");
        }

        public static IWebElement WaitFindId(this IWebDriver driver, string id, int timeOutInMinutes = 3)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(timeOutInMinutes));
            return wait.Until(d => d.FindElements(By.Id(id)).FirstOrDefault());
        }

        public static IWebElement WaitFindSelector(this IWebDriver driver, string selector, int index = 0,
            int timeOutInMinutes = 3)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(timeOutInMinutes));
            return wait.Until(d => d.FindElements(By.CssSelector(selector)).ElementAtOrDefault(index));
        }

        public static bool IsAlertPresent(IWebDriver driver)
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        public static RemoteWebDriver SetBrowser(string browser)
        {
            var path = Directory.GetCurrentDirectory();

            switch (browser)
            {
                case "Chrome":
                    return new ChromeDriver(Environment.GetEnvironmentVariable("ChromeWebDriver") ?? path);
                case "Firefox":
                    return new FirefoxDriver(Environment.GetEnvironmentVariable("GeckoWebDriver") ?? path);
                case "IE":
                    return new InternetExplorerDriver(Environment.GetEnvironmentVariable("IEWebDriver") ?? path);
                default:
                    return new ChromeDriver(Environment.GetEnvironmentVariable("ChromeWebDriver") ?? path);
            }
        }
    }
}