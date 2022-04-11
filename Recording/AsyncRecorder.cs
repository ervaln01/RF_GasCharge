namespace RF_GasCharge.Recording
{
	using System.Windows.Media;
	using System.Windows.Controls;
	using System.Windows.Shapes;
	using System.Windows.Threading;
	using System.Windows;
	using System;
	using System.Linq;

	/// <summary>
	/// Класс, предназначенный для синхронного зваимодействия с текстовыми полями формы.
	/// </summary>
	public static class AsyncRecorder
	{
		/// <summary>
		/// Диспетчер очереди элементов потока.
		/// </summary>
		private static Dispatcher _dispatcher;

		/// <summary>
		/// Инициализация полей <see cref="AsyncRecorder"/>.
		/// </summary>
		/// <param name="dispatcher">Диспетчер потока.</param>
		public static void Initialize(Dispatcher dispatcher) => _dispatcher = dispatcher;

		/// <summary>
		/// Устанавливает цвет состояния соединения.
		/// </summary>
		/// <param name="indicator">Индикатор.</param>
		/// <param name="isConnected">Состояние соединения.</param>
		public static void SetIndication(this Ellipse indicator, bool? isConnected)
		{
			if (isConnected == null)
			{
				_dispatcher.InvokeAsync(() => SafeInvoke(() => indicator.Fill = Brushes.LightGray));
				return;
			}

			_dispatcher.InvokeAsync(() => SafeInvoke(() => indicator.Fill = isConnected.Value ? Brushes.LightGreen : Brushes.Red));
		}

		/// <summary>
		/// Устанавливает видимость изображение.
		/// </summary>
		/// <param name="image">Изображение.</param>
		/// <param name="visibility">Видимость изображения.</param>
		public static void SetVisibility(this Image image, Visibility visibility) => _dispatcher.InvokeAsync(() => SafeInvoke(() => image.Visibility = visibility));

		/// <summary>
		/// Выводит сообщение в поле ввода.
		/// </summary>
		/// <param name="textBox">Поле ввода.</param>
		/// <param name="msg">Сообщение.</param>
		public static void AppendMessage(this TextBox textBox, string msg) =>
			_dispatcher.InvokeAsync(() => SafeInvoke(() =>
			textBox.Text = $"{DateTime.Now:yyyy.MM.dd HH:mm:ss} -> {msg}\n{string.Join('\n', textBox.Text.Split('\n').Take(500))}"));

		/// <summary>
		/// Очищает поле ввода.
		/// </summary>
		/// <param name="textBox">Поле ввода.</param>
		public static void ClearText(this TextBox textBox) => _dispatcher.InvokeAsync(() => SafeInvoke(() => textBox.Text = string.Empty));


		/// <summary>
		/// Устанавливает содержимое надписи.
		/// </summary>
		/// <param name="label">Надпись.</param>
		/// <param name="content">Содержимое надписи.</param>
		public static void SetLabel(this Label label, object content) => 
			_dispatcher.InvokeAsync(() => SafeInvoke(() => label.Content = content));

		private static void SafeInvoke<T>(Func<T> lambda) 
		{
			try { lambda(); } catch { }
		}
	}
}