using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.IO;
using System;
using OpenQA.Selenium.Support.UI;
using AkCore.E2ETests.Models;
using AkCore.E2ETests.Enums;
using AkCore.E2ETests.Extensions;

namespace AkCore.E2ETests
{
    public class E2ETestsBase
    {
        private readonly string path = Directory.GetCurrentDirectory();
        protected static string appUrl;
        protected static WebLogin login;
        protected static WebDriverWait Wait { get; set; }
        protected static IWebDriver Driver;

        [ClassInitialize]
        internal static void BaseTestClassInitialize(TestContext context)
        {
            InitializeSetUp(context);
        }

        private static void InitializeSetUp(TestContext context)
        {
            appUrl = context.Properties["webAppUrl"].ToString();
            login = new WebLogin(){
                UserName = context.Properties["userName"].ToString(),
                Password = context.Properties["password"].ToString()
            };
        }

        [ClassCleanup]
        internal static void BaseDispose()
        {
            //cleanup
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Driver.Quit();
        }

        protected void SetDrivers(Browser browser)
        {
            Driver = browser.CreateLocalDriver();
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
        }
    }
}