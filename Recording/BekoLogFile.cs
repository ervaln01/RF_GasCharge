namespace RF_GasCharge.Recording
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Threading.Tasks;

	/// <summary>
	/// ����� �������������� ������ �����������.
	/// </summary>
	public class BekoLogFile
	{
		#region Fields
		private DirectoryInfo dir;
		private FileInfo outFile;
		private StreamWriter writer; //����� ��� ������ �����
		private int lastDay; //� ������� ����� ���������� � ����� ����
		private readonly string directoryName; //��� ����������� ������� ��������
		#endregion Fieds

		/// <summary>
		/// ����������� ������ <see cref="BekoLogFile"/>.
		/// </summary>
		/// <param name="directory">���������� ���������� ����� ����.</param>
		public BekoLogFile(string directory = @"\logs")
		{
			directoryName = directory;
			InitNewFile();
		}

		/// <summary>
		/// ���������� ������ <see cref="BekoLogFile"/>.
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
		/// ������ ����.
		/// </summary>
		/// <param name="msg">��������� ��� ������ � ���.</param>
		public async Task WriteLogAsync(string msg)
		{
			if (lastDay != DateTime.Now.Day)
				InitNewFile();

			await writer?.WriteLineAsync($"{DateTime.Now:dd.MM.yyyy HH:mm:ss.f} -> {msg}");
		}

		/// <summary>
		/// ������������� ������ �����.
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

			try //����� ���� �������� �� ����������� ������
			{
				lastDay = DateTime.Now.Day;
				if (!dir.Exists)
					dir.Create();

				var logFiles = dir.GetFiles(); //�������� ������ ������
				foreach (var logFile in logFiles)
				{
					//������������� ����� �� ���� ��������, ���� ��������� ������, ���� ���������� ������ (� ���� ������ �������)
					if ((DateTime.Now - logFile.LastWriteTime).TotalDays > 30)
						logFile.Delete();
				}

				var fileName = $"{DateTime.Now:yyyy-MM-dd}.txt";
				outFile = new FileInfo(dir.FullName + "\\" + fileName);
				writer = outFile.AppendText();
				writer.AutoFlush = true; //�������������� ������������ �� ������ � ����, ������ ������� ������
			}
			catch { }
		}
	}
}