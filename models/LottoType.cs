using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumLottoDataApp.Models
{
    public class LottoType
    {
        [Key]
        public Guid Id { get; set; }
        public int LottoName { get; set; }

        public int DrawNumber { get; set; }

        public DateTime DrawDate { get; set; }

        public int NumberRange { get; set; }

        public ICollection<Number> Numbers { get; set; }
    }
}
