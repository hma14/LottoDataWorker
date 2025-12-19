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
    public class LottoDailyGrand : LottoBase
    {
        public LottoDailyGrand(LottoDbContext lottoDbContext) : base(lottoDbContext)
        {
            string url = "https://www.playnow.com/lottery/daily-grand-winning-numbers/";
            Driver.Navigate().GoToUrl(url);
        }

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
            var td = Driver.FindElements(By.ClassName("product-winning-numbers__numbers-list")).First();
            var lis = td.FindElements(By.TagName("li"));
            foreach (var li in lis)
            {
                numbers.Add(li.Text);
            }

            var grand = Driver.FindElements(By.ClassName("product-winning-numbers__bonus-number_dgrd")).First();
            numbers.Add(grand.Text);
            return numbers;

        }


        internal override void InsertDb()
        {
            var last = db.DailyGrand.ToList().OrderByDescending(x => x.DrawNumber).First();
            var currentDrawDate = searchDrawDate();

            if (currentDrawDate > last.DrawDate)
            {
                var lastDrawNumber = last.DrawNumber;
                var numbers = searchDrawNumbers();
                if (numbers != null)
                {
                    // DailyGrand table
                    var entity = new DailyGrand()
                    {
                        Id = Guid.NewGuid(),
                        DrawNumber = lastDrawNumber + 1,
                        DrawDate = currentDrawDate,
                        Number1 = int.Parse(numbers[0]),
                        Number2 = int.Parse(numbers[1]),
                        Number3 = int.Parse(numbers[2]),
                        Number4 = int.Parse(numbers[3]),
                        Number5 = int.Parse(numbers[4]),

                    };

                    // DailyGrand_GrandNumber table
                    var grand = new DailyGrand_GrandNumber
                    {
                        Id = Guid.NewGuid(),
                        DrawNumber = lastDrawNumber + 1,
                        DrawDate = currentDrawDate,
                        GrandNumber = int.Parse(numbers[5]),
                    };

                    try
                    {
                        // save to db
                        db.DailyGrand.Add(entity);
                        db.DailyGrand_GrandNumber.Add(grand);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var error = e.InnerException != null ? (e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message) : e.Message;
                        Console.WriteLine(error);                    
                    }
                }
            }
            Driver.Close();
            Driver.Quit();
        }


        internal override void InsertLottTypeTable()
        {
            var lotto = db.DailyGrand.ToList().OrderByDescending(x => x.DrawNumber).First();
            var lastLottoType = db.LottoTypes
                .Where(x => x.LottoName == (int)LottoNames.DailyGrand)
                .OrderByDescending(d => d.DrawNumber).First();

            if (lotto.DrawNumber == lastLottoType.DrawNumber) return;

            var prevDraw = db.Numbers.Where(x => x.LottoTypeId == lastLottoType.Id)
                                        .OrderBy(n => n.Value).ToArray();

            // Store to LottoType table
            LottoType lottoType = new LottoType
            {
                Id = Guid.NewGuid(),
                LottoName = (int)LottoNames.DailyGrand,
                DrawNumber = lotto.DrawNumber,
                DrawDate = lotto.DrawDate,
                NumberRange = (int)LottoNumberRange.DailyGrand,
            };


            //Store to Numbers table
            List<Number> numbers = new List<Number>();
            for (int i = 1; i <= (int)LottoNumberRange.DailyGrand; i++)
            {
                Number number = new Number
                {
                    Id = Guid.NewGuid(),
                    Value = i,
                    LottoTypeId = lottoType.Id,
                    Distance = (lotto.Number1 != i &&
                                lotto.Number2 != i &&
                                lotto.Number3 != i &&
                                lotto.Number4 != i &&
                                lotto.Number5 != i) ? prevDraw[i - 1].Distance + 1 : 0,

                    IsHit = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i) ? true : false,


                    NumberofDrawsWhenHit =
                                (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i) ? prevDraw[i - 1].Distance + 1 : 0,

                    TotalHits = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i) ? prevDraw[i - 1].TotalHits + 1 : prevDraw[i - 1].TotalHits,

                    // probability
                    Probability = CalculateProbability(LottoNames.DailyGrand, i).Result,
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
