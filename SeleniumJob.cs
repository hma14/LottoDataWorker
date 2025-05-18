using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumLottoDataApp.Lib;
using SeleniumLottoDataApp;
using static SeleniumLottoDataApp.BusinessModels.Constants;

namespace LottoDataWorker
{
    internal class SeleniumJob
    {
        private readonly LottoDbContext _context;
        private readonly ILogger<SeleniumJob> _logger;

        public SeleniumJob(LottoDbContext context, ILogger<SeleniumJob> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task RunSeleniumScraper()
        {
            _logger.LogInformation("Selenium job started at {time}", DateTime.Now);

            try
            {
                using (var driver = new ChromeDriver())
                {
                    // BC lotto
                    LottoBase obj = new LottoMAX(_context);
                    obj.InsertDb();
                    obj.InsertLottTypeTable((int)LottoNames.LottoMax);


                    obj = new Lottery649(_context);
                    obj.InsertDb();
                    obj.InsertLottTypeTable((int)LottoNames.Lotto649);

                    obj = new LottoBC49(_context);
                    obj.InsertDb();
                    obj.InsertLottTypeTable((int)LottoNames.BC49);

                    obj = new LottoDailyGrand(_context);
                    obj.InsertDb();  
                    obj.InsertLottTypeTable((int)LottoNames.DailyGrand);

                    obj = new LottoDailyGrand_GrandNumber(_context);
                    obj.InsertLottTypeTable();


                    obj.CloseDriver();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Selenium scraping.");
            }

            _logger.LogInformation("Selenium job completed at {time}", DateTime.Now);
            await Task.CompletedTask;
        }
    }
}
