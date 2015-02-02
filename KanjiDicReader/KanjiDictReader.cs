using System;
using System.Collections.Generic;
using System.Xml;

namespace KanjiDicReader
{
	public class KanjiDictReader
	{
		public HashSet<Kanji> GetAllKanji(XmlReader reader)
		{
			reader.ReadToDescendant("kanjidic2");

			var kanjis = new HashSet<Kanji>();

			while (reader.ReadToFollowing("character"))
			{
				var kanji = ReadKanji(reader.ReadSubtree());
				kanjis.Add(kanji);
			}
			return kanjis;
		}

		private static Kanji ReadKanji(XmlReader reader)
		{
			var kanji = new Kanji();

			reader.ReadToDescendant("literal");
			kanji.Character = reader.ReadElementContentAsString();

			if (reader.ReadToFollowing("misc"))
				ReadMisc(kanji, reader.ReadSubtree());

			if (reader.ReadToFollowing("reading_meaning"))
			{
				ReadReadingMeaning(kanji, reader.ReadSubtree());
			}
			reader.Close();
			return kanji;
		}

		public static void ReadMisc(Kanji kanji, XmlReader reader)
		{
			reader.ReadToDescendant("jlpt");
			if (reader.NodeType != XmlNodeType.None)
				kanji.JLPT = reader.ReadElementContentAsInt();
			reader.Close();
		}

		public static void ReadReadingMeaning(Kanji kanji, XmlReader reader)
		{
			reader.ReadToDescendant("rmgroup");
			ReadRMGroup(kanji, reader.ReadSubtree());
			reader.Close();
		}

		public static void ReadRMGroup(Kanji kanji, XmlReader reader)
		{
			//if (reader.ReadToDescendant("reading"))
			//{
			//    ReadReading(kanji, reader);
			//    while (reader.ReadToNextSibling("reading"))
			//    {
			//        ReadReading(kanji, reader);
			//    }
			//}

			//if (reader.ReadToDescendant("meaning"))
			//{
			//    if (reader.NodeType != XmlNodeType.None)
			//        kanji.Meanings.Add(reader.ReadElementContentAsString());
			//    while (reader.ReadToNextSibling("meaning"))
			//    {
			//        kanji.Meanings.Add(reader.ReadElementContentAsString());
			//    }
			//}
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.None:
						break;
					case XmlNodeType.Element:
						if (reader.Name == "reading")
						{
							if (reader.NodeType != XmlNodeType.None)
								ReadReading(kanji, reader);
						}
						else if (reader.Name == "meaning")
						{
							if (reader.NodeType != XmlNodeType.None)
						        kanji.Meanings.Add(reader.ReadElementContentAsString());
						}
						break;
					case XmlNodeType.Attribute:
						break;
					case XmlNodeType.Text:
						break;
					case XmlNodeType.CDATA:
						break;
					case XmlNodeType.EntityReference:
						break;
					case XmlNodeType.Entity:
						break;
					case XmlNodeType.ProcessingInstruction:
						break;
					case XmlNodeType.Comment:
						break;
					case XmlNodeType.Document:
						break;
					case XmlNodeType.DocumentType:
						break;
					case XmlNodeType.DocumentFragment:
						break;
					case XmlNodeType.Notation:
						break;
					case XmlNodeType.Whitespace:
						break;
					case XmlNodeType.SignificantWhitespace:
						break;
					case XmlNodeType.EndElement:
						break;
					case XmlNodeType.EndEntity:
						break;
					case XmlNodeType.XmlDeclaration:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			reader.Close();
		}

		private static void ReadReading(Kanji kanji, XmlReader reader)
		{
			if (reader.NodeType != XmlNodeType.None)
			{
				string rtype = reader.GetAttribute("r_type");
				if (rtype.Equals("ja_on"))
					kanji.Reading.On.Add(reader.ReadElementContentAsString());
				else if (rtype.Equals("ja_kun"))
					kanji.Reading.Kun.Add(reader.ReadElementContentAsString());
			}
		}
	}
}