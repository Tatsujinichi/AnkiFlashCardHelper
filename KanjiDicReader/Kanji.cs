using System.Collections.Generic;

namespace KanjiDicReader
{
	public class Kanji
	{
		public Kanji()
		{
			Meanings = new List<string>();
			Reading = new Reading();
		}

		public string Character { get; set; }

		public Reading Reading { get; set; }

		public List<string> Meanings { get; set; }

		public int JLPT { get; set; }
	}
}
