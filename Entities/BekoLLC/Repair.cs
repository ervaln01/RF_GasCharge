namespace RF_GasCharge.Entities.GasCharge
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.EntityFrameworkCore;

	[Keyless]
	[Table("REPAIR")]
	[Index(nameof(Product), nameof(Serial), nameof(St), nameof(Tcode), nameof(Rcode), Name = "IX_REPAIR")]
	public partial class Repair
	{
		[Required]
		[Column("PRODUCT")]
		[StringLength(10)]
		public string Product { get; set; }

		[Required]
		[Column("SERIAL")]
		[StringLength(12)]
		public string Serial { get; set; }

		[Column("ST")]
		public short St { get; set; }

		[Column("COUNT")]
		public short Count { get; set; }

		[Required]
		[Column("TCODE")]
		[StringLength(4)]
		public string Tcode { get; set; }

		[Column("REJECTST")]
		public short? Rejectst { get; set; }

		[Column("RID")]
		[StringLength(12)]
		public string Rid { get; set; }

		[Required]
		[Column("RCODE")]
		[StringLength(4)]
		public string Rcode { get; set; }

		[Column("RTIME", TypeName = "datetime")]
		public DateTime Rtime { get; set; }

		[Column("LINE")]
		public short? Line { get; set; }

		[Column("BACKUP")]
		[StringLength(1)]
		public string Backup { get; set; }

		[Column("INSERT_TIME", TypeName = "datetime")]
		public DateTime? InsertTime { get; set; }

		public Repair() { }

		public Repair(short station)
		{
			St = station;
			Rejectst = 0;
			Rid = "0";
			Rcode = "0000";
			Rtime = DateTime.MinValue.AddYears(1969);
			Line = 1;
			Backup = "0";
			InsertTime = DateTime.Now;
		}

		public string Insert() => $"INSERT INTO [dbo].[REPAIR]" +
			$"([PRODUCT],[SERIAL],[ST],[COUNT],[TCODE],[REJECTST],[RID],[RCODE],[RTIME],[LINE],[BACKUP],[INSERT_TIME])" +
			$"VALUES" +
			$"('{Product}','{Serial}',{St},{Count},'{Tcode}',{Rejectst},'{Rid}','{Rcode}','{Rtime:yyyy-MM-dd HH:mm:ss}',{Line},'{Backup}','{InsertTime:yyyy-MM-dd HH:mm:ss}')";
	}
}