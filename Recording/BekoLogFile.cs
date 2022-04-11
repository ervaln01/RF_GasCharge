namespace RF_GasCharge.Recording
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Threading.Tasks;

	/// <summary>
	/// Класс осуществляющий методы логирования.
	/// </summary>
	public class BekoLogFile
	{
		#region Fields
		private DirectoryInfo dir;
		private FileInfo outFile;
		private StreamWriter writer; //поток для записи логов
		private int lastDay; //в полночь будем сбрасывать в новый файл
		private readonly string directoryName; //для возможности задания каталога
		#endregion Fieds

		/// <summary>
		/// Конструктор класса <see cref="BekoLogFile"/>.
		/// </summary>
		/// <param name="directory">Директория сохранения файла лога.</param>
		public BekoLogFile(string directory = @"\logs")
		{
			directoryName = directory;
			InitNewFile();
		}

		/// <summary>
		/// Деструктор класса <see cref="BekoLogFile"/>.
		/// </summary>
		~BekoLogFile()
		{
			try
			{
				writer?.Close();
			}
			catch { }
		}

		/// <summary>
		/// Запись лога.
		/// </summary>
		/// <param name="msg">Сообщение для записи в лог.</param>
		public async Task WriteLogAsync(string msg)
		{
			if (lastDay != DateTime.Now.Day)
				InitNewFile();

			await writer?.WriteLineAsync($"{DateTime.Now:dd.MM.yyyy HH:mm:ss.f} -> {msg}");
		}

		/// <summary>
		/// Инициализация нового файла.
		/// </summary>
		private void InitNewFile()
		{
			if (dir == null)
			{
				var appName = Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName;
				var folderName = appName.Substring(0, appName.LastIndexOf(@"\"));
				dir = new DirectoryInfo(folderName + directoryName);
			}

			try
			{
				writer?.Close();
			}
			catch { }

			try //далее идут операции по регистрации файлов
			{
				lastDay = DateTime.Now.Day;
				if (!dir.Exists)
					dir.Create();

				var logFiles = dir.GetFiles(); //получаем список файлов
				foreach (var logFile in logFiles)
				{
					//анализировать можно по дате создания, дате последней записи, дате последнего чтения (я взял второй вариант)
					if ((DateTime.Now - logFile.LastWriteTime).TotalDays > 30)
						logFile.Delete();
				}

				var fileName = $"{DateTime.Now:yyyy-MM-dd}.txt";
				outFile = new FileInfo(dir.FullName + "\\" + fileName);
				writer = outFile.AppendText();
				writer.AutoFlush = true; //автоматическое выталкивание из буфера в файл, каждую команду записи
			}
			catch { }
		}
	}
}