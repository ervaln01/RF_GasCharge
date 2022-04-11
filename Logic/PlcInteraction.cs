namespace RF_GasCharge.Logic
{
	using S7.Net;

	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using RF_GasCharge.Configuration;

	/// <summary>
	/// Класс взаимодействия с напрямую Plc.
	/// </summary>
	public class PlcInteraction
	{
		#region Fields
		/// <summary>
		/// Конфигурация устройства.
		/// </summary>
		public readonly PlcConfig config;

		/// <summary>
		/// Экземпляр Plc.
		/// </summary>
		private Plc plc;

		/// <summary>
		/// Флаг последнего состояния детектирования.
		/// </summary>
		private bool detection;

		/// <summary>
		/// Последний полученный баркод.
		/// </summary>
		private string lastBarcode;

		private bool owner = true;

		private System.Timers.Timer detector;

		private bool btn = false;
		private DateTime lastRepeat;
		#endregion Fields

		#region Actions
		/// <summary>
		/// Делегат логирования. При вызове возвращает логируемую строку.
		/// </summary>
		public Action<string> LogEvent;

		/// <summary>
		/// Делегат чтения баркода. При вызове возвращает строку баркода.
		/// </summary>
		public Action<string> ReadBarcode;

		/// <summary>
		/// Делегат статуса соединения. При вызове возвращает флаг соединения.
		/// </summary>
		public Action<bool> ConnectionStatus;

		/// <summary>
		/// Делегат статуса обнаружения. При вызове возвращает флаг обнаружения.
		/// </summary>
		public Action<bool> DetectionSensor;

		/// <summary>
		/// Делегат нажатия на кнопку.
		/// </summary>
		public Action<int, bool> ButtonPressed;
		public Action RepeatSend;
		#endregion Actions

		/// <summary>
		/// Конструктор класса <see cref="PlcInteraction"/>.
		/// </summary>
		/// <param name="plcConfig">Конфигурация соединения.</param>
		public PlcInteraction(PlcConfig plcConfig)
		{
			config = plcConfig;
			plc = new(CpuType.Logo0BA8, config.Ip, 0, 2);

			detection = false;
			lastBarcode = string.Empty;
			DetectionSensor += Reading;
			_ = Task.Run(() => Detection());
		}

		/// <summary>
		/// Запись в контроллер команды запуска конвейера.
		/// </summary>
		public void ConveyorStart()
		{
			try
			{
				if (Settings.Debug) return;
				if (string.IsNullOrEmpty(config.AddressToSend)) return;
				plc.Write(config.AddressToSend, (byte)1);
			}
			catch { }
		}

		/// <summary>
		/// Деструктор класса <see cref="PlcInteraction"/>.
		/// </summary>
		~PlcInteraction()
		{
			plc?.Close();
			detector?.Stop();
			detector?.Dispose();
		}

		/// <summary>
		/// Проверка наличия холодильника по таймеру.
		/// </summary>
		private void Detection()
		{
			detector = new() { Interval = 500 };
			detector.Elapsed += (s, e) =>
			{
				try
				{
					if (plc.IsConnected)
					{
						Detector();
						return;
					}

					if (e.SignalTime.Second % 5 == 0 && e.SignalTime.Millisecond > 500 && owner)
					{
						try
						{
							owner = false;
							plc.Close();
							plc = new(CpuType.Logo0BA8, config.Ip, 0, 2);
							plc.Open();
						}
						catch (Exception ex)
						{
							LogEvent?.Invoke(ex.Message);
						}
						owner = true;
						LogEvent?.Invoke($"Соединение с PLC {(plc.IsConnected ? string.Empty : "не ")}установлено");
						ConnectionStatus?.Invoke(plc.IsConnected);
					}
				}
				catch
				{
					ConnectionStatus?.Invoke(plc.IsConnected);
				}
			};
			detector.Start();
		}

		/// <summary>
		/// Проверка бита присутствия.
		/// </summary>
		private void Detector()
		{
			var presence = plc.ReadBytes(DataType.DataBlock, 500, config.Presence, 1).First() != 0;

			if (presence != detection)
			{
				if (!presence) btn = false;
				detection = presence;
				DetectionSensor?.Invoke(presence);
			}

			var buttonComand = plc.ReadBytes(DataType.DataBlock, 500, config.Button, 1).First();
			if (buttonComand == 0) return;

			if (buttonComand == config.CmdRepeatSend)
			{
				var now = DateTime.Now;
				if ((now - lastRepeat).TotalSeconds > 2)
				{
					RepeatSend?.Invoke();
					lastRepeat = now;
				}
			}
			else
			{
				ButtonPressed?.Invoke(buttonComand, !btn);
				btn = true;
			}
		}

		/// <summary>
		/// Запуск чтения баркода.
		/// </summary>
		/// <param name="presence">Бит присутствия.</param>
		internal void Reading(bool presence)
		{
			try
			{
				if (!presence)
				{
					ReadBarcode?.Invoke(string.Empty);
					return;
				}

				Reader();
			}
			catch { }
		}

		/// <summary>
		/// Чтение баркода.
		/// </summary>
		private void Reader()
		{
			try
			{
				var data = plc.ReadBytes(DataType.DataBlock, 500, config.RangeStart, config.RangeSize);
				var barcode = Encoding.ASCII.GetString(data);
				for (var i = 0; data.All(x => x == 0) || i < 15; i++)
				{
					if (barcode.Count(x => char.IsDigit(x)) == 22) break;
					data = plc.ReadBytes(DataType.DataBlock, 500, config.RangeStart, config.RangeSize);
					barcode = Encoding.ASCII.GetString(data);
					_ = Task.Delay(500);
				}

				if (!barcode.Equals(lastBarcode))
				{
					lastBarcode = barcode;
					ReadBarcode?.Invoke(barcode);
				}
			}
			catch (Exception ex)
			{
				LogEvent?.Invoke("Ошибка чтения данных: " + ex.Message);
				return;
			}
		}
	}
}