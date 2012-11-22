using System;
using System.Windows.Threading;
using Stutter.Core.Events;

namespace Stutter.Core
{
	/// <summary>The state of the iteration.</summary>
	public enum StutterTimedState { Phrase, Block };

	/// <summary>An iteration consists of a Iteration and a Block.</summary>
	public class StutterIteration
	{
		/// <summary>The task to work on during the phrase.</summary>
		public StutterTask Task { get; protected set; }

		/// <summary>The timer running the phrase.</summary>
		public DispatcherTimer Timer { get; protected set; }

		/// <summary>The length of the phrase.</summary>
		public TimeSpan PhraseLength;

		/// <summary>The length of the block.</summary>
		public TimeSpan BlockLength;

		/// <summary>Is the phrase currently running?</summary>
		public bool Running
		{
			get { return Timer.IsEnabled; }
			set { Timer.IsEnabled = value; }
		}

		/// <summary>The amount of time elapsed since the phrase has begun.</summary>
		private TimeSpan elapsedTime;

		/// <summary>The time the phrase was started.</summary>
		private DateTime startTime;

		/// <summary>The phrase has completed.</summary>
		public event StutterTimerEventHandler Complete;

		/// <summary>The phrase has ticked.</summary>
		public event StutterTimerEventHandler Tick;

		/// <summary>The current state of the iteration.</summary>
		public StutterTimedState IterationState;


		/// <summary>Default constructor for StutterIteration</summary>
		/// <param name="task">The task to perform during the iteration.</param>
		/// <param name="phraseLength">The time to spend working.</param>
		/// <param name="blockLength">The time to spend resting.</param>
		public StutterIteration(StutterTask task, TimeSpan phraseLength, TimeSpan blockLength)
		{
			Task = task;
			PhraseLength = phraseLength;
			BlockLength = blockLength;

			CreateTimer();
		}

		/// <summary>Begins a new phrase.</summary>
		public void BeginPhrase()
		{
			// Set the current state.
			IterationState = StutterTimedState.Phrase;
			
			// Start the timer.
			StartTimer(PhraseLength);
		}

		/// <summary>Begins a new block.</summary>
		public void BeginBlock()
		{
			// Set the current state.
			IterationState = StutterTimedState.Block;

			// Start the timer.
			StartTimer(BlockLength);

		}

		/// <summary>Stops the Iteration completely.</summary>
		public void Stop()
		{
			Timer.Stop();
			Timer.IsEnabled = false;
		}

		#region Timer Events

		void Timer_Tick(object sender, EventArgs e)
		{
			// Update the total elapsed time.
			elapsedTime = DateTime.UtcNow - startTime;

			if (elapsedTime > PhraseLength)
			{
				// The timer has completed.
				
				// Stop the timer.
				Timer.Stop();
				
				// Update the current task's value if a phrase was completed.
				if (IterationState == StutterTimedState.Phrase && Task != null) { Task.ActualPoints++; }
				
				// Alert the UI.
				RaiseCompleteEvent();
			}

			else { RaiseTickEvent(); }
		}

		#endregion

		#region Raised Events

		private void RaiseCompleteEvent() { Complete(this, new StutterTimerEvent(Task, elapsedTime, PhraseLength, IterationState)); }

		private void RaiseTickEvent() { Tick(this, new StutterTimerEvent(Task, elapsedTime, PhraseLength, IterationState)); }

		#endregion

		#region Class Functions

		/// <summary>Creates a timer and adds event handlers.</summary>
		private void CreateTimer()
		{
			// Initialize the timer.
			Timer = new DispatcherTimer();
			// Define the UI update time.
			Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
			// Add the tick event listener.
			Timer.Tick += Timer_Tick;
		}

		/// <summary>Start or restart the timer.</summary>
		/// <param name="timerDuration">The length the timer should run.</param>
		/// <param name="forceReset">Should the timer be restarted if already running?</param>
		private void StartTimer(TimeSpan timerDuration, bool forceReset = false)
		{
			if (!Timer.IsEnabled || forceReset)
			{
				startTime = DateTime.UtcNow;
				elapsedTime = new TimeSpan(0);
				PhraseLength = timerDuration;
				Timer.Start();
			}
		}

		#endregion
	}
}
