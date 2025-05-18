using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using SeleniumLottoDataApp.BusinessModels;
using SeleniumLottoDataApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SeleniumLottoDataApp.BusinessModels.Constants;

namespace SeleniumLottoDataApp.Lib
{
    public class LottoDailyGrand_GrandNumber : LottoBase
    {

        public LottoDailyGrand_GrandNumber(LottoDbContext lottoDbContext) : base(lottoDbContext)
        {
            string url = "https://www.playnow.com/lottery/daily-grand-winning-numbers/";
            Driver.Navigate().GoToUrl(url);
        }
#if false

        private DateTime searchDrawDate()
        {
            var tds = Driver.FindElements(By.ClassName("product-date-picker__draw-date"));
            var txt = tds.First().Text;
            var ta = txt.Split(',');
            var year = ta[2];
            var da = ta[1].Split();
            var mon = DicDateShort3[da[1].ToUpper()];
            var day = da[2];
            var dat = $"{year}-{mon}-{day}";
            return DateTime.Parse(dat);

        }

        private List<string> searchDrawNumbers()
        {
            List<string> numbers = new List<string>();
            var td = Driver.FindElementsByClassName("product-winning-numbers__numbers-list").First();
            var lis = td.FindElements(By.TagName("li"));
            foreach (var li in lis)
            {
                numbers.Add(li.Text);
            }

            var grand = Driver.FindElementsByClassName("product-winning-numbers__bonus-number_dgrd").First();
            numbers.Add(grand.Text);
            return numbers;

        }


        internal override void InsertDb()
        {
            using (var db = new LottoDb(new DbContextOptions<LottoDb>()))
            {
                var last = db.DailyGrand.ToList().Last();
                var currentDrawDate = searchDrawDate();

                if (currentDrawDate > last.DrawDate)
                {
                    var lastDrawNumber = last.DrawNumber;
                    var numbers = searchDrawNumbers();
                    if (numbers != null)
                    {
                        var entity = new DailyGrand();
                        entity.DrawNumber = lastDrawNumber + 1;
                        entity.DrawDate = currentDrawDate;
                        entity.Number1 = int.Parse(numbers[0]);
                        entity.Number2 = int.Parse(numbers[1]);
                        entity.Number3 = int.Parse(numbers[2]);
                        entity.Number4 = int.Parse(numbers[3]);
                        entity.Number5 = int.Parse(numbers[4]);

                        try
                        {
                            // save to db
                            db.DailyGrand.Add(entity);

                            // save to GrandNumber
                            var grand = new DailyGrand_GrandNumber();
                            grand.DrawNumber = lastDrawNumber + 1;
                            grand.DrawDate = currentDrawDate;
                            grand.GrandNumber = int.Parse(numbers[5]);

                            db.DailyGrand_GrandNumber.Add(grand);


                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            var error = e.InnerException != null ? (e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message) : e.Message;
                            Console.WriteLine(error);
                            throw e;
                        }
                    }
                }
            }
            Driver.Close();
            Driver.Quit();
        }
#endif


        internal override void InsertLottTypeTable()
        {
            var lotto = db.DailyGrand_GrandNumber.ToList().OrderByDescending(x => x.DrawNumber).First();
            var lastLottoType = db.LottoTypes
                .Where(x => x.LottoName == (int)LottoNames.DailyGrand_GrandNumber)
                .OrderByDescending(d => d.DrawNumber).First();

            if (lotto.DrawNumber == lastLottoType.DrawNumber) return;

            var prevDraw = db.Numbers.Where(x => x.LottoTypeId == lastLottoType.Id)
                                        .OrderBy(n => n.Value).ToArray();

            // Store to LottoType table
            LottoType lottoType = new ()
            {
                Id = Guid.NewGuid(),
                LottoName = (int)LottoNames.DailyGrand_GrandNumber,
                DrawNumber = lotto.DrawNumber,
                DrawDate = lotto.DrawDate,
                NumberRange = (int)LottoNumberRange.DailyGrand_GrandNumber,
            };


            //Store to Numbers table
            List<Number> numbers = [];
            for (int i = 1; i <= (int)LottoNumberRange.DailyGrand_GrandNumber; i++)
            {
                Number number = new Number
                {
                    Id = Guid.NewGuid(),
                    Value = i,
                    LottoTypeId = lottoType.Id,
                    Distance = (lotto.GrandNumber != i) ? prevDraw[i - 1].Distance + 1 : 0,
                    IsHit = (lotto.GrandNumber == i) ? true : false,
                    NumberofDrawsWhenHit = (lotto.GrandNumber == i) ? prevDraw[i - 1].Distance + 1 : 0,
                    TotalHits = (lotto.GrandNumber == i) ? prevDraw[i - 1].TotalHits + 1 : prevDraw[i - 1].TotalHits,

                    // probability
                    Probability = CalculateProbability(LottoNames.DailyGrand_GrandNumber, i).Result,
                };
                numbers.Add(number);
            }

            try
            {
                db.LottoTypes.Add(lottoType);
                db.Numbers.AddRange(numbers);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            
        }
    }
}
