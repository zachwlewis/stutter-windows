using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Stutter.Core;
using Stutter.Core.Events;
using Stutter.Properties;
using System.Windows.Controls;
using System.Deployment.Application;

namespace Stutter.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// The iteration manager for the application.
		/// </summary>
		public StutterIteration Iteration;

		/// <summary>
		/// The user's task list.
		/// </summary>
		public List<StutterTask> Tasks;

		private Random Randomizer;

		//public static readonly DependencyProperty TaskNameRequirementProperty = DependencyProperty.Register("NewTaskName", typeof(string), typeof(MainWindow), new UIPropertyMetadata("Unnamed Task"));
		private string _ntn = "";
		public string NewTaskName
		{
			get { return _ntn; }
			set { _ntn = value; }
		}

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
				if (Iteration == null || !Iteration.Running) { GoalTextBlock.Text = "Click \"Start Stutter\" to see your next task!"; }
				else if (Iteration.IterationState == StutterTimedState.Phrase) { GoalTextBlock.Text = Iteration.Task.Name; }
				else if (Iteration.IterationState == StutterTimedState.Block) { GoalTextBlock.Text = "Take a brief break before the next phrase."; }
			}
			else { GoalTextBlock.Text = "Click \"New Task\" to add your first task!"; }
		}

		void BeginIteration()
		{
			if (!TaskListBox.HasItems)
			{
				GoalTextBlock.Text = "Add some tasks to your list with the \"New Task\" button, then try again.";
				return;
			}

			// Make a list of all incomplete tasks.
			List<StutterTask> temp = new List<StutterTask>();

			foreach(StutterTask st in Tasks)
			{
				if (!st.IsComplete) { temp.Add(st); }
			}

			// If there are no incomplete tasks, fuss.
			if (temp.Count < 1)
			{
				GoalTextBlock.Text = "Add some tasks to your list with the \"New Task\" button, then try again.";
				return;
			}

			StutterTask task = temp[Randomizer.Next(temp.Count)];
			Iteration = new StutterIteration(task);
			Iteration.Complete += Iteration_Complete;
			Iteration.Tick += Iteration_Tick;
			Iteration.BeginPhrase();
			GoalTextBlock.Text = Iteration.Task.Name;
			PhraseProgressLabel.Content = Iteration.Duration.ToString(@"mm\:ss") + " Remaining";
			BeginButton.Content = "Stop Stutter";
		}

		void EndIteration()
		{
			Iteration.Stop();
			Iteration.Complete -= Iteration_Complete;
			Iteration.Tick -= Iteration_Tick;
			PhraseProgressLabel.Content = "";
			PhraseProgressBar.Value = 0;
			BeginButton.Content = "Start Stutter";
			RefreshGoal();
		}

		void StartNextIteration()
		{
			Iteration.Complete -= Iteration_Complete;
			Iteration.Tick -= Iteration_Tick;
			Iteration = null;
			BeginIteration();
		}

		private void ResetTaskEntryArea()
		{
			TaskNameTextBox.Text = "";
			TaskDescriptionTextBox.Text = "";
			TaskEstimateTextBox.Text = "0";
			NewTaskPanel.Visibility = Visibility.Collapsed;
			RefreshTaskList();
			RefreshGoal();
			AddTaskButton.Content = "Add Task";
			CreateTaskButton.IsEnabled = false;
			AddTaskButton.Click -= AddTaskButton_CancelClick;
			AddTaskButton.Click += AddTaskButton_Click;
		}

		private void RefreshTaskList()
		{
			Tasks.Sort();
			TaskListBox.Items.Refresh();
		}

		private void CheckForUpdates()
		{
			ApplicationDeployment deploy;

			try
			{
				deploy = ApplicationDeployment.CurrentDeployment;
			}
			catch
			{
				MessageBox.Show(this, "Sorry, but Stutter can't check for updates at this time.", "No Updates Available", MessageBoxButton.OK);
				return;
			}

			if (deploy.CheckForUpdate())
			{
				UpdateCheckInfo update = deploy.CheckForDetailedUpdate();

				MessageBoxResult result = MessageBox.Show(this, "Update to Stutter " + update.AvailableVersion.ToString(4) + "?\n\nYou're currently running Stutter " + deploy.CurrentVersion.ToString(4) + ".", "Update Stutter", MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
				{
					deploy.Update();
					System.Windows.Forms.Application.Restart();
					Application.Current.Shutdown();
				}
			}
			else
			{
				MessageBox.Show(this, "You're currently running the latest version of Stutter (" + deploy.CurrentVersion.ToString(4) + ").", "No Updates Available", MessageBoxButton.OK);
			}
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
			if (Iteration != null && Iteration.Running) { EndIteration(); }
			else { BeginIteration(); }
		}

		private void Iteration_Tick(object sender, StutterTimerEvent e)
		{
			PhraseProgressBar.Value = PhraseProgressBar.Maximum * (e.Elapsed.TotalSeconds / e.Total.TotalSeconds);
			PhraseProgressLabel.Content = e.State.ToString() + " — " + e.Total.Subtract(e.Elapsed).ToString(@"mm\:ss") + " Remaining";
		}

		private void Iteration_Complete(object sender, StutterTimerEvent e)
		{
			PhraseProgressBar.Value = PhraseProgressBar.Maximum;

			System.Media.SoundPlayer player;

			switch (e.State)
			{
				case StutterTimedState.Block:
					// Handle iteration completion.
					PhraseProgressLabel.Content = "Iteration Complete";
					TryPlaySound(Properties.Resources.StartWork);

					StartNextIteration();
					break;
				case StutterTimedState.Phrase:
					// Handle phrase completion.
					// TODO: Reverse fill direction and stuff.
					TryPlaySound(Properties.Resources.StopWork);

					if (e.Task != null) { e.Task.ActualPoints++; }

					Iteration.BeginBlock();
					break;
				default:
					break;
			}

			RefreshGoal();
		}

		private void TryPlaySound(System.IO.UnmanagedMemoryStream soundResource)
		{
			if (Settings.Default.IsSoundEnabled)
			{
				System.Media.SoundPlayer player;
				player = new System.Media.SoundPlayer(soundResource);
				player.Play();
			}
		}

		private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow dialog = new SettingsWindow();
			dialog.ShowDialog();

			// Force the task list to update in case the user changed the visibility settings.
			RefreshTaskList();
		}

		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			NewTaskPanel.Visibility = Visibility.Visible;
			TaskNameTextBox.Focus();
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
		}
		
		private void MarkCompleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			task.ToggleCompletion();
			RefreshTaskList();
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			// If the iteration is currently on a phrase and the user deletes the current task,
			// stop the iteration to prevent null errors when updating the task.
			if (Iteration != null && Iteration.Task == task && Iteration.Running && Iteration.IterationState == StutterTimedState.Phrase) { EndIteration(); }

			Tasks.Remove(task);
			
			RefreshTaskList();
		}

		private void UpdateMenuItem_Click(object sender, RoutedEventArgs e) { CheckForUpdates(); }

		private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
		{
			uint estimate = 0;
			try { estimate = (uint)Convert.ToInt32(TaskEstimateTextBox.Text);}
			catch {estimate = 0;}
			Tasks.Add(new StutterTask(TaskNameTextBox.Text, TaskDescriptionTextBox.Text, estimate));
			ResetTaskEntryArea();
		}

		
		#endregion

		private void TaskNameTextBox_TextInput(object sender, TextCompositionEventArgs e)
		{
			
		}

		private void TaskNameTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			CreateTaskButton.IsEnabled = !String.IsNullOrWhiteSpace(TaskNameTextBox.Text);
		}

		
	}
}
