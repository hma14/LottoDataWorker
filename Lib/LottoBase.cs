using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumLottoDataApp.BusinessModels;
using SeleniumLottoDataApp.Dto;
using SeleniumLottoDataApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static SeleniumLottoDataApp.BusinessModels.Constants;

namespace SeleniumLottoDataApp.Lib
{

    public class LottoBase
    {
        //public RemoteWebDriver Driver { get; set; }
        public ChromeDriver Driver { get; set; }
        public LottoDbContext db { get; set; }

        public LottoBase(LottoDbContext _db)
        {
            db = _db;

            //PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            //service.IgnoreSslErrors = true;
            //service.LoadImages = false;
            //service.ProxyType = "none";
            //service.SuppressInitialDiagnosticInformation = true;
            //service.AddArgument("--webdriver-loglevel=NONE");

            //Driver = new PhantomJSDriver(service);
            //Driver.Manage().Window.Size = new Size(1024, 768);



            var chromeOptions = new ChromeOptions
            {
                BinaryLocation = @"C:\Program Files\google\chrome\Application\chrome.exe",
            };

            //chromeOptions.AddArguments(new List<string>()
            //{
            //    "--silent-launch",
            //    "--no-startup-window",
            //    "--no-sandbox",
            //    "--window-size=1920,1080",
            //    "--disable-gpu",
            //    "--disable-extensions",
            //    "--proxy-server='direct://'",
            //    "--proxy-bypass-list=*",
            //    "--start-maximized",
            //    "--headless",
            //});

            chromeOptions.AddArguments("--start-maximized");


           var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;    // This is to hidden the console.
            Driver = new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromMinutes(2));
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(2);
            //Driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);

        }

