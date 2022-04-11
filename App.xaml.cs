namespace RF_GasCharge
{
	using RF_GasCharge.Configuration;
	using RF_GasCharge.Recording;
	using System;
	using System.Reflection;
	using System.Threading;
	using System.Windows;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Мьютекс.
		/// </summary>
		private Mutex mutex;

		/// <summary>
		/// Флаг, показывающий является ли текущий процесс владельцем мьютекса.
		/// </summary>
		private bool currentProcessOwner;

		/// <summary>
		/// Запускает приложение.
		/// </summary>
		/// <param name="sender">Ссылка на объект, вызвавший событие.</param>
		/// <param name="e">Аргументы события.</param>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name, out currentProcessOwner);
			if (!currentProcessOwner)
			{
				_ = MessageBox.Show("Приложение уже запущено");
				Current.Shutdown();
				return;
			}

			try
			{
				Settings.Initialize();
				Failcodes.Initialize();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка конфигурации (app.config)");
				Current.Shutdown();
				return;
			}

			new MainWindow(new BekoLogFile()).Show();
		}

		/// <summary>
		/// Вызывает событие Application.Exit и освобождает мьютекс.
		/// </summary>
		/// <param name="e">Аргументы события.</param>
		protected override void OnExit(ExitEventArgs e)
		{
			if (currentProcessOwner) mutex.ReleaseMutex();

			try
			{
				base.OnExit(e);
			}
			catch
			{
				Current.Shutdown();
			}
		}
	}
}