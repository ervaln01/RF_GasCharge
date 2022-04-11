namespace RF_GasCharge.Configuration
{
	using System;
	using System.Configuration;

	/// <summary>
	/// Класс настроек приложения, описанных в файле App.config.
	/// </summary>
	public static class Settings
	{		
		/// <summary>
		/// Конфигурация Com-портов.
		/// </summary>
		public static ComConfig Com { get; private set; }

		/// <summary>
		/// Номер станции.
		/// </summary>
		public static short Station { get; private set; }

		/// <summary>
		/// Номер соседней станции.
		/// </summary>
		public static short NeighborStation { get; private set; }

		/// <summary>
		/// Конфигурация соединения.
		/// </summary>
		public static PlcConfig Config { get; private set; }

		/// <summary>
		/// Название базы данных, с которой работает машина газозакачки.
		/// </summary>
		public static GasDb DbName { get; private set; }

		/// <summary>
		/// Разрешенное количество ошибок.
		/// </summary>
		public static int ErrorsCount { get; private set; }

		/// <summary>
		/// Разрешенное количество ремонтов.
		/// </summary>
		public static int RepairsCount { get; private set; }

		/// <summary>
		/// Разрешенный тайм-аут учета ошибок.
		/// </summary>
		public static int Timeout { get; private set; }

		/// <summary>
		/// Флаг отладки приложения.
		/// </summary>
		public static bool Debug { get; private set; }

		/// <summary>
		/// Инициализация класса <see cref="Settings"/>.
		/// </summary>
		public static void Initialize()
		{
			Com = new();
			Timeout = GetTimeout();
			DbName = GetGasDb();
			ErrorsCount = GetErrorCount();
			RepairsCount = int.Parse(ConfigurationManager.AppSettings["repairs_count"]);
			Station = short.Parse(ConfigurationManager.AppSettings["station_number"]);
			NeighborStation = short.Parse(ConfigurationManager.AppSettings["station_number_neighbor"]);
			Debug = bool.Parse(ConfigurationManager.AppSettings["debug"]);

			Config = new PlcConfig();
		}

		/// <summary>
		/// Получение таймаута (целое отрицательное число минут).
		/// </summary>
		/// <returns>Таймаут.</returns>
		private static int GetTimeout()
		{
			var timeout = int.Parse(ConfigurationManager.AppSettings["cycle_timeout"]);
			return timeout > 0 ? -timeout : timeout;
		}

		/// <summary>
		/// Получение базы данных.
		/// </summary>
		/// <returns>Подключаемая база данных.</returns>
		private static GasDb GetGasDb()
		{
			var dbname = ConfigurationManager.AppSettings["db_name"];
			var parsed = Enum.TryParse(dbname, true, out GasDb db);
			return parsed ? db : GasDb.Unknown;
		}

		/// <summary>
		/// Получение допустимого количества ошибок закачки (отмена ограничений при установленном значении меньше 1).
		/// </summary>
		/// <returns>Допустимое количество ошибок закачки.</returns>
		private static int GetErrorCount()
		{
			var errorsCount = int.Parse(ConfigurationManager.AppSettings["errors_count"]);
			return errorsCount < 1 ? int.MaxValue : errorsCount;
		}
	}
}