using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.IO;
using System.Threading;
using Polly;
using System.Configuration;
using System.Net;
using System.IO.Compression;
namespace VRChatAvatar
{
    class Program
    {
        public static IWebDriver driver;
        static void Main(string[] args)
        {


            string username = ConfigurationManager.AppSettings.Get("Username");
            string password = ConfigurationManager.AppSettings.Get("Password");
            bool bypassDownload = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("BypassDownload"));
            string myPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!(bypassDownload))
            {

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C reg query \"HKEY_CURRENT_USER\\Software\\Google\\Chrome\\BLBeacon\" /v version";
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;

                process.StartInfo.RedirectStandardOutput = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string version = output.Split(new string[] { "REG_SZ" }, StringSplitOptions.None).GetValue(1).ToString();
                version = version.TrimStart();
                version = version.TrimEnd();
                version = version.Split('.').GetValue(0).ToString();
                int ChromeVersion = Convert.ToInt32(version);
                WebClient client = new WebClient();
                bool downloadFound = false;


                try
                {
                    switch (ChromeVersion)
                    {
                        case 103:
                            client.DownloadFile($"https://chromedriver.storage.googleapis.com/103.0.5060.134/chromedriver_win32.zip", myPath + "/webdriver.zip");
                            downloadFound = true;
                            break;
                        case 104:
                            client.DownloadFile($"https://chromedriver.storage.googleapis.com/104.0.5112.79/chromedriver_win32.zip", myPath + "/webdriver.zip");
                            downloadFound = true;
                            break;
                        case 105:
                            client.DownloadFile($"https://chromedriver.storage.googleapis.com/105.0.5195.52/chromedriver_win32.zip", myPath + "/webdriver.zip");
                            downloadFound = true;
                            break;
                        case 106:
                            client.DownloadFile($"https://chromedriver.storage.googleapis.com/106.0.5249.21/chromedriver_win32.zip", myPath + "/webdriver.zip");
                            downloadFound = true;
                            break;
                    }

                    if (!(downloadFound))
                    {
                        Console.WriteLine("Unable to find a driver for your chrome version! Talk to Fold.");
                        Console.ReadLine();
                    }

                    ZipFile.ExtractToDirectory(myPath + "/webdriver.zip", myPath);
                }
                catch
                {
                }

            }

            string driverLocation = myPath;
            Console.WriteLine("Loading driver from: " + driverLocation);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--mute-audio");
            options.AddArgument("--log-level=3");
            driver = new ChromeDriver(driverLocation, options);
            driver.Manage().Window.Maximize();



            string loginURL = "https://ripper.store/login";
            string watchAdsURL = "https://ripper.store/clientarea?section=credits_watch_ads";
            driver.Url = loginURL;


            //ChromeOptions options = new ChromeOptions();
            //options.DebuggerAddress = "127.0.0.1:55359";
            //options.AddArgument("--mute-audio");
            //var policy = Policy
            //    .Handle<InvalidOperationException>()
            //    .WaitAndRetry(10, t => TimeSpan.FromSeconds(1));

            //policy.Execute(() =>
            //{
            //    driver = new ChromeDriver(driverLocation, options);
            //}
            //);



            IWebElement usernameElement = driver.FindElement(By.Id("username"));
            IWebElement pwdElement = driver.FindElement(By.Id("password_1"));

            usernameElement.SendKeys(username);
            Thread.Sleep(300);
            pwdElement.SendKeys(password);
            Thread.Sleep(2000);
            pwdElement.SendKeys(Keys.Enter);
            Thread.Sleep(5000);

            driver.Url = watchAdsURL;
            Thread.Sleep(5000);
            while (true)
            {
                IWebElement watchAdButton = driver.FindElement(By.Id("showRewardAdButton"));

                if (watchAdButton.Text == "Start Video")
                {
                    try
                    {
                        watchAdButton.Click();
                        Console.WriteLine(DateTime.Now + " - Successfully watched ad.");
                    }
                    catch
                    {
                        Console.WriteLine(DateTime.Now + " - Failed to click watch element.");
                        driver.Navigate().Refresh();
                    }
                }
              
                

                int waitTime = 1000;
                Thread.Sleep(waitTime);
            }
           
        }

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }
        }
    }
}
