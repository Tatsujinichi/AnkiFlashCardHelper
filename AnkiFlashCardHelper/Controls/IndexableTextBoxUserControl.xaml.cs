using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnkiFlashCardHelper.Controls
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class IndexableTextBoxUserControl : UserControl
	{
		//need  Text 
		public IndexableTextBoxUserControl()
		{
			InitializeComponent();
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		//TODO: add default two way binding
		//TODO: expose other properties instead of setting them directly in xaml
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(IndexableTextBoxUserControl), new PropertyMetadata(string.Empty));

		public int CaretPosition
		{
			get { return (int)GetValue(CaretPositionProperty); }
			set { SetValue(CaretPositionProperty, value); }
		}

		public static readonly DependencyProperty CaretPositionProperty =
			DependencyProperty.Register("CaretPosition", typeof(int), typeof(IndexableTextBoxUserControl), new FrameworkPropertyMetadata(PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = d as IndexableTextBoxUserControl;
			if (obj != null)
			{
				obj.TextBox0.CaretIndex = (int)e.NewValue;
				obj.TextBox0.Focus();
				int line = obj.TextBox0.GetLineIndexFromCharacterIndex(obj.TextBox0.CaretIndex);
				obj.TextBox0.ScrollToLine(line);
			}
		}

	}
}
