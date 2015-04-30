using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnkiFlashCardHelper
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			DataContext = new ViewModel();
			InitializeComponent();
		}

		private void WriteFile_Click(object sender, RoutedEventArgs e)
		{
			((ViewModel)DataContext).WriteFile();
		}

		private void ClearDuplicates_Click(object sender, RoutedEventArgs e)
		{
			((ViewModel)DataContext).ClearDuplicates();
		}

		
		//private void Browse_Click(object sender, RoutedEventArgs e)
		//{
		//	((ViewModel) DataContext).LoadDictionary();
		//}
		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			((ViewModel)DataContext).WritePersistentData();
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("AnkiFlashCardHelper by John Ferreira" +
			                "\nuses JMDict." +
			                "\nhttp://www.edrdg.org/jmdict/j_jmdict.html", "About", MessageBoxButton.OK);
		}
	}
}
