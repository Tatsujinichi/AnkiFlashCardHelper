using System.Collections.Generic;

namespace KanjiDicReader
{
	public class Reading
	{
		public Reading()
		{
			On = new List<string>();
			Kun = new List<string>();
		}
		public List<string> On { get; set; }
		public List<string> Kun { get; set; }
	}
}