using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AnkiFlashCardHelper.Annotations;
using KanjiDicReader;

namespace AnkiFlashCardHelper
{
	public class ViewModel : INotifyPropertyChanged
	{
		private string _input;
		private string _output;
		private string _dictionaryFile;
		private string _outputFile;
		private string _title;
		private int _maxReadings;
		private int _maxMeanings;
		private bool _loaded;
		private ObservableCollection<string> _notFoundWords;
		private int _inputCount;
		private int _outputCount;
		private int _notFoundWordsCount;
		private ObservableCollection<string> _duplicates;
		private string _selectedNotFoundWord;

		public string Input
		{
			get { return _input; }
			set
			{
				_input = value;
				Update();
				OnPropertyChanged();
			}
		}

		public ObservableCollection<string> NotFoundWords
		{
			get { return _notFoundWords; }
			set
			{
				_notFoundWords = value;
				OnPropertyChanged();
			}
		}

		public string Output
		{
			get { return _output; }
			set
			{
				_output = value;
				OnPropertyChanged();
			}
		}

		public string DictionaryFile
		{
			get { return _dictionaryFile; }
			set
			{
				_dictionaryFile = value;
				OnPropertyChanged();
			}
		}

		public string OutputFile
		{
			get { return _outputFile; }
			set
			{
				_outputFile = value;
				OnPropertyChanged();
			}
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		public int MaxReadings
		{
			get { return _maxReadings; }
			set
			{
				_maxReadings = value;
				OnPropertyChanged();
				Update();
			}
		}

		public int MaxMeanings
		{
			get { return _maxMeanings; }
			set
			{
				_maxMeanings = value;
				OnPropertyChanged();
				Update();
			}
		}

		public bool Loaded
		{
			get { return _loaded; }
			set
			{
				_loaded = value;
				OnPropertyChanged();
			}
		}

		public int InputCount
		{
			get { return _inputCount; }
			set
			{
				_inputCount = value;
				OnPropertyChanged();
			}
		}

		public int OutputCount
		{
			get { return _outputCount; }
			set
			{
				_outputCount = value;
				OnPropertyChanged();
			}
		}

		public int NotFoundWordsCount
		{
			get { return _notFoundWordsCount; }
			set
			{
				_notFoundWordsCount = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<string> Duplicates
		{
			get { return _duplicates; }
			set
			{
				_duplicates = value;
				OnPropertyChanged();
			}
		}

		public string SelectedNotFoundWord
		{
			get { return _selectedNotFoundWord; }
			set
			{
				_selectedNotFoundWord = value;
				OnPropertyChanged();
				HighlightNotFoundWord();
			}
		}

		public Dictionary<string, JWord> JWords { get; set; }
		public List<JWord> Matches { get; set; }
		public List<int> AvailableMaxReadings { get; set; }
		public List<int> AvailableMaxMeanings { get; set; }
		

		public ViewModel()
		{
			Title = "AnkiCardImport Maker, using JMDict ....Loading....";
			Input = string.Empty;
			Output = string.Empty;
			DictionaryFile = "KanjiDicReader.JMdict_e.gz";
			OutputFile = @"C:\AnkiTest\test.csv";
			AvailableMaxReadings = new List<int> { 1, 2, 3, 4, 5, 6 };
			AvailableMaxMeanings = new List<int> { 1, 2, 3, 4, 5, 6 };
			Duplicates = new ObservableCollection<string>();
			MaxMeanings = 3;
			MaxReadings = 3;
			LoadDictionary();
		}

		//TODO: move biz logic out of view model
		private async void LoadDictionary()
		{
			await Task.Factory.StartNew(() =>
			{
				var settings = new XmlReaderSettings
				{
					ValidationType = ValidationType.DTD,
					DtdProcessing = DtdProcessing.Parse,
				};
				Assembly a = Assembly.GetAssembly(typeof(AnkiImportCreator));
				using (Stream stream = a.GetManifestResourceStream(DictionaryFile))
				{
					Debug.Assert(stream != null, "stream != null");
					XmlReader reader = XmlReader.Create(stream, settings);
					var dictReader = new JMDictReader();
					JWords = dictReader.GetAllWords(reader).ToDictionary(e => e.Word, e => e);
				}
			});
			Loaded = true;
			Title = "AnkiCardImport Maker, using JMDict";
			Update();
		}

		private void Update()
		{
			if (Loaded)
			{
				string[] inputWords = ParseInputWords();
				var ankiImportCreator = new AnkiImportCreator(MaxMeanings, MaxReadings);
				Matches = new List<JWord>();
				NotFoundWords = new ObservableCollection<string>();
				var alreadyFound = new HashSet<string>();
				NotFoundWordsCount = 0;
				OutputCount = 0;
				Duplicates = new ObservableCollection<string>();
				InputCount = 0;
				foreach (string inputWord in inputWords)
				{
					if (JWords.ContainsKey(inputWord))
					{
						if (alreadyFound.Add(inputWord)) // prevent duplicates
						{
							Matches.Add(JWords[inputWord]);
							OutputCount++;
						}
						else
						{
							Duplicates.Add(inputWord);
						}
					}
					else
					{
						NotFoundWords.Add(inputWord);
						NotFoundWordsCount++;
					}
					InputCount++;
				}

				byte[] bytes;
				using (var ms = new MemoryStream())
				{
					ankiImportCreator.WriteImportStream(ms, Matches);
					bytes = ms.ToArray();
				}
				using (var ms = new MemoryStream(bytes))
				{
					using (var streamReader = new StreamReader(ms))
					{
						Output = streamReader.ReadToEnd();
					}
				}
			}
		}

		private string[] ParseInputWords()
		{
			string[] inputWords = Input.Split(new[] {Environment.NewLine, "\t","　", " ", ",", "、", ";", "/", "\\"}, // that double spaced space...
				StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < inputWords.Length; i++)
			{
				string inputWord = inputWords[i].Trim(new[] { '\t', '　', ' ', ',', '、', ';', '/', '\\' });
				inputWords[i] = inputWord;
			}
			return inputWords;
		}

		internal void ClearDuplicates()
		{
			var inputWords = new List<string>(Input.Split(new[] { Environment.NewLine, "\t", "　", " ", ",", "、", ";", "/", "\\" }, StringSplitOptions.RemoveEmptyEntries));
			foreach (string duplicate in Duplicates)
			{
				inputWords.Remove(duplicate);
			}
			Input = string.Join(Environment.NewLine, inputWords);
		}

		private void HighlightNotFoundWord()
		{
			
		}

		internal void WriteFile()
		{
			string dir = Path.GetDirectoryName(OutputFile);
			if (!string.IsNullOrEmpty(dir))
			{
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				using (var fs = new FileStream(OutputFile, FileMode.Create, FileAccess.Write))
				{
					//TODO: // maybe just write the output field since we already called this
					var ankiImportCreator = new AnkiImportCreator(MaxMeanings, MaxReadings);
					ankiImportCreator.WriteImportStream(fs, Matches);
				}
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}