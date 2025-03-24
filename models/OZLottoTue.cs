namespace SeleniumLottoDataApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OZLottoTue")]
    public partial class OZLottoTue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DrawNumber { get; set; }

        [StringLength(25)]
        public string DrawDate { get; set; }

        public int? Number1 { get; set; }

        public int? Number2 { get; set; }

        public int? Number3 { get; set; }

        public int? Number4 { get; set; }

        public int? Number5 { get; set; }

        public int? Number6 { get; set; }

        public int? Number7 { get; set; }

        public int? Supp1 { get; set; }

        public int? Supp2 { get; set; }
    }
}
