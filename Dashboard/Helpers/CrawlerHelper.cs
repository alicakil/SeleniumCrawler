using Microsoft.Win32;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;
using Dashboard.Models;
using Dashboard.Repository;

namespace Dashboard.Helpers
{
    internal static class CrawlerHelper
    {
        private static int Sleep;

        public class Selenium
        {
            public UnitOfWork u { get; set; }
            public ChromeDriver Driver { get; set; }
        }

        public static Selenium InitializeSelenium()
        {
            try
            {
                // Initialize --- (skipping DI)
                Print("\nSelenium is Initializing...");
                Context c = new Context(); 
                CurrentUser currentUser = new CurrentUser();
                currentUser.Id = 4;
                currentUser.Name = "Crawler";
                // --------------

                var defaults = CrawlerHelper.GetDefaults();
                Sleep = defaults.Sleep;
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true; // hide the console, remove this line if u want to see it

                var options = new ChromeOptions();
                options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-infobars");

                if (!defaults.ShowBrowser)
                    options.AddArgument("--headless"); // Set Chrome options to run in headless mode

                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
                options.AddUserProfilePreference("useAutomationExtension", false);

                ChromeDriver driver = new ChromeDriver(driverService, options);
                driver.Manage().Window.Maximize();

                Thread.Sleep(Sleep);
                Print("Selenium Initialized");

                return new Selenium
                {
                    u = new UnitOfWork(c, currentUser),
                    Driver = driver
                };
            }
            catch (Exception ex)
            {
                Print("Initialization Failed! " + ex.Message);
                return new Selenium { };
            }
        }

        public static void Print(string msg)
        {
            Console.WriteLine(msg);
            Wait(2);
        }

        public static void Wait(int t)
        {
            Thread.Sleep(t * Sleep);
        }

        public static void Secs(int t)
        {
            for (int i = 0; i < t; i++)
            {
                Console.WriteLine($"{i}/{t}sn");
                Thread.Sleep(1000);
            }
        }

        public static bool IsElementExists(ChromeDriver driver, By by)
        {
            try
            {
                IWebElement element = driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static CrawlDefaults GetDefaults()
        {
            // check if the configuration file exists
            if (!File.Exists(AppContext.BaseDirectory + @"/crawlersettings.json"))
            {
                // create the configuration file with default values
                var defaultConfig = new CrawlDefaults
                {
                    Sleep = 300,
                    ShowBrowser = true,
                    StartWithWindows = false
                };

                File.WriteAllText(AppContext.BaseDirectory + @"/appsettings.json", JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
            }

            var defaults = JsonConvert.DeserializeObject<CrawlDefaults>(File.ReadAllText("crawlersettings.json"));
            SetStartup(defaults.StartWithWindows);
            return defaults;
        }

        public static void SetStartup(bool enableStartup)
        {
            var appName = "Crawler";
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (enableStartup)
            {
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, exePath);
                }
            }
            else
            {
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                }
            }
        }






    }
}
