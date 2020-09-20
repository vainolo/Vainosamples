using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Reflection;

namespace DynamicWebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            Scrape();
            Console.ReadLine();
        }

        public static void Scrape()
        {
            ChromeOptions options = new ChromeOptions();
            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl($"file://{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/page.html");
                driver.FindElement(By.Id("heading1")).Click();
                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Id("heading2")));
                Console.WriteLine(firstResult.GetAttribute("textContent"));
            }
        }
    }
}
