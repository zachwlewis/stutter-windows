using System;

namespace Stutter.Core.Events
{
	public class StutterTimerEvent : EventArgs
	{
		public StutterTask Task;
		public readonly TimeSpan Elapsed;
		public readonly TimeSpan Total;
		public readonly StutterTimedState State;

		public StutterTimerEvent(StutterTask task, TimeSpan elapsed, TimeSpan total, StutterTimedState state)
		{
			Task = task;
			Elapsed = elapsed;
			Total = total;
			State = state;
		}
	}

	/// <summary>
	/// Represents the method that will handle an event that has Stutter data.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A Stutter.StutterTimerEvent that contains event data.</param>
	public delegate void StutterTimerEventHandler(object sender, StutterTimerEvent e);
}
