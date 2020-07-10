using System;
using System.IO;

namespace DegvoterDecoder
{
	public sealed class FileResultLog
		:IResultLog,IDisposable
	{
		private readonly object _locker = new object();
		private StreamWriter _writer;

		public FileResultLog(string fileName)
		{
			_writer=File.AppendText(fileName);
		}

		public void Dispose()
		{
			_writer.Flush();
			_writer.Dispose();
		}

		public void InformFound(int serie, int number)
		{
			lock (_locker)
			{
				var passport = string.Format("{0:D4} {1:D6}", serie, number);
				_writer.WriteLine(passport);
				_writer.Flush();
			}
		}
	}
}
