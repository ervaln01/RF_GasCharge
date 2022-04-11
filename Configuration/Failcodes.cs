namespace RF_GasCharge.Configuration
{
	using RF_GasCharge.Entities.GasCharge;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Коды ошибок машин газозакачки и их описания.
	/// </summary>
	public static class Failcodes
	{
		/// <summary>
		/// Словарь описаний ошибок.
		/// </summary>
		public static Dictionary<string, string> Descriptions;

		/// <summary>
		/// Инициализация класса <see cref="Failcodes"/>.
		/// </summary>
		public static void Initialize()
		{
			var bekoContext = new GasChargeContext();
			Descriptions = bekoContext.Failcodes.Where(x => x.Application == Settings.DbName.ToString()).ToDictionary(x => x.Code, x => x.Ru);
		}

		// TODO
		public static List<RepairFailcode> all = new()
		{
			new RepairFailcode() { Code = "0000", Application = "Plis", En = "Failed", Ru = "Сбой в работе" },
			new RepairFailcode() { Code = "0001", Application = "Plis", En = "Passed", Ru = "Тест пройден" },
			new RepairFailcode() { Code = "0300", Application = "Plis", En = "MAX95 passed", Ru = "MAX95 тест пройден" },
			new RepairFailcode() { Code = "0301", Application = "Plis", En = "Evacuation failed", Ru = "Сбой вакуумирования" },
			new RepairFailcode() { Code = "0302", Application = "Plis", En = "Evacuation not processed", Ru = "Вакуумирование не произведено" },
			new RepairFailcode() { Code = "0303", Application = "Plis", En = "Vacuum not reached", Ru = "Вакуум не достигнут" },
			new RepairFailcode() { Code = "0304", Application = "Plis", En = "Vacuum not processed", Ru = "Ошибка вакуумирования" },
			new RepairFailcode() { Code = "0305", Application = "Plis", En = "Charging amount not reached", Ru = "Контур не заполнен" },
			new RepairFailcode() { Code = "0306", Application = "Plis", En = "Charging not processed", Ru = "Ошибка проверки уровня закачки" },
			new RepairFailcode() { Code = "0307", Application = "Plis", En = "Welding failed", Ru = "Ошибка сварного шва - не запаян" },
			new RepairFailcode() { Code = "0308", Application = "Plis", En = "Welding not processed", Ru = "Ошибка проверки сварного шва" },
			new RepairFailcode() { Code = "0309", Application = "Plis", En = "Pause-accept failed", Ru = "Задержка проверки сварного шва" },
			new RepairFailcode() { Code = "0310", Application = "Plis", En = "Pause-accept not processed", Ru = "Задержка проверки сварного шва не начата" },
			new RepairFailcode() { Code = "0311", Application = "Plis", En = "Flushing failed", Ru = "Ошибка продувки" },
			new RepairFailcode() { Code = "0312", Application = "Plis", En = "Flushing not processed", Ru = "Продувка не запущена" },
			new RepairFailcode() { Code = "0313", Application = "Plis", En = "Venting failed", Ru = "Ошибка удаления газа" },
			new RepairFailcode() { Code = "0314", Application = "Plis", En = "Venting not processed", Ru = "Удаление газа не запущено" },
			new RepairFailcode() { Code = "0315", Application = "Plis", En = "Recovery failed", Ru = "Воостановление не удалось" },
			new RepairFailcode() { Code = "0316", Application = "Plis", En = "Recovery not processed", Ru = "Восстановление не начато" },
			new RepairFailcode() { Code = "0317", Application = "Plis", En = "Compressor check failed", Ru = "Ошибка проверки компрессора" },
			new RepairFailcode() { Code = "0318", Application = "Plis", En = "Compressor check not passed", Ru = "Проверка компрессора не пройдена" },
			new RepairFailcode() { Code = "0319", Application = "Plis", En = "Display text failed", Ru = "Не удалось отобразить текст" },
			new RepairFailcode() { Code = "0320", Application = "Plis", En = "Display text not processed", Ru = "Отображаемый текст не обработан" },
			new RepairFailcode() { Code = "0321", Application = "Plis", En = "Low vacuum not reached", Ru = "Нижний уровень вакуума не достигнут" },
			new RepairFailcode() { Code = "1000", Application = "Plis", En = "Manually passed", Ru = "Пройден в ручном режиме" },
			new RepairFailcode() { Code = "1001", Application = "Plis", En = "Manually failed", Ru = "Не пройден в ручном режиме" },
			new RepairFailcode() { Code = "9000", Application = "Plis", En = "Error code not found", Ru = "Код ошибки не найден" },
			new RepairFailcode() { Code = "9001", Application = "Plis", En = "Exceeded the allowed number of errors", Ru = "Превышено допустимое количество ошибок" },
			new RepairFailcode() { Code = "9002", Application = "Plis", En = "No test results", Ru = "Результатов тестирования нет" },
			new RepairFailcode() { Code = "0000", Application = "Geda", En = "Pass", Ru = "Тест пройден" },
			new RepairFailcode() { Code = "0001", Application = "Geda", En = "Charged circuit", Ru = "Контур заправлен" },
			new RepairFailcode() { Code = "0002", Application = "Geda", En = "Evacuation failed", Ru = "Сбой вакуумирования" },
			new RepairFailcode() { Code = "0003", Application = "Geda", En = "Static leak test failed", Ru = "Сбой статистического теста на утечку" },
			new RepairFailcode() { Code = "0004", Application = "Geda", En = "Clogged circuit", Ru = "Контур засорен" },
			new RepairFailcode() { Code = "0012", Application = "Geda", En = "Refrigerant high pressure", Ru = "Высокая температура хладагента" },
			new RepairFailcode() { Code = "0013", Application = "Geda", En = "Refrigerant low pressure", Ru = "Низкая температура хладагента" },
			new RepairFailcode() { Code = "0017", Application = "Geda", En = "Dosing system end of stroke", Ru = "Конец хода дозирующей системы" },
			new RepairFailcode() { Code = "0018", Application = "Geda", En = "Anomaly in dosing system", Ru = "Сбой в системе дозирования" },
			new RepairFailcode() { Code = "0031", Application = "Geda", En = "Pressure loss in the filler", Ru = "Потеря давления в инжекторе" },
			new RepairFailcode() { Code = "0032", Application = "Geda", En = "Vacuum leak in the filler", Ru = "Утечка вакуума в инжекторе" },
			new RepairFailcode() { Code = "0033", Application = "Geda", En = "Refrigerant not discharged", Ru = "Не выполнен спуск хладогента" },
			new RepairFailcode() { Code = "0034", Application = "Geda", En = "Leak in isolvacuum valve", Ru = "Утечка в отсекающем вакуумном клапане" },
			new RepairFailcode() { Code = "0037", Application = "Geda", En = "Cycle stopped by operator", Ru = "Цикл остановлен оператором" },
			new RepairFailcode() { Code = "9000", Application = "Geda", En = "Error code not found", Ru = "Код ошибки не найден" },
			new RepairFailcode() { Code = "9001", Application = "Geda", En = "Exceeded the allowed number of errors", Ru = "Превышено допустимое количество ошибок" },
			new RepairFailcode() { Code = "9002", Application = "Geda", En = "No test results", Ru = "Результатов тестирования нет" }
		};
	}
}