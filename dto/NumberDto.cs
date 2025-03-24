using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeleniumLottoDataApp.Dto
{
    public class NumberDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("distance")]
        public int Distance { get; set; }

        [JsonPropertyName("isHit")]
        public bool IsHit { get; set; }

        [JsonPropertyName("numberofDrawsWhenHit")]
        public int NumberofDrawsWhenHit { get; set; }

        [JsonPropertyName("isBonusNumber")]
        public bool IsBonusNumber { get; set; }

        [JsonPropertyName("totalHits")]
        public int TotalHits { get; set; }

        // new

        [JsonPropertyName("isNextPotentialHit")]
        public bool? IsNextPotentialHit { get; set; }

        [JsonPropertyName("lottoName")]
        public int? LottoName { get; set; }

        [JsonPropertyName("drawNumber")]
        public int? DrawNumber { get; set; }

        [JsonPropertyName("drawDate")]
        public DateTime? DrawDate { get; set; }

        [JsonPropertyName("numberRange")]
        public int? NumberRange { get; set; }
    }
}
