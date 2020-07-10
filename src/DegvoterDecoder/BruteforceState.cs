using System.IO;
using System.IO.MemoryMappedFiles;

namespace DegvoterDecoder
{
	/// <summary>
	/// Лень оптимизировать, и так сойдет
	/// </summary>
	public sealed class BruteforceState
	{
		private FileStream _f;
		private readonly object _locker = new object();

		private MemoryMappedFile _memoryMappedFile;
		private MemoryMappedViewAccessor _viewAccessor;

		public BruteforceState(string fileName)
		{
			var fileInfo = new FileInfo(fileName);

			bool create = false;
			if (fileInfo.Exists)
			{
				if (fileInfo.Length != 10000)
				{
					File.Delete(fileName);
					create = true;
				}
			}
			else
			{
				create = true;
			}
			if (create)
			{
				_f = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.None);
				_f.SetLength(10000);
			}
			else
			{
				_f = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.None);
			}
			_memoryMappedFile = MemoryMappedFile.CreateFromFile(_f, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);

			_viewAccessor = _memoryMappedFile.CreateViewAccessor();
			for (int i = 0; i < 10000; i++)
			{
				if (_viewAccessor.ReadByte(i) != 2)
				{
					_viewAccessor.Write(i, (byte) 0);
				}
			}
			_viewAccessor.Flush();
		}

		public int TakeSerie()
		{
			int ret = -1;
			lock (_locker)
			{
				for (int i = 0; i <10000; i++)
				{
					if (_viewAccessor.ReadByte(i) == 0)
					{
						_viewAccessor.Write(i, (byte) 1);
						ret = i;
						break;
					}
				}
			}
			return ret;
		}

		public void InformCompleted(int serie)
		{
			lock (_locker)
			{
				_viewAccessor.Write(serie, (byte) 2);
				_viewAccessor.Flush();
			}
		}
	}
}
