namespace SeleniumLottoDataApp
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LottoName")]
    public partial class LottoName
    {
        public int id { get; set; }

        [StringLength(25)]
        public string name { get; set; }
    }
}
