using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcademicWork.JobWeb.E2ETests.Enums;
using OpenQA.Selenium;

namespace AkCore.E2ETests
{
    [TestClass]
    public class LoginTests : E2ETestsBase
    {
        private static TestContext _context;

        [ClassInitialize]
        public static void TestClassInitialize(TestContext context)
        {
            _context = context;
        }

        [DataTestMethod]
        [DataRow(Browser.DesktopChrome)]
        public void Login_in_with_correct_username(
            Browser browser)
        {
            BaseTestClassInitialize(_context);
            SetDrivers(browser);

            Driver.Navigate().GoToUrl(appUrl);

            var page = new LoginPage(Driver);
            page.Login(login.UserName, login.Password);
            var profileLink = Driver.WaitFindSelector(".profile-link");
            var a = profileLink.Text;
            Assert.IsNotNull(profileLink);
            Assert.AreEqual(profileLink.Text, login.UserName);
        }
    }
}