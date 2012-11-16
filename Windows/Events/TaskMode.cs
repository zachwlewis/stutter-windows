using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stutter.Core;

namespace Stutter.Windows.Events
{
	public enum TaskMode { Create, Edit, Closed };

	public class TaskModeEventArgs : EventArgs
	{
		public StutterTask Task;
		public readonly TaskMode Mode;

		public TaskModeEventArgs(StutterTask task, TaskMode mode)
		{
			Task = task;
			Mode = mode;
		}
	}

	public class TaskModeManager
	{
		private TaskMode _mode;
		public TaskMode Mode
		{
			get { return _mode; }
			set
			{
				if (value != _mode)
				{
					_mode = value;
					Changed(this, new TaskModeEventArgs(Task, _mode));
				}

			}
		}

		public StutterTask Task;
		public TaskModeEventHandler Changed;

		public TaskModeManager(TaskMode mode)
		{
			_mode = mode;
		}
	}

	/// <summary>
	/// Represents the method that will handle an event that has task mode data.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">Contains event data.</param>
	public delegate void TaskModeEventHandler(object sender, TaskModeEventArgs e);
}
