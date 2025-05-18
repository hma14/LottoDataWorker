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
    public class LottoBC49 : LottoBase
    {
        public LottoBC49(LottoDbContext lottoDbContext) : base(lottoDbContext)
        {
            string url = "https://www.playnow.com/lottery/bc-49-winning-numbers/";
            Driver.Navigate().GoToUrl(url);
        }

        private DateTime searchDrawDate()
        {
            var dat = Driver.FindElements(By.ClassName("product-date-picker__draw-date"));
            var arr = dat[0].Text.Split();
            var da = arr[3] + '-' + DicDateShort3[arr[1].ToUpper()] + "-" + arr[2].Trim(',');
            return DateTime.Parse(da);
        }

        private List<string> searchDrawNumbers()
        {
            List<string> NList = new List<string>();
            var list = Driver.FindElements(By.ClassName("product-winning-numbers__number_bc49"));
            foreach (var lst in list)
            {
                NList.Add(lst.Text);
            }
            var list2 = Driver.FindElements(By.ClassName("product-winning-numbers__bonus-number_bc49"));
            if (list2 == null || !list2.Any())
                return null;
            NList.Add(list2[0].Text);
            return NList;
        }

        internal override void InsertDb()
        {
            var last = db.BC49.ToList().Last();
            var currentDrawDate = searchDrawDate();

            if (currentDrawDate > last.DrawDate)
            {
                var lastDrawNumber = last.DrawNumber;
                var numbers = searchDrawNumbers();
                if (numbers != null)
                {
                    var entity = new BC49();
                    entity.DrawNumber = lastDrawNumber + 1;
                    entity.DrawDate = currentDrawDate;
                    entity.Number1 = int.Parse(numbers[0]);
                    entity.Number2 = int.Parse(numbers[1]);
                    entity.Number3 = int.Parse(numbers[2]);
                    entity.Number4 = int.Parse(numbers[3]);
                    entity.Number5 = int.Parse(numbers[4]);
                    entity.Number6 = int.Parse(numbers[5]);
                    entity.Bonus = int.Parse(numbers[6]);

                    try
                    {
                        // save to db
                        db.BC49.Add(entity);
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
            
            Driver.Close();
            Driver.Quit();
        }


#if false
        internal override void InsertLottTypeTable()
        {
            var lotto = db.BC49.ToList().Last();
            var lastLottoType = db.LottoTypes
                .Where(x => x.LottoName == (int)LottoNames.BC49)
                .OrderByDescending(d => d.DrawNumber).First();

            if (lotto.DrawNumber == lastLottoType.DrawNumber) return;

            var prevDraw = db.Numbers.Where(x => x.LottoTypeId == lastLottoType.Id)
                                        .OrderBy(n => n.Value).ToArray();

            // Store to LottoType table
            LottoType lottoType = new LottoType
            {
                Id = Guid.NewGuid(),
                LottoName = (int)LottoNames.BC49,
                DrawNumber = lotto.DrawNumber,
                DrawDate = lotto.DrawDate,
                NumberRange = (int)LottoNumberRange.BC49,
            };
            db.LottoTypes.Add(lottoType);

            //Store to Numbers table
            List<Number> numbers = new List<Number>();
            for (int i = 1; i <= (int)LottoNumberRange.BC49; i++)
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
                                lotto.Number5 != i &&
                                lotto.Number6 != i &&
                                lotto.Bonus != i) ? prevDraw[i - 1].Distance + 1 : 0,

                    IsHit = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                lotto.Bonus == i) ? true : false,


                    NumberofDrawsWhenHit =
                                (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                lotto.Bonus == i) ? prevDraw[i - 1].Distance + 1 : 0,

                    IsBonusNumber = lotto.Bonus == i ? true : false,
                    TotalHits = (lotto.Number1 == i ||
                                lotto.Number2 == i ||
                                lotto.Number3 == i ||
                                lotto.Number4 == i ||
                                lotto.Number5 == i ||
                                lotto.Number6 == i ||
                                lotto.Bonus == i) ? prevDraw[i - 1].TotalHits + 1 : prevDraw[i - 1].TotalHits,

                    // probability
                    Probability = CalculateProbability(LottoNames.BC49, i)?.Result ?? 0,
                };
                numbers.Add(number);
            }
            db.Numbers.AddRange(numbers);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            
        }
#endif
    }
}
