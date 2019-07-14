using OpenQA.Selenium;

namespace AkCore.E2ETests
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void SetUsername(string username)
        {
            _driver.WaitAndGetElement(By.Id, "username").SendKeys(username);
            _driver.WaitForValue(By.Id, "username", username);
        }

        public void SetPassword(string password)
        {
            _driver.WaitAndGetElement(By.Id, "password").SendKeys(password);
            _driver.WaitForValue(By.Id, "password", password);
        }

        public void ClickLoginButton()
        {
            var loginform = _driver.WaitAndGetElement(By.Id, "loginForm");
            loginform.FindElement(By.ClassName("submit-login")).Click();
        }

        public void Login(string username, string password)
        {
            var loginButton = _driver.WaitFindSelector(".account .login");

            loginButton.Click();
            SetUsername(username);
            SetPassword(password);
            ClickLoginButton();
        }
    }
}
