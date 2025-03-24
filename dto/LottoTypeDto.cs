using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeleniumLottoDataApp.Dto
{
    public class LottoTypeDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("lottoName")]
        public int LottoName { get; set; }

        [JsonPropertyName("drawNumber")]
        public int DrawNumber { get; set; }

        [JsonPropertyName("drawDate")]
        public DateTime DrawDate { get; set; }

        [JsonPropertyName("numberRange")]
        public int NumberRange { get; set; }

        [JsonPropertyName("numbers")]
        public List<NumberDto> Numbers { get; set; }
    }
}
