using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace KanjiDicReader
{
	class Program
	{
		static void Main(string[] args)
		{
			//ParseKanjiDict2(@"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\kanjidic2.xml", @"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\kanjidic2x.xsd");
			ParseJMDict(@"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\JMDict\JMdict_e.gz");
		}

		private static void ParseJMDict(string file)
		{
			var settings = new XmlReaderSettings
			{
				ValidationType = ValidationType.DTD,
				DtdProcessing = DtdProcessing.Parse,
			};
			XmlReader reader = XmlReader.Create(file, settings);
			var dictReader = new JMDictReader();
			var hashsetOfWords = dictReader.GetAllWords(reader);
			var ankiImportCreator = new AnkiImportCreator(3, 3);
			using (var fs = new FileStream(@"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\JMDict\jmdict-out.csv", FileMode.Create, FileAccess.Write))
			{
				ankiImportCreator.WriteImportStream(fs, hashsetOfWords);
			}

		}

		private static void ParseKanjiDict2(string file, string xsdFile)
		{
			var schemaSet = new XmlSchemaSet();
			schemaSet.ValidationEventHandler += ValidationCallback;
			schemaSet.Add("http://www.kanjidic2x.xsd", xsdFile);
			schemaSet.Compile();

			var settings = new XmlReaderSettings
			{
				Schemas = schemaSet,
				ValidationType = ValidationType.Schema,
				DtdProcessing = DtdProcessing.Parse,
			};

			XmlReader reader = XmlReader.Create(file, settings);

			var dictReader = new KanjiDictReader();
			HashSet<Kanji> kanjis = dictReader.GetAllKanji(reader);

			var kanjiLevelsDictionary = new Dictionary<int, List<Kanji>>
			{
				{0, new List<Kanji>()},
				{1, new List<Kanji>()},
				{2, new List<Kanji>()},
				{3, new List<Kanji>()},
				{4, new List<Kanji>()},
				{5, new List<Kanji>()}
			};

			foreach (Kanji kanji in kanjis)
			{
				kanjiLevelsDictionary[kanji.JLPT].Add(kanji);
			}

			for (int i = 0; i < kanjiLevelsDictionary.Count; i++)
			{
				var ankiWriter = new AnkiImportCreator(3, 3);

				using (
					var fs = new FileStream(@"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\kanjidic2-" + i + ".csv", FileMode.Create,
						FileAccess.Write))
				{
					ankiWriter.WriteImportStream(fs, kanjiLevelsDictionary[i]);
				}
			}
		}

		static void Validate()
		{
			var schemaSet = new XmlSchemaSet();
			schemaSet.ValidationEventHandler += ValidationCallback;
			schemaSet.Add("http://www.kanjidic2x.xsd", @"C:\dev\KanjiDicReader\KanjiDicReader\dictionary\kanjidic2x.xsd");
			schemaSet.Compile();

			XmlSchema kanjiSchema = null;
			foreach (XmlSchema schema in schemaSet.Schemas())
			{
				kanjiSchema = schema;
			}

			foreach (XmlSchemaElement element in kanjiSchema.Elements.Values)
			{

				Console.WriteLine("Element: {0}", element.Name);

				// Get the complex type of the Customer element.
				var complexType = element.ElementSchemaType as XmlSchemaComplexType;

				// If the complex type has any attributes, get an enumerator  
				// and write each attribute name to the console. 
				if (complexType != null)
				{
					if (complexType.AttributeUses.Count > 0)
					{
						IDictionaryEnumerator enumerator =
							complexType.AttributeUses.GetEnumerator();

						while (enumerator.MoveNext())
						{
							var attribute = (XmlSchemaAttribute) enumerator.Value;

							Console.WriteLine("Attribute: {0}", attribute.Name);
						}
					}

					// Get the sequence particle of the complex type.
					var sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

					// Iterate over each XmlSchemaElement in the Items collection. 
					if (sequence != null)
					{
						foreach (var o in sequence.Items)
						{
							var childElement = (XmlSchemaElement) o;
							Console.WriteLine("Element: {0}", childElement.Name);
						}
					}
				}
			}
			schemaSet.ValidationEventHandler -= ValidationCallback;
		}

		static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Warning)
				Console.Write("WARNING: ");
			else if (args.Severity == XmlSeverityType.Error)
				Console.Write("ERROR: ");

			Console.WriteLine(args.Message);
		}
	}
}
