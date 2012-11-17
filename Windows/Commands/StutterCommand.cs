using System.Windows.Input;

namespace Stutter.Windows.Commands
{
	public class StutterCommand
	{
		private static readonly RoutedUICommand addTaskCommand = new RoutedUICommand("Add Task", "AddTask", typeof(StutterCommand));
		public static RoutedUICommand AddTaskCommand { get { return addTaskCommand; } }

		private static readonly RoutedUICommand closeTaskCommand = new RoutedUICommand("Close Task", "closeTask", typeof(StutterCommand));
		public static RoutedUICommand CloseTaskCommand { get { return closeTaskCommand; } }
	}
}
