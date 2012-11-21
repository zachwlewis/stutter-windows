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
	public static class NotificationHelper
	{
		/// <summary>
		/// Enables "minimize to tray" behavior for the specified Window.
		/// </summary>
		/// <param name="window">Window to enable the behavior for.</param>
		public static void Enable(Window window)
		{
			if (!instances.ContainsKey(window))
			{
				// The search completed without finding the window. Add it.
				instances.Add(window, new MinimizeToTrayInstance(window));
			}
		}

		/// <summary>
		/// Disables "minimize to tray" behavior for the specified Window.
		/// </summary>
		/// <param name="window">Window to enable the behavior for.</param>
		public static void Disable(Window window)
		{
			if(instances.ContainsKey(window))
			{
				MinimizeToTrayInstance match = instances[window];
				instances.Remove(window);
				match.Dispose();
			}
		}

		public static void CreateNotificiation(Window window, string message, string title = null)
		{
			if (instances.ContainsKey(window))
			{
				instances[window].CreateNotification(message);
			}
		}

		private static Dictionary<Window,MinimizeToTrayInstance> instances = new Dictionary<Window,MinimizeToTrayInstance>();

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

			public void CreateNotification(string message, string title = null)
			{
				// TODO: Support showing notifications even if the app isn't minimized.
				if (_notifyIcon != null && _notifyIcon.Visible)
				{
					_notifyIcon.ShowBalloonTip(1000, title, message, ToolTipIcon.Info);
				}
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
					_notifyIcon.ShowBalloonTip(1000, null, _window.Title + " has been minimized to the notification area.", ToolTipIcon.None);
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
