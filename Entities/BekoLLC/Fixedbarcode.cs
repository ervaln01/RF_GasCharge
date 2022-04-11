namespace RF_GasCharge.Entities.GasCharge
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using Microsoft.EntityFrameworkCore;

	[Table("FIXEDBARCODE")]
	[Index(nameof(Location), nameof(Time), nameof(Backup), Name = "IX_LocTImBack")]
	public partial class Fixedbarcode
	{
		[Key]
		[Column("PRODUCT")]
		[StringLength(10)]
		public string Product { get; set; }

		[Key]
		[Column("SERIAL")]
		[StringLength(12)]
		public string Serial { get; set; }

		[Key]
		[Column("LOCATION")]
		public short Location { get; set; }

		[Key]
		[Column("TIME", TypeName = "datetime")]
		public DateTime Time { get; set; }

		[Column("BACKUP")]
		[StringLength(1)]
		public string Backup { get; set; }

		public Fixedbarcode() { }

		public Fixedbarcode(short location)
		{
			Location = location;
			Time = DateTime.Now;
			Backup = "0";
		}
	}
}