        private async Task<IEnumerable<LottoTypeDto>> GetLottoTypesAsync(int lottoName)
        {
            // Define the base URL and API endpoint
            string apiUrl = $"http://api.lottotry.com/api/lottotypes?lottoName={lottoName}";

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the API endpoint
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Ensure the response was successful (status code 200)
                    response.EnsureSuccessStatusCode();

                    // Read the response content as a string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into IEnumerable<LottoTypeDto>
                    var lottoTypes = JsonSerializer.Deserialize<IEnumerable<LottoTypeDto>>(responseBody);

                    return lottoTypes.ToList();
                }
                catch (HttpRequestException e)
                {
                    // Handle any errors that occur during the request
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
            }
        }


        internal async Task<int> CalculateProbability(LottoNames lottoName, int num)
        {
            // get range of this number in history
            var list = await GetLottoTypesAsync((int)lottoName);
            var sortedList = list.OrderByDescending(x => x.DrawDate);
            int probability = 0;


            List<NumberDto> numbers = new List<NumberDto>();
            foreach (var item in sortedList)
            {
                var n = item.Numbers.Where(x => x.Value == num).FirstOrDefault();
                if (n == null) continue;

                n.LottoName = item.LottoName;
                n.DrawNumber = item.DrawNumber;
                n.DrawDate = item.DrawDate;
                n.NumberRange = item.NumberRange;
                numbers.Add(n);
            }

            var hits = numbers.Where(x => x.IsHit == true).ToList();

            if ((hits[0].NumberofDrawsWhenHit > Constants.COLD_POINT ||
                hits[1].NumberofDrawsWhenHit > Constants.COLD_POINT) &&
                numbers[0].Distance < Constants.NORMAL_RANGE &&
                numbers[0].Distance >= Constants.HOT_POINT) probability++;

            if ((hits[0].NumberofDrawsWhenHit > Constants.COLD_POINT &&
                hits[1].NumberofDrawsWhenHit > Constants.NORMAL_RANGE) &&
                numbers[0].Distance < Constants.NORMAL_RANGE &&
                numbers[0].Distance >= Constants.HOT_POINT - 2) probability++;

            if (hits[0].NumberofDrawsWhenHit > Constants.COLD_POINT &&
                numbers[0].Distance > Constants.NORMAL_RANGE) probability++;

            if (hits[0].DrawNumber == hits[1].DrawNumber + 1 &&
                numbers[0].Distance < Constants.NORMAL_RANGE) probability++;

            if ((hits[0].DrawNumber == hits[1].DrawNumber + 2 ||
                hits[0].DrawNumber == hits[1].DrawNumber + 3) &&
                numbers[0].IsHit == false &&
                numbers[0].Distance < Constants.NORMAL_RANGE) probability++;

            if (hits[0].Distance >= Constants.COLD_POINT &&
                numbers[0].Distance > Constants.NORMAL_RANGE) probability++;
            
            if (hits[1].NumberofDrawsWhenHit > Constants.COLD_POINT &&
                hits[0].DrawNumber < hits[1].DrawNumber + Constants.NORMAL_RANGE &&
                numbers[0].Distance < Constants.NORMAL_RANGE &&
                numbers[0].IsHit == false) probability++;

            if (hits[0].NumberofDrawsWhenHit == hits[1].NumberofDrawsWhenHit &&
                numbers[0].IsHit == false)
            {
                if(numbers[0].Distance + 1 == hits[0].NumberofDrawsWhenHit ||
                   numbers[0].Distance == hits[0].NumberofDrawsWhenHit)
                {
                    probability++;
                }
                else if (IsInRange(numbers[0].Distance, hits[0].NumberofDrawsWhenHit, Constants.NORMAL_RANGE))
                {
                    probability++;
                }
            }

            if (hits[0].NumberofDrawsWhenHit <= Constants.HOT_POINT &&
                hits[1].NumberofDrawsWhenHit <= Constants.HOT_POINT &&
                hits[2].NumberofDrawsWhenHit <= Constants.HOT_POINT &&
                hits[3].NumberofDrawsWhenHit <= Constants.NORMAL_RANGE &&
                numbers[0].IsHit == false) probability++;

            if (numbers[0].Distance + 1 == hits[0].NumberofDrawsWhenHit)
                probability++;


            if (numbers[0].Distance >= Constants.COLD_POINT &&
                hits[0].DrawNumber == hits[1].DrawNumber + 1)
                probability++;





            return probability;
        }

        private bool IsInRange(int num, int start, int end)
        {
            return num >= start && num <= end;
        }


        public void CloseDriver()
        {
            Driver.Quit();
        }

        internal virtual void InsertDb() { }
        public virtual void InsertDb(int drawNumber, string drawDate, string[] numbers) { }
        internal virtual void InsertLottTypeTable() { }



        public IDictionary DicDate = new Dictionary<string, string>  {
                    { "January","1" },
                    { "February","2" },
                    { "March","3" },
                    { "April","4" },
                    { "May","5" },
                    { "June","6" },
                    { "July","7" },
                    { "August","8" },
                    { "September","9" },
                    { "October","10" },
                    { "November","11" },
                    { "December","12" }
        };

        public IDictionary DicDateShort = new Dictionary<string, string>  {
                    { "Jan","1" },
                    { "Feb","2" },
                    { "Mar","3" },
                    { "Apr","4" },
                    { "May","5" },
                    { "Jun","6" },
                    { "Jul","7" },
                    { "Aug","8" },
                    { "Sep","9" },
                    { "Oct","10" },
                    { "Nov","11" },
                    { "Dec","12" }
        };

        public IDictionary DicDateShort2 = new Dictionary<string, string> {
                    { "JAN","1" },
                    { "FEB","2" },
                    { "MAR","3" },
                    { "APR","4" },
                    { "MAY","5" },
                    { "JUN","6" },
                    { "JUL","7" },
                    { "AUG","8" },
                    { "SEP","9" },
                    { "OCT","10" },
                    { "NOV","11" },
                    { "DEC","12" }
        };

        public IDictionary DicDateShort3 = new Dictionary<string, string> {
                    { "JAN","1" },
                    { "FEB","2" },
                    { "MAR","3" },
                    { "APR","4" },
                    { "MAY","5" },
                    { "JUN","6" },
                    { "JUL","7" },
                    { "AUG","8" },
                    { "SEPT","9" },
                    { "OCT","10" },
                    { "NOV","11" },
                    { "DEC","12" }
        };

        public void InsertLottTypeTable(int lottoName)
        {
            var lotto = db.LottoMax.ToList().Last();
            var lastLottoType = db.LottoTypes
                .Where(x => x.LottoName == lottoName)
                .OrderByDescending(d => d.DrawNumber).First();

            if (lotto.DrawNumber == lastLottoType.DrawNumber) return;

            var prevDraw = db.Numbers.Where(x => x.LottoTypeId == lastLottoType.Id)
                                        .OrderBy(n => n.Value).ToArray();

            // Store to LottoType table
            LottoType lottoType = new ()
            {
                Id = Guid.NewGuid(),
                LottoName = lastLottoType.LottoName, //(int)LottoNames.LottoMax,
                DrawNumber = lotto.DrawNumber,
                DrawDate = lotto.DrawDate,
                NumberRange = lastLottoType.NumberRange, //(int)LottoNumberRange.LottoMax,
            };
            db.LottoTypes.Add(lottoType);

            //Store to Numbers table
            List<Number> numbers = [];
            var numberRange = lastLottoType.NumberRange;
            for (int i = 1; i <= numberRange; i++)
            {
                Number number = new ()
                {
                    Id = Guid.NewGuid(),
                    Value = i,
                    LottoTypeId = lottoType.Id,
                    Distance = (lotto.Number1 != i &&
                                lotto.Number2 != i &&
                                lotto.Number3 != i &&
                                lotto.Number4 != i &&
                                lotto.Number5 != i &&
                                lotto.Number6 != i &&
                                (lottoName != (int)LottoNames.LottoMax || lotto.Number7 != i) &&
                                lotto.Bonus != i) ? prevDraw[i - 1].Distance + 1 : 0,

                    IsHit = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                lotto.Number7 == i ||
                                lotto.Bonus == i) ? true : false,

                    NumberofDrawsWhenHit =
                                (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                (lottoName != (int)LottoNames.LottoMax || lotto.Number7 != i) &&
                                lotto.Bonus == i) ? prevDraw[i - 1].Distance + 1 : 0,

                    IsBonusNumber = lotto.Bonus == i ? true : false,
                    TotalHits = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                (lottoName != (int)LottoNames.LottoMax || lotto.Number7 != i) &&
                                lotto.Bonus == i) ? prevDraw[i - 1].TotalHits + 1 : prevDraw[i - 1].TotalHits,

                    // probability
                    Probability = CalculateProbability((LottoNames)lastLottoType.LottoName, i).Result,
                };

                numbers.Add(number);
            }
            db.Numbers.AddRange(numbers);
            db.SaveChanges();

        }

    }
}
