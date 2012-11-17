﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Converters;
using System.Windows.Input;
using System.Windows.Data;
using System.Globalization;
using Stutter.Windows.Events;

namespace Stutter.Windows
{
	/// <summary>Converts from a bool to the visibility of the task list.</summary>
	public class BoolToTaskListVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Nullable<bool> isVisible = value as Nullable<bool>;
			if (isVisible.HasValue && isVisible.Value) { return Visibility.Visible; }
			else { return Visibility.Collapsed; }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new NotImplementedException();
		}
	}
}