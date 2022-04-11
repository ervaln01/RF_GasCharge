namespace RF_GasCharge.Entities.Plis
{
	using Microsoft.EntityFrameworkCore;
	using System.Configuration;

	/// <summary>
	/// Класс контекста базы данных Plis.
	/// </summary>
	public partial class PlisContext : DbContext
	{
		/// <summary>
		/// Конструктор класса <see cref="PlisContext"/>.
		/// </summary>
		public PlisContext() : base() { }

		/// <summary>
		/// Таблица RESULTS_HEADER.
		/// </summary>
		public virtual DbSet<Result> ResultsHeaders { get; set; }

		/// <summary>
		/// Конфигурирование опций.
		/// </summary>
		/// <param name="optionsBuilder">Построитель опций.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => _ = optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["PlisCS"].ConnectionString.Replace("%CONTENTROOTPATH%", System.IO.Directory.GetCurrentDirectory()));

		/// <summary>
		/// Создание модели.
		/// </summary>
		/// <param name="modelBuilder">Построитель модели.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

			modelBuilder.Entity<Result>(entity =>
			{
				entity.HasKey(e => new { e.Seqno, e.Server });
				entity.Property(e => e.DateStartTest).HasDefaultValueSql("(getdate())");
				entity.Property(e => e.DateTested).HasDefaultValueSql("(getdate())");
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