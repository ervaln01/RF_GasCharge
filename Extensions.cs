namespace RF_GasCharge
{
	using RF_GasCharge.Configuration;
	using RF_GasCharge.Entities.Geda;
	using RF_GasCharge.Entities.Plis;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Класс расширений методов.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Получение кода ошибки.
		/// </summary>
		/// <typeparam name="T">Один из классов <see cref="Result"/> или <see cref="Report"/>.</typeparam>
		/// <param name="reports">Список результатов.</param>
		/// <returns>Код ошибки.</returns>
		public static string FailIdQiery<T>(this List<T> reports) => GetGasDb(reports) switch
		{
			GasDb.Plis => (reports as List<Result>).OrderByDescending(x => x.DateTested)?.FirstOrDefault()?.Failcode.ToString(),
			GasDb.Geda => (reports as List<Report>).OrderByDescending(x => x.Date).FirstOrDefault()?.Alarm1,
			_ => null
		};

		/// <summary>
		/// Получение результата тестов.
		/// </summary>
		/// <typeparam name="T">Один из классов <see cref="Result"/> или <see cref="Report"/>.</typeparam>
		/// <param name="reports">Список результатов.</param>
		/// <param name="failId">Код ошибки.</param>
		/// <returns>Тест пройден?</returns>
		public static bool GetTestResult<T>(this List<T> reports, string failId) => GetGasDb(reports) switch
		{
			GasDb.Plis => new List<string> { "0001", "0300", "1000" }.Contains(failId),
			GasDb.Geda => failId.Equals("0000"),
			_ => false
		};

		/// <summary>
		/// Получение базы данных
		/// </summary>
		/// <typeparam name="T">Один из классов <see cref="Result"/> или <see cref="Report"/>.</typeparam>
		/// <param name="reports">Список результатов.</param>
		/// <returns>База данных машины газозакачки.</returns>
		private static GasDb GetGasDb<T>(this List<T> reports)
		{
			if (reports is List<Result>) return GasDb.Plis;
			if (reports is List<Report>) return GasDb.Geda;
			return GasDb.Unknown;
		}

		/// <summary>
		/// Получение списка результатов тестирования из базы Plis.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		/// <returns>Список результатов тестирования.</returns>
		public static List<Result> PlisQuery(string barcode)
		{
			using var contex = new PlisContext();
			return contex.ResultsHeaders.Where(x => x.Barcode.Equals(barcode) && x.DateTested > DateTime.Now.AddMinutes(Settings.Timeout)).ToList();
		}

		/// <summary>
		/// Получение списка результатов тестирования из базы Geda.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		/// <returns>Список результатов тестирования.</returns>
		public static List<Report> GedaQuery(string barcode)
		{
			using var context = new GedaContext();
			return context.Reports.Where(x => x.Idp.Equals(barcode) && x.Date > DateTime.Now.AddMinutes(Settings.Timeout)).ToList();
		}
	}
}