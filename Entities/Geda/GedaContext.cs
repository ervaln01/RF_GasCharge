namespace RF_GasCharge.Entities.Geda
{
	using Microsoft.EntityFrameworkCore;
	using System.Configuration;

	/// <summary>
	/// Класс контекста базы данных Geda.
	/// </summary>
	public partial class GedaContext : DbContext
	{
		/// <summary>
		/// Конструктор класса <see cref="GedaContext"/>.
		/// </summary>
		public GedaContext() : base() { }

		/// <summary>
		/// Таблица Report.
		/// </summary>
		public virtual DbSet<Report> Reports { get; set; }

		/// <summary>
		/// Конфигурирование опций.
		/// </summary>
		/// <param name="optionsBuilder">Построитель опций.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => _ = optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["GedaCS"].ConnectionString.Replace("%CONTENTROOTPATH%", System.IO.Directory.GetCurrentDirectory()));

		/// <summary>
		/// Создание модели.
		/// </summary>
		/// <param name="modelBuilder">Построитель модели.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

			modelBuilder.Entity<Report>(entity =>
			{
				entity.Property(e => e.Alarm1).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.Alarm2).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.IdOperator).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.Idp).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.Ids).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.Result).IsUnicode(false).IsFixedLength(true);
				entity.Property(e => e.Structure).IsUnicode(false).IsFixedLength(true);
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