using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace KanjiDicReader
{
	public class JMDictReader
	{
		public HashSet<JWord> GetAllWords(XmlReader reader)
		{
			var words = new HashSet<string>();
			var uniqueWords = new HashSet<JWord>();

			//reader.ReadToDescendant("JMdict");
			reader.MoveToContent();
			reader.ReadToDescendant("entry");
			while (reader.ReadToNextSibling("entry"))
			{
				JWord jword = ReadKanji(reader.ReadSubtree());
				if(!string.IsNullOrEmpty(jword.Word))
					if (words.Add(jword.Word))
					{
						uniqueWords.Add(jword);
					}
			}
			return uniqueWords;
		}

		private JWord ReadKanji(XmlReader reader)
		{
			var jword = new JWord();

			if (reader.ReadToDescendant("k_ele"))
			{
				ReadKEle(jword, reader.ReadSubtree());

				if (reader.ReadToNextSibling("r_ele"))
				{
					ReadREle(jword, reader.ReadSubtree());
				}
				if (reader.ReadToNextSibling("sense"))
				{
					ReadSense(jword, reader.ReadSubtree());
				}
			}
			reader.Close();
			return jword;
		}

		private void ReadSense(JWord jword, XmlReader reader)
		{
			while (reader.ReadToDescendant("gloss"))
			{
				jword.Meanings.Add(reader.ReadElementContentAsString());
			}
			while (reader.ReadToNextSibling("gloss"))
			{
				jword.Meanings.Add(reader.ReadElementContentAsString());
			}
			reader.Close();
		}

		private static void ReadKEle(JWord jword, XmlReader reader)
		{
			reader.ReadToDescendant("keb");
			jword.Word = reader.ReadElementContentAsString();
			reader.Close();
		}

		private void ReadREle(JWord kanji, XmlReader reader)
		{
			reader.ReadToDescendant("reb");
			kanji.Reading = reader.ReadElementContentAsString();
			reader.Close();
		}
	}
}