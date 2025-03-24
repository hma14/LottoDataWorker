namespace SeleniumLottoDataApp
{
    using Microsoft.EntityFrameworkCore;
    using SeleniumLottoDataApp.BusinessModels;
    using SeleniumLottoDataApp.Models;
    //using System.Data.Entity;

    public partial class LottoDbContext : DbContext
    {
        public LottoDbContext(DbContextOptions<LottoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BC49> BC49 { get; set; }
        public virtual DbSet<Lotto649> Lotto649 { get; set; }
        public virtual DbSet<LottoMax> LottoMax { get; set; }
        public virtual DbSet<Cash4Life> Cash4Life { get; set; }
        public virtual DbSet<Cash4Life_CashBall> Cash4Life_CashBall { get; set; }
        public virtual DbSet<LottoNumber> LottoNumber { get; set; }
        public virtual DbSet<Number> Numbers { get; set; }
        public virtual DbSet<LottoType> LottoTypes { get; set; }
        public virtual DbSet<DailyGrand> DailyGrand { get; set; }
        public virtual DbSet<DailyGrand_GrandNumber> DailyGrand_GrandNumber { get; set; }

#if false
            public virtual DbSet<BritishLotto> BritishLottoes { get; set; }
            public virtual DbSet<CaSuperlottoPlu> CaSuperlottoPlus { get; set; }
            public virtual DbSet<CaSuperlottoPlus_Mega> CaSuperlottoPlus_Mega { get; set; }
            public virtual DbSet<ColoradoLotto> ColoradoLottoes { get; set; }
            public virtual DbSet<ConnecticutLotto> ConnecticutLottoes { get; set; }
            public virtual DbSet<EuroJackpot> EuroJackpots { get; set; }
            public virtual DbSet<EuroJackpot_Euros> EuroJackpot_Euros { get; set; }
            public virtual DbSet<EuroMillion> EuroMillions { get; set; }
            public virtual DbSet<EuroMillions_LuckyStars> EuroMillions_LuckyStars { get; set; }
            public virtual DbSet<FloridaFantasy5> FloridaFantasy5 { get; set; }
            public virtual DbSet<FloridaLotto> FloridaLottoes { get; set; }
            public virtual DbSet<FloridaLucky> FloridaLuckies { get; set; }
            public virtual DbSet<GermanLotto> GermanLottoes { get; set; }
            
            public virtual DbSet<MegaMillion> MegaMillions { get; set; }
            public virtual DbSet<MegaMillions_MegaBall> MegaMillions_MegaBall { get; set; }
            public virtual DbSet<MyProduct> MyProducts { get; set; }
            public virtual DbSet<NewJerseyPick6Lotto> NewJerseyPick6Lotto { get; set; }
            public virtual DbSet<NYLotto> NYLottoes { get; set; }
            public virtual DbSet<NYSweetMillion> NYSweetMillions { get; set; }
            public virtual DbSet<OregonMegabuck> OregonMegabucks { get; set; }
            public virtual DbSet<OZLottoMon> OZLottoMons { get; set; }
            public virtual DbSet<OZLottoSat> OZLottoSats { get; set; }
            public virtual DbSet<OZLottoTue> OZLottoTues { get; set; }
            public virtual DbSet<OZLottoWed> OZLottoWeds { get; set; }
            public virtual DbSet<PowerBall> PowerBalls { get; set; }
            public virtual DbSet<PowerBall_PowerBall> PowerBall_PowerBall { get; set; }
            public virtual DbSet<SevenLotto> SevenLottoes { get; set; }
            public virtual DbSet<SSQ> SSQs { get; set; }
            public virtual DbSet<SSQ_Blue> SSQ_Blue { get; set; }
            public virtual DbSet<SuperLotto> SuperLottoes { get; set; }
            public virtual DbSet<SuperLotto_Rear> SuperLotto_Rear { get; set; }
            public virtual DbSet<LottoName> LottoNames { get; set; }
            public virtual DbSet<Lotto> Lottos { get; set; }
            public virtual DbSet<NewYorkTake5> NewYorkTake5 { get; set; }
            public virtual DbSet<TexasCashFive> TexasCashFive { get; set; }
#endif


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LottoNumber>()
                .HasKey(ln => new { ln.LottoName, ln.DrawNumber, ln.Number });

            base.OnModelCreating(modelBuilder);

        }
    }
}
