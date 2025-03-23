namespace SeleniumLottoDataApp
{
    using SeleniumLottoDataApp.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DailyGrand_GrandNumber")]
    public partial class DailyGrand_GrandNumber : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DrawNumber { get; set; }
     
        public DateTime DrawDate { get; set; }

        public int? GrandNumber { get; set; }
    }
}
