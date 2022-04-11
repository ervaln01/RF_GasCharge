namespace RF_GasCharge.Configuration
{
	/// <summary>
	/// Название подключаемой базы данных.
	/// </summary>
	public enum GasDb
	{
		/// <summary>
		/// База данных Plis (машина Agramkow).
		/// </summary>
		Plis,

		/// <summary>
		/// База данных Geda (машина Galeleo).
		/// </summary>
		Geda,

		/// <summary>
		/// Неизвестная конфигурация базы данных.
		/// </summary>
		Unknown
	}
}