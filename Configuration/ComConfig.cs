namespace RF_GasCharge.Configuration
{
	using System;
	using System.Configuration;
	using System.IO.Ports;

	/// <summary>
	/// Класс конфигурации Com-порта.
	/// </summary>
	public class ComConfig
	{
		/// <summary>
		/// Наименование Com-порта для записи.
		/// </summary>
		public string ComForWrite { get; private set; }
		public int ComWriteRate { get; private set; }
		public int ComWriteBits { get; private set; }
		public Parity ComWriteParity { get; private set; }
		public StopBits ComWriteStop { get; private set; }

		/// <summary>
		/// Наименование Com-порта для чтения.
		/// </summary>
		public string ComForRead { get; private set; }

		/// <summary>
		/// Разрешение на запись в Com-порт.
		/// </summary>
		public bool ComWriteAllow { get; private set; }

		/// <summary>
		/// Разрешение на чтение в Com-порта.
		/// </summary>
		public bool ComReadAllow { get; private set; }

		/// <summary>
		/// Префикс записи в com-порт.
		/// </summary>
		public string STX { get; private set; }

		/// <summary>
		/// Суффикс записи в com-порт.
		/// </summary>
		public string CR { get; private set; }

		/// <summary>
		/// Конструктор класса <see cref="ComConfig"/>.
		/// </summary>
		public ComConfig()
		{
			ComForRead = ConfigurationManager.AppSettings["com_read"];
			ComForWrite = ConfigurationManager.AppSettings["com_write"];
			ComWriteRate = int.Parse(ConfigurationManager.AppSettings["com_write_rate"]);
			ComWriteBits = int.Parse(ConfigurationManager.AppSettings["com_write_bits"]);
			ComWriteParity = Enum.Parse<Parity>(ConfigurationManager.AppSettings["com_write_parity"]);
			ComWriteStop = Enum.Parse<StopBits>(ConfigurationManager.AppSettings["com_write_stop"]);
			ComReadAllow = bool.Parse(ConfigurationManager.AppSettings["com_read_allow"]);
			ComWriteAllow = bool.Parse(ConfigurationManager.AppSettings["com_write_allow"]);
			STX = GetSuffix("com_stx");
			CR = GetSuffix("com_cr");
		}

		/// <summary>
		/// Получение значения суффикса.
		/// </summary>
		/// <param name="name">Название настройки.</param>
		/// <returns>Значение суффикса.</returns>
		private static string GetSuffix(string name)
		{
			var setting = ConfigurationManager.AppSettings[name];
			if (string.IsNullOrEmpty(setting)) return string.Empty;

			var symbol = int.Parse(setting);
			return $"{(char)symbol}";
		}
	}
}