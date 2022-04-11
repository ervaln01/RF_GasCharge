namespace RF_GasCharge.Logic
{
	using RF_GasCharge.Configuration;
	using System;
	using System.IO.Ports;
	using System.Linq;
	using System.Threading;

	/// <summary>
	/// Класс взаимодействия с Com-портами.
	/// </summary>
	public class ComportInteraction
	{
		/// <summary>
		/// Конфигурация Com-портов.
		/// </summary>
		private readonly ComConfig config;

		/// <summary>
		/// Порт для записи.
		/// </summary>
		private readonly SerialPort writer;

		/// <summary>
		/// Порт для чтения.
		/// </summary>
		private readonly SerialPort reader;

		/// <summary>
		/// Делегат чтения баркода. При вызове возвращает строку баркода.
		/// </summary>
		public Action<string> ReadedBarcode;

		/// <summary>
		/// Делегат логирования. При вызове возвращает логируемую строку.
		/// </summary>
		public Action<string> LogEvent;

		/// <summary>
		/// Конструктор класса <see cref="ComportInteraction"/>.
		/// </summary>
		public ComportInteraction(ComConfig comConfig)
		{
			config = comConfig;
			writer = new(config.ComForWrite, config.ComWriteRate, config.ComWriteParity, config.ComWriteBits, config.ComWriteStop);
			reader = new(config.ComForRead);

			if (config.ComReadAllow)
			{
				reader.DataReceived += (sender, e) =>
				{
					try
					{
						var serialPort = sender as SerialPort;
						Thread.Sleep(150);

						var barcode = serialPort.ReadExisting();
						barcode = string.Join(string.Empty, barcode.ToCharArray().SkipWhile(x => x < '0' || x > '9').TakeWhile(x => x >= '0' && x <= '9'));

						if (barcode.Length == 22)
						{
							ReadedBarcode?.Invoke(barcode);
							LogEvent?.Invoke($"{barcode} получен через com-порт");
						}
					}
					catch (Exception ex)
					{
						LogEvent?.Invoke($"Исключение в com-порте {ex.Message}");
					}
				};
				reader.Open();
			}
		}

		/// <summary>
		/// Запись баркода на заданный порт.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		public void WriteToPort(string barcode)
		{
			try
			{
				if (!config.ComWriteAllow) return;
				if (!writer.IsOpen) writer.Open();
				writer.Write($"{config.STX}{barcode}{config.CR}");
				writer.Close();
			}
			catch (Exception e)
			{
				LogEvent?.Invoke(e.Message);
			}
		}
	}
}