using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Stutter.Core;
using Stutter.Core.Events;
using Stutter.Properties;
using Stutter.Windows.Events;
using System.ComponentModel;
using System.Windows.Shell;

namespace Stutter.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		/// <summary>The iteration manager for the application.</summary>
		public StutterIteration Iteration;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>The user's task list.</summary>
		public List<StutterTask> Tasks;

		/// <summary>Is the task list visible?</summary>
		public bool IsTaskListVisible
		{
			get { return Settings.Default.IsTaskListVisible || _tmm.Mode != TaskMode.Closed; }
			set
			{
				Settings.Default.IsTaskListVisible = value;
				Settings.Default.Save();
				OnPropertyChanged("IsTaskListVisible");
			}
		}

		/// <summary>Are completed tasks visibile in the task list?</summary>
		public bool AreCompletedTasksVisible
		{
			get { return Settings.Default.AreCompletedTasksVisible; }
			set
			{
				Settings.Default.AreCompletedTasksVisible = value;
				Settings.Default.Save();
				OnPropertyChanged("AreCompletedTasksVisible");
			}
		}

		/// <summary>Is the task creation/edit area closed?</summary>
		public bool IsTaskAreaClosed
		{
			get { return _tmm.Mode == TaskMode.Closed; }
		}

		private void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) { handler(this, new PropertyChangedEventArgs(name)); }
		}

		private TaskModeManager _tmm = new TaskModeManager(TaskMode.Closed);
		private Random _random;

		private bool _doesHideOnMinimize;
		public bool DoesHideOnMinimize
		{
			get { return _doesHideOnMinimize; }
			set
			{
				_doesHideOnMinimize = value;
				if (value) { NotificationHelper.Enable(this); }
				else { NotificationHelper.Disable(this); }
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
			TaskListMenuItem.IsChecked = Settings.Default.IsTaskListVisible;
			OnPropertyChanged("IsTaskListVisible");

			_tmm.Changed += TaskMode_Changed;

			_random = new Random();

			Settings.Default.SettingsLoaded += SettingsLoaded;
			Settings.Default.SettingsSaving += SettingsSaving;

			// Handle minimization toggling.
			DoesHideOnMinimize = Settings.Default.DoesHideOnMinimize;
		}

		private void SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
		{
			// Update anything based on our new settings.
			DoesHideOnMinimize = Settings.Default.DoesHideOnMinimize;
		}

		private void SettingsSaving(object sender, CancelEventArgs e)
		{
			// Update anything based on our new settings.
			DoesHideOnMinimize = Settings.Default.DoesHideOnMinimize;
		}

		private void RefreshGoal()
		{
			if (TaskListBox.HasItems)
			{
				if (Iteration == null || !Iteration.Running)
				{
					GoalTextBlock.Text = "Click \"Start Stutter\" to see your next task!";
					GoalDescriptionBlock.Text = "To keep you on your toes, any incomplete task could be next.";
				}
				else if (Iteration.IterationState == StutterTimedState.Phrase)
				{
					GoalTextBlock.Text = Iteration.Task.Name;
					if (String.IsNullOrWhiteSpace(Iteration.Task.Description))
					{
						GoalDescriptionBlock.Visibility = System.Windows.Visibility.Collapsed;
						GoalDescriptionBlock.Text = ""; 
					}
					else
					{
						GoalDescriptionBlock.Visibility = System.Windows.Visibility.Visible;
						GoalDescriptionBlock.Text = Iteration.Task.Description;
					}

				}
				else if (Iteration.IterationState == StutterTimedState.Block)
				{
					GoalTextBlock.Text = "Take a brief break before the next phrase.";
					GoalDescriptionBlock.Text = "Make sure to mark a task as complete if you completed it during this phrase. Just double-click on the task to mark it complete.";
				}
			}
			else {
				GoalTextBlock.Text = "Click \"New Task\" to add your first task!";
				GoalDescriptionBlock.Text = "Be sure to give your tasks clear, concise names. Leave the detail for the description.";
			}
		}

		#region Iteration Handling

		private void BeginIteration()
		{
			if (!TaskListBox.HasItems)
			{
				GoalTextBlock.Text = "Add some tasks to your list with the \"New Task\" button, then try again.";
				GoalDescriptionBlock.Text = "Be sure to give your tasks clear, concise names. Leave the detail for the description.";
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
				GoalDescriptionBlock.Text = "Remember, completed tasks won't be selected during a phrase.";
				return;
			}

			// Set up variables for the new Iteration
			StutterTask task = temp[_random.Next(temp.Count)];
			TimeSpan phraseLength = new TimeSpan(0, Settings.Default.PhraseLength, 0);
			TimeSpan blockLength = new TimeSpan(0, Settings.Default.BlockLength, 0);

#if DEBUG

			// Shorter iterations for debugging purposes.
			phraseLength = new TimeSpan(0,0,10);
			blockLength = new TimeSpan(0,0,5);
			
#endif

			// Create the Iteration.
			Iteration = new StutterIteration(task, phraseLength, blockLength);
			Iteration.Complete += Iteration_Complete;
			Iteration.Tick += Iteration_Tick;
			Iteration.BeginPhrase();
			RefreshGoal();
			PhraseProgressLabel.Content = Iteration.PhraseLength.ToString(@"mm\:ss") + " Remaining";
			BeginButton.Content = "Stop Stutter";

			NotificationHelper.CreateNotificiation(this, Iteration.Task.Name, "Phrase Started");
		}

		private void EndIteration()
		{
			Iteration.Stop();
			Iteration.Complete -= Iteration_Complete;
			Iteration.Tick -= Iteration_Tick;
			PhraseProgressLabel.Content = "";
			PhraseProgressBar.Value = 0;
			BeginButton.Content = "Start Stutter";
			RefreshGoal();

			// When the user stops the iteration, clear the taskbar progressRatio.
			TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
			TaskbarItemInfo.ProgressValue = 0.0;
		}

		private void StartNextIteration()
		{
			Iteration.Complete -= Iteration_Complete;
			Iteration.Tick -= Iteration_Tick;
			Iteration = null;
			BeginIteration();
		}

		private void Iteration_Tick(object sender, StutterTimerEvent e)
		{
			double progressRatio = e.Elapsed.TotalSeconds / e.Total.TotalSeconds;

			// Update the progress bar in the application.
			PhraseProgressBar.Value = PhraseProgressBar.Maximum * progressRatio;

			// Update the progress bar in the taskbar. [0-1]
			// Display green during a phrase and red during a block.
			TaskbarItemInfo.ProgressState = (e.State == StutterTimedState.Phrase) ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.Error;
			TaskbarItemInfo.ProgressValue = progressRatio;

			// Update the text string.
			PhraseProgressLabel.Content = e.State.ToString() + " — " + e.Total.Subtract(e.Elapsed).ToString(@"mm\:ss") + " Remaining";
		}

		private void Iteration_Complete(object sender, StutterTimerEvent e)
		{
			PhraseProgressBar.Value = PhraseProgressBar.Maximum;

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

					Iteration.BeginBlock();

					// Display a visual alert if the user has hidden the application.
					NotificationHelper.CreateNotificiation(this, "The current phrase has ended. Take a short break.", "Phrase Ended");

					// Update the task list with new values.
					RefreshTaskList();

					break;
				default:
					break;
			}

			RefreshGoal();
		}

		#endregion

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

		#region Task Entry Area Events

		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			switch (_tmm.Mode)
			{
				case TaskMode.Closed:
					// Show the new task panel and set up for adding a task.
					_tmm.Mode = TaskMode.Create;
					break;

				default:
					// We just want to cancel everything.
					_tmm.Mode = TaskMode.Closed;
					break;
			}
		}

		private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
		{
			uint estimate = 0;
			try { estimate = (uint)Convert.ToInt32(TaskEstimateTextBox.Text); }
			catch { estimate = 0; }

			if (_tmm.Mode == TaskMode.Create) { Tasks.Add(new StutterTask(TaskNameTextBox.Text, TaskDescriptionTextBox.Text, estimate)); }
			else if (_tmm.Mode == TaskMode.Edit)
			{
				_tmm.Task.Name = TaskNameTextBox.Text;
				_tmm.Task.Description = TaskDescriptionTextBox.Text;
				_tmm.Task.EstimatedPoints = estimate;
			}

			// Close the task drawer.
			_tmm.Mode = TaskMode.Closed;
		}

		private void TaskNameTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			CreateTaskButton.IsEnabled = !String.IsNullOrWhiteSpace(TaskNameTextBox.Text);
		}

		private void CloseTaskArea()
		{
			NewTaskPanel.Visibility = Visibility.Collapsed;
			AddTaskButton.Content = "Add Task";

			RefreshTaskList();
			RefreshGoal();
		}

		private void OpenTaskAreaForCreation()
		{
			// Show the new task panel and set up for adding a task.
			NewTaskPanel.Visibility = Visibility.Visible;
			TaskNameTextBox.Focus();
			TaskNameTextBox.Text = "";
			TaskDescriptionTextBox.Text = "";
			TaskEstimateTextBox.Text = "";
			AddTaskButton.Content = "Cancel";
			CreateTaskButton.Content = "Create Task";
			CreateTaskButton.IsEnabled = false;
		}

		private void OpenTaskAreaForEditing()
		{
			NewTaskPanel.Visibility = Visibility.Visible;

			// 1.
			CreateTaskButton.Content = "Edit Task";
			AddTaskButton.Content = "Cancel";

			// 2.
			TaskNameTextBox.Text = _tmm.Task.Name;
			TaskDescriptionTextBox.Text = _tmm.Task.Description;
			TaskEstimateTextBox.Text = _tmm.Task.EstimatedPoints.ToString();
			
			// TODO: Provide a field to edit the actual points.

			CreateTaskButton.IsEnabled = true;
		}

		private void TaskMode_Changed(object sender, TaskModeEventArgs e)
		{
			switch (e.Mode)
			{
				case TaskMode.Closed:
					CloseTaskArea();
					break;
				case TaskMode.Create:
					OpenTaskAreaForCreation();
					break;
				case TaskMode.Edit:
					OpenTaskAreaForEditing();
					break;
				default:
					break;
			}

			OnPropertyChanged("IsTaskListVisible");
			OnPropertyChanged("IsTaskAreaClosed");

			CancelTaskButton.Visibility = (e.Mode != TaskMode.Closed) ? Visibility.Visible : Visibility.Collapsed;
			AddTaskButton.Visibility = (e.Mode == TaskMode.Closed) ? Visibility.Visible : Visibility.Collapsed;
		}

		#endregion

		#region Commands

		private void AddTaskCommand_Executed(object sender, ExecutedRoutedEventArgs e) { _tmm.Mode = TaskMode.Create; }

		private void AddTaskCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = (_tmm.Mode == TaskMode.Closed); }

		private void CloseTaskCommand_Executed(object sender, ExecutedRoutedEventArgs e) { _tmm.Mode = TaskMode.Closed; }

		private void CloseTaskCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = (_tmm.Mode != TaskMode.Closed); }

		#endregion

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

		private void StutterMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			StutterIO.SaveTaskListToXML(Tasks, Settings.Default.LastTaskListFilename);
			Settings.Default.Save();

			// TODO: If the phrase is more than half over, update the phrase points for the current task.
		}

		private void UpdateMenuItem_Click(object sender, RoutedEventArgs e) { CheckForUpdates(); }
		
		#region ListBoxItem Events

		private void MarkCompleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			task.ToggleCompletion();
			RefreshTaskList();
		}
		
		private void EditTaskMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Get the task to edit.
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			_tmm.Task = task;
			_tmm.Mode = TaskMode.Edit;
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;
			StutterTask task = (StutterTask)item.DataContext;

			// If the iteration is currently on a phrase and the user deletes the current task,
			// stop the iteration to prevent null errors when updating the task.
			if (Iteration != null && Iteration.Task == task && Iteration.Running && Iteration.IterationState == StutterTimedState.Phrase) { EndIteration(); }

			if (_tmm.Mode == TaskMode.Edit && _tmm.Task == task) { _tmm.Mode = TaskMode.Closed; }

			Tasks.Remove(task);

			RefreshTaskList();
		}

		private void TaskListContainer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
			{
				// The user double-clicked on this task. Mark it as complete.
				StackPanel sp = (StackPanel)sender;
				StutterTask task = (StutterTask)sp.DataContext;
				task.ToggleCompletion();
				RefreshTaskList();
			}
		}

		#endregion

		#endregion
	}

	
}
