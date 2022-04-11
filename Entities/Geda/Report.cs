namespace RF_GasCharge.Entities.Geda
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Report")]
	public partial class Report
	{
		[Key]
		[Column("IDR")]
		public int Idr { get; set; }

		[Column("IDC")]
		public int? Idc { get; set; }

		[Column("IDP")]
		[StringLength(40)]
		public string Idp { get; set; }

		[Column("IDS")]
		[StringLength(10)]
		public string Ids { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime Date { get; set; }

		[Required]
		[StringLength(4)]
		public string Result { get; set; }

		[Required]
		[StringLength(4)]
		public string Alarm1 { get; set; }

		public double? AlarmSetPoint1 { get; set; }

		[StringLength(4)]
		public string Alarm2 { get; set; }

		public double? AlarmSetPoint2 { get; set; }

		[StringLength(5)]
		public string IdOperator { get; set; }

		[StringLength(30)]
		public string Structure { get; set; }
	}
}