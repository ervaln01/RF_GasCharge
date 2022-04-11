namespace RF_GasCharge.Configuration
{
	using System.Configuration;

	/// <summary>
	/// Конфигурация соединения через Plc.
	/// </summary>
	public class PlcConfig
	{
		/// <summary>
		/// Ip-адрес контроллера.
		/// </summary>
		public string Ip { get; private set; }

		/// <summary>
		/// Байт присутствия.
		/// </summary>
		public int Presence { get; private set; }

		/// <summary>
		/// Байт нажатия на кнопку.
		/// </summary>
		public int Button { get; private set; }

		/// <summary>
		/// Начальный адрес читаемого диапазона адресов.
		/// </summary>
		public int RangeStart { get; private set; }

		/// <summary>
		/// Размер диапазона читаемых адресов.
		/// </summary>
		public int RangeSize { get; private set; }

		/// <summary>
		/// Адрес для отправки данных в контроллер (запуск конвейера).
		/// </summary>
		public string AddressToSend { get; private set; }

		/// <summary>
		/// Команда запуска конвейера.
		/// </summary>
		public int CmdStart { get; private set; }

		/// <summary>
		/// Команда аварийного запуска конвейера.
		/// </summary>
		public int CmdStartEmergency { get; private set; }

		/// <summary>
		/// Команда повторной отправки баркода в машину.
		/// </summary>
		public int CmdRepeatSend { get; private set; }

		/// <summary>
		/// Конструктор класса <see cref="PlcConfig"/>.
		/// </summary>
		public PlcConfig()
		{
			Ip = ConfigurationManager.AppSettings["plc_ip"];
			Presence = int.Parse(ConfigurationManager.AppSettings["plc_presence"]);
			Button = int.Parse(ConfigurationManager.AppSettings["plc_button"]);
			RangeStart = int.Parse(ConfigurationManager.AppSettings["plc_range_start"]);
			RangeSize = int.Parse(ConfigurationManager.AppSettings["plc_range_size"]);
			AddressToSend = ConfigurationManager.AppSettings["plc_address_to_send"];
			CmdStart = int.Parse(ConfigurationManager.AppSettings["plc_start"]);
			CmdStartEmergency = int.Parse(ConfigurationManager.AppSettings["plc_start_emergency"]);
			CmdRepeatSend = int.Parse(ConfigurationManager.AppSettings["plc_repeat_send"]);
		}
	}
}