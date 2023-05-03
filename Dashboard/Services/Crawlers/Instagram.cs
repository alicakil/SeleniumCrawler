using Dashboard.Helpers;
using Dashboard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using Tesseract;
using static Dashboard.Helpers.CrawlerHelper;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Dashboard.Services.Crawlers
{

    /// <summary>
    /// Instagram crawler, Note that this class might not for for the future 
    /// due to changes on the target web site (instagram) front-end code. Must be kept up-to-date if broken.
    /// </summary>
    public class Instagram : ICrawler
    {
        private Selenium selenium;


        public Instagram()
        {
            selenium = CrawlerHelper.InitializeSelenium();
        }

        ~Instagram()
        {
            Print("Disposing Instagram Crawling\n");

            if (selenium is not null)
                selenium.Driver.Quit();
        }

        public void Login()
        {
            Print("Instagram Login Starting");
            selenium.Driver.Navigate().GoToUrl(AppConstants.Instagram.RootUrl);
            Wait(7);
            selenium.Driver.FindElement(By.XPath("//input[@type='text']")).SendKeys(AppConstants.Instagram.username);
            selenium.Driver.FindElement(By.XPath("//input[@type='password']")).SendKeys(AppConstants.Instagram.password);
            Wait(1);
            selenium.Driver.FindElement(By.XPath("//div[@class='_ab8w  _ab94 _ab99 _ab9f _ab9m _ab9p  _abak _abb8 _abbq _abb- _abcm']")).Click();
            Wait(17);
            Print("Instagram Login Completed");
        }

     
        public void GetUserStory(string account, string province, string country)
        {
            //try
            //{
            Print("\n\nNavigating to Events Page for " + account + ", " + province + ", " + country);
            var instagramUrl = AppConstants.Instagram.RootUrl + "stories/" + account + "/";
            selenium.Driver.Navigate().GoToUrl(instagramUrl);
            Wait(9);


            var storyCount = 10;

            for (int i = 1; i <= storyCount; i++)
            {
                Print($"\nStory {account} {i}/{storyCount}");

                var nextButtonPath = By.XPath("//button[@class='_acan _acap _acau _acav _aj1-']");

                if (IsElementExists(nextButtonPath))
                {
                    selenium.Driver.FindElement(nextButtonPath).Click();
                    Print("View Story Clicked");
                }
                Wait(3);


                var imgPath = By.XPath("//img[@class='_aa63']");
                if (!IsElementExists(imgPath))
                {
                    Print("IMAGE NOT FOUND, Skipping");
                    ClickNext();
                    continue;
                }

                var imgElement = selenium.Driver.FindElement(imgPath);
                var imgUrl = imgElement.GetAttribute("src");
                Print("Image Captured......");


                // Save Img to Local
                S3 s3 = null;

                try
                {
                    s3 = new S3(AppContext.BaseDirectory);
                }
                catch (Exception ex)
                {
                    Print("S3 : " + ex.Message + " BaseDir : " + AppContext.BaseDirectory);
                }


                string imgBase64Str;
                if (!s3.GetBase64FromUrl(imgUrl, out imgBase64Str, out double aspectRatio)) continue;

                string fileName = s3.GetUniqueName("CoverPhoto") + ".jpg";
                string S3Path = S3.Folder_Dir_StoryImage + fileName;
                string LocalPath = AppContext.BaseDirectory + @"/" + S3Path.Replace("/", @"\");

                s3.SaveBase64File(LocalPath, imgBase64Str);


                // OCR Process
                var ocrResult = Ocr(LocalPath);
                if (!ocrResult.success)
                {
                    Print("ORC error! " + ocrResult.ErrMsg);
                    ClickNext();
                    continue;
                }

                // Image OCR Resolving...
                if (ocrResult.Confidence < 0.5)
                {
                    Print("Insufficiant Confidence! : " + ocrResult.Confidence);
                    ClickNext();
                    continue;
                }

                var keywords = selenium.u.storyRepository.GetKeyWords();
                var matches = keywords.Where(keyword => ocrResult.ImageText.ToLower().Contains(keyword.Word));
                var macthesList = string.Join(", ", matches.Select(x => x).ToList());

                if (matches.Count() < 1)
                {

                    Print("\n\n Insufficiant Match! Current Matches:" + macthesList);
                    Print(ocrResult.ImageText);
                    ClickNext();
                    continue;
                }
                Print("\n\n Matches:" + macthesList + "\n");

                // Check if already exists
                if (selenium.u.storyRepository.Any(x => x.Title == account + " " + macthesList))
                {
                    Console.Write(" Already exists.");
                    ClickNext();
                    continue;
                }





                // Save Sotry Img to S3 Storage
                Print("S3 Image Uploading");
                s3.UploadFile(LocalPath, S3Path);
                Print("S3 Image Uploaded");
                Wait(1);

                var macthingKeywords = new List<string>();

                foreach (var keyword in keywords)
                {
                    if (ocrResult.ImageText.ToLower().Contains(keyword.Word.ToLower()))
                    {
                        macthingKeywords.Add(keyword.Word);
                    }
                }

                Story e = new Story()
                {
                    Title = LimitLength(ocrResult.ImageText, 80),
                    Description = ocrResult.ImageText,
                    CoverPhotoURL = fileName,
                    CoverPhotoAspectRatio = aspectRatio,
                    Keywords = macthingKeywords.ToArray(),
                    InstagramUrl = instagramUrl,
                };

                selenium.u.storyRepository.Add(e);
                selenium.u.Complete();
                Print("Event Crawled !");
                ClickNext();
            }
        }

        string LimitLength(string input, int limitAt)
        {
            if (input.Length > limitAt)
            {
                return input.Substring(0, limitAt);
            }
            else
            {
                return input;
            }
        }

        void ClickNext()
        {
            var nextXPath = By.XPath("//div[@class='_9zm2']");

            if (IsElementExists(nextXPath))
            {
                selenium.Driver.FindElement(nextXPath).Click();
                Print($"Next Clicked");
                Wait(1);
            }
        }

        public void CrawlWebSite()
        {
            try
            {
                Login();

                GetUserStory("account-name-1", "Antalya", "TR");
                GetUserStory("account-name-2", "Paris", "FR");
                GetUserStory("account-name-3", "Madrid", "ES");

                Print("\n\n\n   - Crawling Completed! - \n\n");
            }
            catch (Exception ex)
            {
                Print("Error : CrawlWebSite : " + ex.Message);
            }

            selenium.Driver.Quit();
        }

        private void CloseTabGoToFirstTab()
        {
            // close the tab
            var lastTab = selenium.Driver.WindowHandles.Last();
            selenium.Driver.SwitchTo().Window(lastTab).Close();
            Wait(1);

            // return to first tab
            var firstTab = selenium.Driver.WindowHandles.First();

            try
            {
                selenium.Driver.SwitchTo().Window(firstTab);
            }
            catch (Exception)
            { }

            Wait(1);
            Print("Tab is closed");
        }

        public bool IsElementExists(By by)
        {
            try
            {
                IWebElement element = selenium.Driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        class OcrResult
        {
            public string ImageText { get; set; }
            public float Confidence { get; set; }
            public bool success { get; set; }
            public string ErrMsg { get; set; }
        }

        OcrResult Ocr(string ImgPath)
        {
            try
            {
                //Following Path to be updated/tested
              
                var testdataPath = AppContext.BaseDirectory + @"\tessdata";

                using (var engine = new TesseractEngine(testdataPath, "eng", EngineMode.Default))
                {
                    using (var image = Pix.LoadFromFile(ImgPath))
                    {
                        using (var page = engine.Process(image))
                        {
                            return new OcrResult
                            {
                                ImageText = page.GetText(),
                                Confidence = page.GetMeanConfidence(),
                                success = true
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new OcrResult
                {
                    success = true,
                    ErrMsg = ex.Message
                };
            }

        }
    }
}
