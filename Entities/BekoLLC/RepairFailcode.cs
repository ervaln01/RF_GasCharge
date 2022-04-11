namespace RF_GasCharge.Entities.GasCharge
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("REPAIR_FAILCODE")]
	public partial class RepairFailcode
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Column("CODE")]
		[StringLength(4)]
		public string Code { get; set; }

		[Required]
		[Column("APP_NAME")]
		[StringLength(10)]
		public string Application { get; set; }

		[Required]
		[Column("RU")]
		[StringLength(50)]
		public string Ru { get; set; }

		[Required]
		[Column("EN")]
		[StringLength(50)]
		public string En { get; set; }
	}
}
