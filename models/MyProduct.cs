namespace SeleniumLottoDataApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MyProduct")]
    public partial class MyProduct
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }
    }
}
