namespace RF_GasCharge.Entities.Plis
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.EntityFrameworkCore;

	[Table("RESULTS_HEADER")]
	[Index(nameof(Barcode), nameof(DateTested), Name = "IX_RESULTS_HEADER")]
	[Index(nameof(DeviceId), nameof(Barcode), nameof(LimittypeId), nameof(Server), Name = "IX_RESULTS_HEADER_1")]
	[Index(nameof(LimitsId), nameof(LimitsHistoryId), nameof(Server), Name = "IX_RESULTS_HEADER_2")]
	[Index(nameof(LimittypeId), nameof(Server), Name = "IX_RESULTS_HEADER_AREA")]
	[Index(nameof(Barcode), Name = "IX_RESULTS_HEADER_BARCODE")]
	[Index(nameof(ParentSeqno), nameof(Server), Name = "IX_RESULTS_HEADER_CHILD")]
	[Index(nameof(DateTested), Name = "IX_RESULTS_HEADER_DATE")]
	[Index(nameof(DeviceId), Name = "IX_RESULTS_HEADER_DEVICE")]
	[Index(nameof(Failcode), nameof(DeviceId), nameof(Server), Name = "IX_RESULTS_HEADER_FAILCODE")]
	[Index(nameof(LimitsId), nameof(Complete), Name = "IX_RESULTS_HEADER_LIM_COMP")]
	[Index(nameof(LineId), nameof(Server), Name = "IX_RESULTS_HEADER_LINE")]
	[Index(nameof(ProcessorId), nameof(UnitId), nameof(Server), Name = "IX_RESULTS_HEADER_PROCESSOR")]
	[Index(nameof(WorkshiftId), nameof(Server), Name = "IX_RESULTS_HEADER_SHIFT")]
	[Index(nameof(Barcode), nameof(Server), Name = "IX_RESULTS_HEADER_SRV")]
	[Index(nameof(UnitId), nameof(Server), Name = "IX_RESULTS_HEADER_UNIT")]
	[Index(nameof(Seqno), nameof(UnitId), nameof(ModelId), nameof(Server), Name = "IX_RESULTS_HEADER_UNIT_MOD")]
	public partial class Result
	{
		[Key]
		[Column("SEQNO")]
		public long Seqno { get; set; }

		[Required]
		[Column("BARCODE")]
		[StringLength(50)]
		public string Barcode { get; set; }

		[Column("DATE_TESTED", TypeName = "datetime")]
		public DateTime DateTested { get; set; }

		[Column("UNIT_ID")]
		public short UnitId { get; set; }

		[Column("DEVICE_ID")]
		public short DeviceId { get; set; }

		[Column("LINE_ID")]
		public short LineId { get; set; }

		[Column("WORKSHIFT_ID")]
		public short WorkshiftId { get; set; }

		[Required]
		[Column("MODEL_ID")]
		[StringLength(50)]
		public string ModelId { get; set; }

		[Column("FAILCODE")]
		public long Failcode { get; set; }

		[Column("OPERATOR_CODE")]
		public long OperatorCode { get; set; }

		[Column("LIMITS_ID")]
		public long LimitsId { get; set; }

		[Column("LIMITS_HISTORY_ID")]
		public long LimitsHistoryId { get; set; }

		[Column("STATUS")]
		public byte Status { get; set; }

		[Column("AREA_ID")]
		public short AreaId { get; set; }

		[Column("LIMITTYPE_ID")]
		public short LimittypeId { get; set; }

		[Column("COMPLETE")]
		public byte Complete { get; set; }

		[Column("LIMITS_DESCRIPTOR")]
		[StringLength(50)]
		public string LimitsDescriptor { get; set; }

		[Column("DATE_START_TEST", TypeName = "datetime")]
		public DateTime? DateStartTest { get; set; }

		[Column("SYNC_DATE", TypeName = "datetime")]
		public DateTime? SyncDate { get; set; }

		[Column("REWORK")]
		public byte? Rework { get; set; }

		[Column("PRE_WORK")]
		public byte? PreWork { get; set; }

		[Column("POST_WORK")]
		public byte? PostWork { get; set; }

		[Column("PROCESSOR_ID")]
		public short? ProcessorId { get; set; }

		[Column("PARENT_SEQNO")]
		public long? ParentSeqno { get; set; }

		[Key]
		[Column("SERVER")]
		public byte Server { get; set; }
	}
}