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

namespace Stutter.Windows
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		private Stutter.Properties.Settings StutterSettings;

		public AboutWindow()
		{
			InitializeComponent();
			StutterSettings = Stutter.Properties.Settings.Default;

			CopyrightLabel.Content = StutterSettings.COPYRIGHT + " " + StutterSettings.AUTHOR;
			ApplicationLabel.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString() +" " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);

		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
