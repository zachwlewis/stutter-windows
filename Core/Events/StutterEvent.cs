using System;

namespace Stutter.Core.Events
{
	public class StutterEventArgs : EventArgs
	{
		public TimeSpan Elapsed;
		public TimeSpan Total;

		public StutterEventArgs():base()
		{
		
		}
	}

	/// <summary>
	/// Represents the method that will handle an event that has Stutter data.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A Stutter.StutterEventArgs that contains event data.</param>
	public delegate void StutterEventHandler(object sender, StutterEventArgs e);
}
