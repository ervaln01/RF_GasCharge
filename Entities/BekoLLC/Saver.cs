namespace RF_GasCharge.Entities.GasCharge
{
	using Microsoft.EntityFrameworkCore;
	using RF_GasCharge.Configuration;
	using RF_GasCharge.Recording;
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Класс сохранения данных в БД.
	/// </summary>
	public static class Saver
	{
		/// <summary>
		/// Сохранение данных в таблицу Fixedbarcode.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		public static void SaveFixedbarcode(string barcode)
		{
			var context = new GasChargeContext();
			var fixedbarcode = new Fixedbarcode(Settings.Station)
			{
				Product = barcode[..10],
				Serial = barcode[10..]
			};
			var existItem = context.Fixedbarcodes.Where(x =>
				x.Product == fixedbarcode.Product &&
				x.Serial == fixedbarcode.Serial &&
				x.Location == fixedbarcode.Location).FirstOrDefault();

			if (existItem != null) fixedbarcode.Backup = "2";

			context.Fixedbarcodes.Add(fixedbarcode);
			context.SaveChanges();
		}

		/// <summary>
		/// Сохранение данных в таблицу Repair.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		/// <param name="errorCode">Код ошибки.</param>
		public static void SaveRepair(string barcode, string errorCode, BekoLogFile log)
		{
			using var context = new GasChargeContext();
			if (barcode.Length != 22) return;

			var product = barcode[..10];
			var serial = barcode[10..];

			var repair = new Repair(Settings.Station)
			{
				Product = product,
				Serial = serial,
				Count = 1,
				Tcode = errorCode
			};
			try
			{
				var existItem = context.Repairs.FirstOrDefault(x =>
					x.Product.Equals(product) &&
					x.Serial.Equals(serial) &&
					x.Tcode.Equals(errorCode) &&
					x.Count == 1 &&
					x.Rejectst == 0 &&
					x.Rid.Equals("0") &&
					x.Rcode.Equals("0") &&
					x.Line == 1 &&
					x.Backup.Equals("0") &&
					(x.St == Settings.Station || x.St == Settings.NeighborStation));

				if (existItem == null) context.Database.ExecuteSqlRaw(repair.Insert());
			}
			catch (Exception e)
			{
				Task.Run(async () => await log.WriteLogAsync($"Barcode: {barcode} ErrorCode: {errorCode}. При операции Insert в таблице Repair произошла ошибка {e.Message}"));
			}
		}
	}
}