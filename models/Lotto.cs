namespace SeleniumLottoDataApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lottos")]
    public partial class Lotto
    {
        public int id { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(100)]
        public string links { get; set; }
    }
}
