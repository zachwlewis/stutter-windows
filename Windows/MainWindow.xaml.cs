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
using Stutter.Events;
using Stutter.Core;
using Stutter.Properties;

namespace Stutter.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public StutterPhrase Phrase;

		public List<StutterTask> Tasks;

		private Random Randomizer;

		public MainWindow()
		{
			InitializeComponent();
			Tasks = new List<StutterTask>();
			Tasks.Add(new StutterTask("Create iconography for Stutter.", "Desc", 10, 5));
			Tasks.Add(new StutterTask("Set Stutter to deploy via Shimmer.", "Desc", 0, 5));
			Tasks.Add(new StutterTask("Complete the Stutter task list.", "Desc", 0, 0));
			Tasks.Add(new StutterTask("Create settings window for Stutter.", "Desc", 15, 25));
			TaskListBox.ItemsSource = Tasks;
			Randomizer = new Random();
		}

		private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow dialog = new AboutWindow();
			dialog.ShowDialog();
		}

		private void BeginButton_Click(object sender, RoutedEventArgs e)
		{
			
			BeginPhrase();
			GoalTextBlock.Text = Phrase.Task.ToString();
			PhraseProgressLabel.Content = Phrase.Duration.ToString(@"mm\:ss") + " Remaining";
			BeginButton.IsEnabled = false;
		}

		void Phrase_Tick(object sender, StutterEventArgs e)
		{
			PhraseProgressBar.Value = PhraseProgressBar.Maximum * (e.Elapsed.TotalSeconds / e.Total.TotalSeconds);
			PhraseProgressLabel.Content = e.Total.Subtract(e.Elapsed).ToString(@"mm\:ss") + " Remaining";
		}

		void Phrase_Complete(object sender, StutterEventArgs e)
		{
			PhraseProgressBar.Value = PhraseProgressBar.Maximum;
			PhraseProgressLabel.Content = "Phrase Complete";
			BeginButton.IsEnabled = true;

			EndPhrase();
		}

		void BeginPhrase()
		{
			StutterTask task = Tasks[Randomizer.Next(Tasks.Count)];
			Phrase = new StutterPhrase(task, Settings.Default.PhraseLength, 0);
			Phrase.Complete += Phrase_Complete;
			Phrase.Tick += Phrase_Tick;
			Phrase.Start();
		}

		void EndPhrase()
		{
			Phrase.Complete -= Phrase_Complete;
			Phrase.Tick -= Phrase_Tick;
			Phrase = null;
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			SettingsWindow dialog = new SettingsWindow();
			dialog.ShowDialog();
		}
	}
}
