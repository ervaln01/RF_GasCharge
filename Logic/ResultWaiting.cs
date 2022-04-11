namespace RF_GasCharge.Logic
{
	using RF_GasCharge.Configuration;
	using RF_GasCharge.Entities.GasCharge;
	using RF_GasCharge.Entities.Geda;
	using RF_GasCharge.Entities.Plis;
	using RF_GasCharge.Recording;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Класс ожидания результатов в базе данных.
	/// </summary>
	/// <typeparam name="T">Один из классов <see cref="Result"/> или <see cref="Report"/>.</typeparam>
	public class ResultWaiting<T>
	{
		/// <summary>
		/// Делегат результата теста. При вызове возвращает флаг прохождения теста.
		/// </summary>
		public Action<bool> ResultTest;

		/// <summary>
		/// Делегат чтения описания. При вызове возвращает строку описания кода ошибки.
		/// </summary>
		public Action<string> ReadDescription;

		/// <summary>
		/// Лог.
		/// </summary>
		private readonly BekoLogFile _log;

		/// <summary>
		/// Конструктор класса <see cref="ResultWaiting{T}"/>.
		/// </summary>
		/// <param name="log">Лог.</param>
		public ResultWaiting(BekoLogFile log) => _log = log;

		/// <summary>
		/// Обработка результатов газозакачки.
		/// </summary>
		/// <param name="query">Запрос.</param>
		/// <param name="barcode">Баркод.</param>
		public void Process(List<T> reports, string barcode)
		{
			if (reports.Count == 0)
			{
				RecordResult("9002");
				return;
			}

			if (reports.Count > Settings.ErrorsCount)
			{
				RecordResult("9001");
				return;
			}

			var failId = reports.FailIdQiery();
			if (failId.Length == 0)
			{
				RecordResult("9000");
				return;
			}

			failId = $"{failId}".PadLeft(4, '0');
			var result = reports.GetTestResult(failId);

			if (result)
			{
				DisplayResult(barcode, true, failId, Failcodes.Descriptions[failId]);
				Saver.SaveFixedbarcode(barcode);
				return;
			}

			RecordResult(failId);

			void RecordResult(string failId, string failDesc = null)
			{
				DisplayResult(barcode, false, failId, string.IsNullOrEmpty(failDesc) ? Failcodes.Descriptions[failId] : failDesc);
				Saver.SaveRepair(barcode, failId, _log);
			}
		}

		/// <summary>
		/// Визуализация результатов тестов и их логирование.
		/// </summary>
		/// <param name="reports">Список результатов тестов.</param>
		/// <param name="barcode">Баркод.</param>
		/// <returns>Тест пройден?</returns>
		private void DisplayResult(string barcode, bool result, string failId, string failDesc)
		{
			ResultTest?.Invoke(result);
			ReadDescription?.Invoke(failDesc);
			var log = $"{barcode} - {failDesc}$id$ - {DateTime.Now:HH:mm:ss.fff}";
			log = log.Replace("$id$", string.IsNullOrEmpty(failId) ? string.Empty : $" - {failId}");
			Task.Run(async () => await _log.WriteLogAsync(log));
		}
	}
}