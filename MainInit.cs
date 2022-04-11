namespace RF_GasCharge
{
	using Microsoft.EntityFrameworkCore;

	using RF_GasCharge.Configuration;
	using RF_GasCharge.Entities.GasCharge;
	using RF_GasCharge.Entities.Geda;
	using RF_GasCharge.Entities.Plis;
	using RF_GasCharge.Logic;
	using RF_GasCharge.Recording;

	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Media.Imaging;

	/// <inheritdoc/>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Инициализация контента окна.
		/// </summary>
		private void InitializeContent()
		{
			Title = "RF_GasCharge";
			SickName.Header = $"Sick - точка {Settings.Station} (закачка газа)";
			GasDB.Header = $"Test - точка {Settings.Station} (база данных {Settings.DbName})";
			GasDB_Ind.Content = Settings.DbName;

			Icon = BitmapFrame.Create(new Uri("img/icon.ico", UriKind.RelativeOrAbsolute));
			ImgDone.Source = BitmapFrame.Create(new Uri("img/done.jpg", UriKind.RelativeOrAbsolute));
			ImgError.Source = BitmapFrame.Create(new Uri("img/error.jpg", UriKind.RelativeOrAbsolute));
			ImgQuestion.Source = BitmapFrame.Create(new Uri("img/question.jpg", UriKind.RelativeOrAbsolute));
			ImgOut.Source = BitmapFrame.Create(new Uri("img/out.jpg", UriKind.RelativeOrAbsolute));
		}

		/// <summary>
		/// Инициализация контекста базы данных BekoLLC.
		/// </summary>
		private void BekoContextInit()
		{
			try
			{
				bekoContext = new GasChargeContext();
				// TODO
				// Failcodes.all.ForEach(x =>
				//{
				//	bekoContext.Failcodes.Add(x);
				//	bekoContext.SaveChanges();
				//});
				_ = bekoContext.Database.ExecuteSqlRaw("SELECT 1");
				BekoDB_Indicator.SetIndication(true);
			}
			catch
			{
				BekoDB_Indicator.SetIndication(false);
			}
		}

		/// <summary>
		/// Инициализация контекста базы данных Plis.
		/// </summary>
		private void PlisContextInit()
		{
			try
			{
				var plisContext = new PlisContext();
				_ = plisContext.Database.ExecuteSqlRaw("SELECT 1");
				GasDb_Indicator.SetIndication(true);
			}
			catch
			{
				GasDb_Indicator.SetIndication(false);
			}
		}

		/// <summary>
		/// Инициализация контекста базы данных Geda.
		/// </summary>
		private void GedaContextInit()
		{
			try
			{
				var gedaContext = new GedaContext();
				_ = gedaContext.Database.ExecuteSqlRaw("SELECT 1");
				GasDb_Indicator.SetIndication(true);
			}
			catch
			{
				GasDb_Indicator.SetIndication(false);
			}
		}

		/// <summary>
		/// Инициализация Plc.
		/// </summary>
		private void PlcInit()
		{
			DS_Indicator.SetIndication(null);
			Action<string> waiting = Settings.DbName switch
			{
				GasDb.Plis => PlisQuery,
				GasDb.Geda => GedaQuery,
				_ => throw new Exception($"Неизвестная конфигурация gasdb {Settings.DbName}")
			};

			plc = new PlcInteraction(Settings.Config);

			plc.ConnectionStatus += (status) =>
			{
				try
				{
					Device_Indicator.SetIndication(status);
				}
				catch { }
			};

			plc.DetectionSensor += (exist) =>
			{
				try
				{
					SendToRepair = false;
					DS_Indicator.SetIndication(!exist ? null : exist);
					if (exist)
					{
						ResponseDesc.SetLabel(string.Empty);
						TestBarcode.SetLabel(string.Empty);
						ImgDone.SetVisibility(Visibility.Hidden);
						ImgError.SetVisibility(Visibility.Hidden);
						ImgQuestion.SetVisibility(Visibility.Hidden);
						ImgOut.SetVisibility(Visibility.Hidden);
					}
				}
				catch { }
			};

			plc.ButtonPressed += async (cmd, btn) =>
			{
				try
				{
					if (SendToRepair)
					{
						plc.ConveyorStart();
						return;
					}

					if (cmd == plc.config.CmdStart)
					{
						if (btn) waiting(CurrentBarcode);
						plc.ConveyorStart();
						return;
					}

					if (cmd == plc.config.CmdStartEmergency && btn)
					{
						TestBarcode.SetLabel(CurrentBarcode);
						ImgOut.SetVisibility(Visibility.Visible);
						ResponseDesc.SetLabel("Отправка вручную");
						Log.AppendMessage($"{CurrentBarcode} отправка вручную");
						await _log.WriteLogAsync($"{CurrentBarcode} отправка вручную");
					}
				}
				catch { }
			};

			plc.RepeatSend += () =>
			{
				try
				{
					if (CurrentBarcode.Length != 22 || CurrentBarcode.Count(x => char.IsDigit(x)) != CurrentBarcode.Length)
						plc.Reading(true);

					comport.WriteToPort(CurrentBarcode);
				}
				catch { }
			};

			plc.LogEvent += async (msg) =>
			{
				try
				{
					Log.AppendMessage(msg);
					await _log.WriteLogAsync(msg);
				}
				catch { }
			};

			plc.ReadBarcode += (msg) =>
			{
				try
				{
					if (string.IsNullOrEmpty(msg))
					{
						Barcode.SetLabel(string.Empty);
						Product.SetLabel(string.Empty);
						Serial.SetLabel(string.Empty);
						BarcodeReaded?.Invoke(string.Empty);
						return;
					}

					CurrentBarcode = msg;
					Barcode.SetLabel(msg);
					Product.SetLabel(msg[..10]);
					Serial.SetLabel(msg[10..]);
					BarcodeReaded?.Invoke(msg);
				}
				catch { }
			};
		}

		/// <summary>
		/// Инициализация com-портов.
		/// </summary>
		private void PortsInit()
		{
			comport = new ComportInteraction(Settings.Com);
			comport.ReadedBarcode += (msg) =>
			{
				try
				{
					if (string.IsNullOrEmpty(msg))
					{
						Barcode.SetLabel(string.Empty);
						Product.SetLabel(string.Empty);
						Serial.SetLabel(string.Empty);
						BarcodeReaded?.Invoke(string.Empty);
						return;
					}

					CurrentBarcode = msg;
					Barcode.SetLabel(msg);
					Product.SetLabel(msg[..10]);
					Serial.SetLabel(msg[10..]);
					BarcodeReaded?.Invoke(msg);
				}
				catch { }
			};

			comport.LogEvent += async (msg) =>
			{
				try
				{
					Log.AppendMessage(msg);
					await _log.WriteLogAsync(msg);
				}
				catch { }
			};
		}

		/// <summary>
		/// Инициализация таймера.
		/// </summary>
		private void TimerInit()
		{
			var timer = new System.Timers.Timer { Interval = 1000 };
			timer.Elapsed += async (s, e) =>
			{
				await Task.Run(() => lTimer.SetLabel($"{DateTime.Now:dd.MM.yyyy HH:mm:ss}"));
				if (e.SignalTime is { Hour: 0, Minute: 0, Second: 0 }) Log.ClearText();
			};
			timer.Start();
		}
	}
}