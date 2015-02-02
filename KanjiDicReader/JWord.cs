using System.Collections.Generic;

namespace KanjiDicReader
{
	public class JWord
	{
		public JWord()
		{
			Meanings = new List<string>();
		}

		public string Word { get; set; }

		public string Reading { get; set; }

		public List<string> Meanings { get; set; }

		public int JLPT { get; set; } 
	}
}