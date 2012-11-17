using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Stutter.Windows
{
	/// <summary>
	/// Class implementing support for "minimize to tray" functionality.
	/// </summary>
	public static class MinimizeToTray
	{
		/// <summary>
		/// Enables "minimize to tray" behavior for the specified Window.
		/// </summary>
		/// <param name="window">Window to enable the behavior for.</param>
		public static void Enable(Window window)
		{
			foreach (MinimizeToTrayInstance mi in instances)
			{
				if (mi.Target == window)
				{
					// The window already exists. Don't add it.
					return;
				}
			}

			// The search completed without finding the window. Add it.
			instances.Add(new MinimizeToTrayInstance(window));
		}

		/// <summary>
		/// Disables "minimize to tray" behavior for the specified Window.
		/// </summary>
		/// <param name="window">Window to enable the behavior for.</param>
		public static void Disable(Window window)
		{
			MinimizeToTrayInstance match = null;
			foreach (MinimizeToTrayInstance mi in instances)
			{
				if (mi.Target == window)
				{
					// The window exists in the list.
					match = mi;
					break;
				}
			}

			// If a match was found, dispose of it.
			if (match != null) {
				instances.Remove(match);
				match.Dispose();
			}


		}

		private static List<MinimizeToTrayInstance> instances = new List<MinimizeToTrayInstance>();

		/// <summary>
		/// Class implementing "minimize to tray" functionality for a Window instance.
		/// </summary>
		private class MinimizeToTrayInstance : IDisposable
		{
			private Window _window;
			public Window Target { get { return _window;}}
			private NotifyIcon _notifyIcon;
			private bool _balloonShown;

			/// <summary>
			/// Initializes a new instance of the MinimizeToTrayInstance class.
			/// </summary>
			/// <param name="window">Window instance to attach to.</param>
			public MinimizeToTrayInstance(Window window)
			{
				_window = window;
				_window.StateChanged += new EventHandler(HandleStateChanged);
			}

			public void Dispose()
			{
				_window.StateChanged -= new EventHandler(HandleStateChanged);
				_notifyIcon.Dispose();
			}

			/// <summary>
			/// Handles the Window's StateChanged event.
			/// </summary>
			/// <param name="sender">Event source.</param>
			/// <param name="e">Event arguments.</param>
			private void HandleStateChanged(object sender, EventArgs e)
			{
				if (_notifyIcon == null)
				{
					// Initialize NotifyIcon instance "on demand"
					_notifyIcon = new NotifyIcon();
					_notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
					_notifyIcon.MouseClick += new MouseEventHandler(HandleNotifyIconOrBalloonClicked);
					_notifyIcon.BalloonTipClicked += new EventHandler(HandleNotifyIconOrBalloonClicked);
				}
				// Update copy of Window Title in case it has changed
				_notifyIcon.Text = _window.Title;

				// Show/hide Window and NotifyIcon
				var minimized = (_window.WindowState == WindowState.Minimized);
				_window.ShowInTaskbar = !minimized;
				_notifyIcon.Visible = minimized;
				if (minimized && !_balloonShown)
				{
					// If this is the first time minimizing to the tray, show the user what happened
					_notifyIcon.ShowBalloonTip(1000, null, _window.Title, ToolTipIcon.None);
					_balloonShown = true;
				}
			}

			/// <summary>
			/// Handles a click on the notify icon or its balloon.
			/// </summary>
			/// <param name="sender">Event source.</param>
			/// <param name="e">Event arguments.</param>
			private void HandleNotifyIconOrBalloonClicked(object sender, EventArgs e)
			{
				// Restore the Window
				_window.WindowState = WindowState.Normal;
			}
		}
	}
}
