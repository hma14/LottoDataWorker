using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeleniumLottoDataApp.BusinessModels.Constants;

namespace SeleniumLottoDataApp.BusinessModels
{
    public class LottoNumber
    {
        [Key, Column(Order = 0)]
        public LottoNames LottoName { get; set; }

        [Key, Column(Order = 1)]
        public int DrawNumber { get; set; }

        public DateTime DrawDate { get; set; }

        [Key, Column(Order = 2)]
        public int Number { get; set; }

        public LottoNumberRange NumberRange { get; set; }

        public int Distance { get; set; }
        public bool IsHit { get; set; }
        public int NumberofDrawsWhenHit { get; set; }
        public bool IsBonusNumber { get; set; }
        public int TotalHits { get; set; }
    }
}
