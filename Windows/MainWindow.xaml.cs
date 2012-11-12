using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Stutter.Core;
using Stutter.Core.Events;
using Stutter.Properties;
using System.Windows.Controls;

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

		public bool IsTaskListVisible
		{
			get { return Settings.Default.IsTaskListVisible; }
			set
			{
				Settings.Default.IsTaskListVisible = value;
				Settings.Default.Save();
				TaskListColumnDefinition.Width = value ? new GridLength(160, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);	
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			// Load our previous task list.
			Tasks = StutterIO.LoadTaskListFromXML(Settings.Default.LastTaskListFilename);
			TaskListBox.ItemsSource = Tasks;

			RefreshGoal();

			// TODO: Is this the best way to update this value?
			IsTaskListVisible = IsTaskListVisible;

			Randomizer = new Random();
		}

		private void RefreshGoal()
		{
			if (TaskListBox.HasItems)
			{
				if (Phrase != null && Phrase.Running) { GoalTextBlock.Text = Phrase.Task.Name; }
				else { GoalTextBlock.Text = "Click \"Begin Phrase\" to see your next task!"; }
			}
			else { GoalTextBlock.Text = "Click \"Add Task\" to add your first task!"; }
		}

		void BeginPhrase()
		{
			if (!TaskListBox.HasItems)
			{
				GoalTextBlock.Text = "Add some tasks to your list with the Add Task button, then try again.";
				return;
			}
			StutterTask task = Tasks[Randomizer.Next(Tasks.Count)];
			Phrase = new StutterPhrase(task, Settings.Default.PhraseLength, 0);
			Phrase.Complete += Phrase_Complete;
			Phrase.Tick += Phrase_Tick;
			Phrase.Start();
			GoalTextBlock.Text = Phrase.Task.ToString();
			PhraseProgressLabel.Content = Phrase.Duration.ToString(@"mm\:ss") + " Remaining";
			BeginButton.IsEnabled = false;
		}

		void EndPhrase()
		{
			Phrase.Complete -= Phrase_Complete;
			Phrase.Tick -= Phrase_Tick;
			Phrase = null;
		}

		private void ResetTaskEntryArea()
		{
			TaskEntryTextBox.Text = "";
			TaskEntryTextBox.Visibility = Visibility.Collapsed;
			TaskListBox.Items.Refresh();
			RefreshGoal();
			AddTaskButton.Content = "Add Task";
			AddTaskButton.Click -= AddTaskButton_CancelClick;
			AddTaskButton.Click += AddTaskButton_Click;
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			Tasks.Remove(task);
			TaskListBox.Items.Refresh();
		}

		#region UI Events

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
			RefreshGoal();
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
			RefreshGoal();
		}

		private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow dialog = new SettingsWindow();
			dialog.ShowDialog();

			// Force the task list to update in case the user changed the visibility settings.
			TaskListBox.Items.Refresh();
		}

		private void TaskEntryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && TaskEntryTextBox.Text.Length > 0)
			{
				Tasks.Add(new StutterTask(TaskEntryTextBox.Text));
				ResetTaskEntryArea();
			}
		}

		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			TaskEntryTextBox.Visibility = Visibility.Visible;
			TaskEntryTextBox.Focus();
			TaskEntryTextBox.Text = "Name your new task, then press Enter.";
			TaskEntryTextBox.SelectAll();
			AddTaskButton.Content = "Cancel";
			AddTaskButton.Click -= AddTaskButton_Click;
			AddTaskButton.Click += AddTaskButton_CancelClick;
		}

		private void AddTaskButton_CancelClick(object sender, RoutedEventArgs e)
		{
			ResetTaskEntryArea();
		}

		private void StutterMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			StutterIO.SaveTaskListToXML(Tasks, Settings.Default.LastTaskListFilename);

			// TODO: If the phrase is more than half over, update the phrase points for the current task.
			// TODO: Handle task deletion.
		}

		#endregion

		
	}
}
