using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumLottoDataApp.BusinessModels
{
    public class Constants
    {
        public enum LottoNames
        {
            BC49 = 1,
            Lotto649 = 2,
            LottoMax = 3,
            DailyGrand = 4,
            DailyGrand_GrandNumber = 5,
            //Cash4Life = 6,
            //Cash4Life_CashBall = 7,


        }
        public enum LottoNumberRange
        {
            BC49 = 49,
            Lotto649 = 49,
            LottoMax = 50,
            LottoCash4Life = 60,
            DailyGrand = 49,
            DailyGrand_GrandNumber = 7,
            //Cash4Life = 6,
            //Cash4Life_CashBall = 4,
        }

        public const int COLD_POINT = 15;
        public const int HOT_POINT = 5;

        public const int HOT_RANGE = 5;
        public const int NORMAL_RANGE = 10;

    }
}
