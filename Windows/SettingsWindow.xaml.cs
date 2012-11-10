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
using System.Windows.Shapes;
using Stutter.Properties;

namespace Stutter.Windows
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			Settings s = Settings.Default;
			InitializeComponent();
			PhraseLengthSlider.Value = s.PhraseLength;;
			BlockLengthSlider.Value = s.BlockLength; ;
			PhraseLengthSlider.ValueChanged +=PhraseLengthSlider_ValueChanged;
			BlockLengthSlider.ValueChanged += BlockLengthSlider_ValueChanged;
		}

		void BlockLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Settings.Default.BlockLength = (int)Math.Round(e.NewValue);
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.Save();
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.Reload();
			Close();
		}

		private void PhraseLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Settings.Default.PhraseLength = (int)Math.Round(e.NewValue);
		}
	}
}
