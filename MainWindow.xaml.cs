namespace RF_GasCharge
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Threading;

	using RF_GasCharge.Configuration;
	using RF_GasCharge.Entities.GasCharge;
	using RF_GasCharge.Entities.Geda;
	using RF_GasCharge.Entities.Plis;
	using RF_GasCharge.Logic;
	using RF_GasCharge.Recording;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Plc-контроллер.
		/// </summary>
		private PlcInteraction plc;

		/// <summary>
		/// Контекс базы данных BekoLLC.
		/// </summary>
		private static GasChargeContext bekoContext;

		/// <summary>
		/// Событие прочтения баркода.
		/// </summary>
		private readonly Action<string> BarcodeReaded;

		/// <summary>
		/// Лог.
		/// </summary>
		private readonly BekoLogFile _log;

		/// <summary>
		/// Сом-порт.
		/// </summary>
		private ComportInteraction comport;

		/// <summary>
		/// Текущий баркод.
		/// </summary>
		private string CurrentBarcode;

		/// <summary>
		/// Флаг отправки на ремонт до закачки газом.
		/// </summary>
		private bool SendToRepair;

		/// <summary>
		/// Конструктор класса <see cref="MainWindow"/>.
		/// </summary>
		/// <param name="log">Лог.</param>
		public MainWindow(BekoLogFile log)
		{
			_log = log;
			InitializeComponent();
			InitializeContent();
			AsyncRecorder.Initialize(Dispatcher.CurrentDispatcher);

			BarcodeReaded += (barcode) =>
			{
				if (!string.IsNullOrEmpty(barcode))
				{
					var product = barcode[..10];
					var serial = barcode[10..];
					var repair = bekoContext.Repairs.Count(x => x.Product.Equals(product) && x.Serial.Equals(serial) && (x.St == Settings.Station || x.St == Settings.NeighborStation));
					if (repair > Settings.RepairsCount)
					{
						Saver.SaveRepair(barcode, "9001", _log);
						ImgOut.SetVisibility(Visibility.Visible);
						ResponseDesc.SetLabel("Сразу на ремонт");
						Log.AppendMessage($"{barcode} - сразу отправлен на ремонт");
						Task.Run(async () => await _log.WriteLogAsync($"{barcode} - сразу на ремонт, count > 3"));
						SendToRepair = true;
						return;
					}
					
					Task.Run(() => comport.WriteToPort(barcode));
				}
			};
		}

		/// <summary>
		/// Ожидание результатов в базе данных Plis.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		private void PlisQuery(string barcode) => Wait(Extensions.PlisQuery(barcode), barcode);

		/// <summary>
		/// Ожидание результатов в базе данных Geda.
		/// </summary>
		/// <param name="barcode">Баркод.</param>
		private void GedaQuery(string barcode) => Wait(Extensions.GedaQuery(barcode), barcode);

		/// <summary>
		/// Ожидание результатов заданного типа в заданной базе данных.
		/// </summary>
		/// <typeparam name="T">Один из классов <see cref="Result"/> или <see cref="Report"/>.</typeparam>
		/// <param name="query">Запрос.</param>
		/// <param name="barcode">Баркод.</param>
		private void Wait<T>(List<T> query, string barcode)
		{
			TestBarcode.SetLabel(barcode);
			var resultWaiting = new ResultWaiting<T>(_log);
			resultWaiting.ReadDescription += ResponseDesc.SetLabel;
			resultWaiting.ResultTest += (done) =>
			{
				if (done)
				{
					ImgDone.SetVisibility(Visibility.Visible);
					ImgError.SetVisibility(Visibility.Hidden);
					ImgQuestion.SetVisibility(Visibility.Hidden);
					return;
				}
				ImgError.SetVisibility(Visibility.Visible);
				ImgDone.SetVisibility(Visibility.Hidden);
				ImgQuestion.SetVisibility(Visibility.Hidden);
			};
			resultWaiting.Process(query, barcode);
		}

		/// <summary>
		/// Обработка события загрузки окна.
		/// </summary>
		/// <param name="sender">Объект, вызвавший событие.</param>
		/// <param name="e">Аргумент события.</param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			BekoContextInit();
			if (Settings.DbName == GasDb.Plis) PlisContextInit();
			if (Settings.DbName == GasDb.Geda) GedaContextInit();

			Task.Run(() => PlcInit());
			Task.Run(() => PortsInit());
			Task.Run(() => TimerInit());
		}
	}
}