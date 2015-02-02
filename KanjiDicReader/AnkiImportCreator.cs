using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KanjiDicReader
{
	public class AnkiImportCreator
	{
		private readonly int _maxMeanings;
		private readonly int _maxReadings;

		public AnkiImportCreator(int maxMeanings, int maxReadings)
		{
			_maxMeanings = maxMeanings;
			_maxReadings = maxReadings;
		}

		public void WriteImportStream(Stream stream, IEnumerable<JWord> jwords)
		{
			using (var sw = new StreamWriter(stream, Encoding.UTF8))
			{
				foreach (JWord jword in jwords)
				{
					WriteJWord(jword, sw);
				}
			}
		}

		public void WriteImportStream(Stream stream, IEnumerable<Kanji> kanjis)
		{
			using (var sw = new StreamWriter(stream, Encoding.UTF8))
			{
				foreach (Kanji kanji in kanjis)
				{
					WriteKanji(kanji, sw);
				}
			}
		}

		private void WriteJWord(JWord jword, StreamWriter sw)
		{
			var sb = new StringBuilder();

			sb.Append(jword.Word);
			sb.Append(";");
			for (int i = 0; i < jword.Meanings.Count && i < _maxMeanings; i++)
			{
				string meaning = jword.Meanings[i];
				sb.Append(meaning);
				int min = Math.Min(jword.Meanings.Count, _maxMeanings);			// control extra commas
				if (i < min - 1)
					sb.Append(", ");
			}
			sb.Append(";");
			sb.Append(jword.Reading);

			sw.WriteLine(sb.ToString());
		}

		private void WriteKanji(Kanji kanji, StreamWriter sw)
		{
			var sb = new StringBuilder();
			sb.Append(kanji.Character);
			sb.Append(";");
			for (int i = 0; i < kanji.Meanings.Count && i < _maxMeanings; i++)
			{
				string meaning = kanji.Meanings[i];
				sb.Append(meaning);
				int min = Math.Min(kanji.Meanings.Count, _maxMeanings);			// control extra commas
				if (i < min - 1)
					sb.Append(", ");
			}
			sb.Append(";");
			for (int i = 0; i < kanji.Reading.Kun.Count && i < _maxReadings; i++)
			{
				string kunReading = kanji.Reading.Kun[i];
				sb.Append(kunReading);
				int min = Math.Min(kanji.Reading.Kun.Count, _maxReadings);
				if (i < min - 1)
					sb.Append(", ");
			}
			sb.Append(";");
			for (int i = 0; i < kanji.Reading.On.Count && i < _maxReadings; i++)
			{
				string onReading = kanji.Reading.On[i];
				sb.Append(onReading);
				int min = Math.Min(kanji.Reading.On.Count, _maxReadings);
				if (i < min - 1)
					sb.Append(", ");
			}
			sb.Append(";");
			sb.Append(kanji.JLPT);
			sw.WriteLine(sb.ToString());
		}
	}
}
