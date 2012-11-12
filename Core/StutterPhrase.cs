using System;
using System.Windows.Threading;
using Stutter.Core.Events;

namespace Stutter.Core
{
	/// <summary>
	/// Handles timing and task management for a task.
	/// </summary>
	public class StutterPhrase
	{
		/// <summary>
		/// The task to work on during the phrase.
		/// </summary>
		public StutterTask Task { get; protected set; }

		/// <summary>
		/// The timer running the phrase.
		/// </summary>
		public DispatcherTimer Timer { get; protected set; }

		/// <summary>
		/// The length of the phrase.
		/// </summary>
		public TimeSpan Duration { get; protected set; }

		/// <summary>
		/// Is the phrase currently running?
		/// </summary>
		public bool Running
		{
			get { return Timer.IsEnabled; }
			set { Timer.IsEnabled = value; }
		}

		/// <summary>
		/// The amount of time elapsed since the phrase has begun.
		/// </summary>
		private TimeSpan elapsedTime;

		/// <summary>
		/// The time the phrase was started.
		/// </summary>
		private DateTime startTime;

		/// <summary>
		/// The phrase has completed.
		/// </summary>
		public event StutterEventHandler Complete;

		/// <summary>
		/// The phrase has ticked.
		/// </summary>
		public event StutterEventHandler Tick;

		public StutterPhrase(StutterTask task, int phraseMinutes, int phraseSeconds)
		{
			Task = task;
			Duration = new TimeSpan(0, phraseMinutes, phraseSeconds);
			CreateTimer();			
		}

		public void Start()
		{
			startTime = DateTime.UtcNow;
			elapsedTime = new TimeSpan(0);
			Timer.Start();
		}

		void Timer_Tick(object sender, EventArgs e)
		{
			// Update the total elapsed time.
			elapsedTime = DateTime.UtcNow - startTime;

			if (elapsedTime > Duration) { RaiseCompleteEvent(); }
			else { RaiseTickEvent(); }
		}

		void RaiseCompleteEvent()
		{
			StutterEventArgs e = new StutterEventArgs();
			e.Total = Duration;
			e.Elapsed = elapsedTime;
			Complete(this, e);
			Timer.Stop();
		}

		void RaiseTickEvent()
		{
			StutterEventArgs e = new StutterEventArgs();
			e.Total = Duration;
			e.Elapsed = elapsedTime;
			Tick(this, e);
		}

		void CreateTimer()
		{
			// Initialize the timer.
			Timer = new DispatcherTimer();
			// Define the UI update time.
			Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
			// Add the tick event listener.
			Timer.Tick += Timer_Tick;
		}
	}
}
