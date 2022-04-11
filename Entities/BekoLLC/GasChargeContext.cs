namespace RF_GasCharge.Entities.GasCharge
{
	using Microsoft.EntityFrameworkCore;

	using System.Configuration;

	/// <summary>
	/// Класс контекста базы данных BekoLLC.
	/// </summary>
	public partial class GasChargeContext : DbContext
	{
		/// <summary>
		/// Конструктор класса <see cref="GasChargeContext"/>.
		/// </summary>
		public GasChargeContext() : base() { }

		/// <summary>
		/// Таблица FIXEDBARCODE.
		/// </summary>
		public virtual DbSet<Fixedbarcode> Fixedbarcodes { get; set; }

		/// <summary>
		/// Таблица REPAIR.
		/// </summary>
		public virtual DbSet<Repair> Repairs { get; set; }

		/// <summary>
		/// Таблица REPAIR_FAILCODE.
		/// </summary>
		public virtual DbSet<RepairFailcode> Failcodes { get; set; }

		/// <summary>
		/// Конфигурирование опций.
		/// </summary>
		/// <param name="optionsBuilder">Построитель опций.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => _ = optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["GasChargeCS"].ConnectionString.Replace("%CONTENTROOTPATH%", System.IO.Directory.GetCurrentDirectory()));

		/// <summary>
		/// Создание модели.
		/// </summary>
		/// <param name="modelBuilder">Построитель модели.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("Relational:Collation", "Turkish_CI_AS");

			modelBuilder.Entity<Fixedbarcode>(entity =>
			{
				entity.HasKey(e => new { e.Product, e.Serial, e.Location, e.Time }).HasName("aaaaaFIXEDBARCODE_PK").IsClustered(false);
				entity.Property(e => e.Time).HasDefaultValueSql("(getdate())");
			});

			modelBuilder.Entity<Repair>(entity =>
			{
				entity.Property(e => e.Backup).HasDefaultValueSql("(0)");
				entity.Property(e => e.Line).HasDefaultValueSql("(0)");
				entity.Property(e => e.Rejectst).HasDefaultValueSql("(0)");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		/// <summary>
		/// Базовый метод создания модели.
		/// </summary>
		/// <param name="modelBuilder">Построитель модели.</param>
		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}