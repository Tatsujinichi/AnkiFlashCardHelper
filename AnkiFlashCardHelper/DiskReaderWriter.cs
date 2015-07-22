using System;
using System.IO;

namespace AnkiFlashCardHelper
{
	public sealed class DiskReaderWriter
	{
		private readonly string _path;

		public DiskReaderWriter(string path)
		{
			_path = path;
		}
		
		public void WriteToDisk(string data)
		{
			using (var fs = new FileStream(_path, FileMode.Create, FileAccess.ReadWrite))
			{
				using (var sw = new StreamWriter(fs))
				{
					sw.Write(data);
				}
			}
		}

		public string ReadFromDisk()
		{
			using (var sr = new StreamReader(new FileStream(_path, FileMode.OpenOrCreate)))
			{
				return sr.ReadToEnd();
			}
		}

	}
}