using System;
using System.Windows;
using System.Windows.Media;

namespace Stutter.Core
{
	public class StutterTask
	{
		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public uint EstimatedPoints { get; set; }
		public uint ActualPoints { get; set; }

		// TODO: Seperate the display of the class from the implementation of the class.
		public uint PointValue
		{
			get { return Math.Min(ActualPoints, EstimatedPoints); }
		}

		public uint PointMaximum
		{
			get { return Math.Max(ActualPoints, EstimatedPoints); }
		}

		public Visibility IsValueVisible
		{
			get
			{
				if (Stutter.Properties.Settings.Default.IsTaskValueVisible)
				{
					if (EstimatedPoints == 0) { return Visibility.Collapsed; }
					else { return Visibility.Visible; }
				}
				else { return Visibility.Collapsed; }
			}
		}

		public Brush EstimateValueBrush
		{
			get
			{
				if (ActualPoints > EstimatedPoints) return new SolidColorBrush(Colors.OrangeRed);
				else return new SolidColorBrush(Colors.DimGray);
			}
		}

		public Visibility IsProgressVisible
		{
			get
			{
				if (Stutter.Properties.Settings.Default.IsTaskProgressVisible)
				{
					if (EstimatedPoints == 0) { return Visibility.Collapsed; }
					else { return Visibility.Visible; }
				}
				else { return Visibility.Collapsed; }
			}
		}

		public StutterTask(string name, string description = "", uint estimatedPoints = 0, uint actualPoints = 0)
		{
			Name = name;
			Description = description;
			EstimatedPoints = estimatedPoints;
			ActualPoints = actualPoints;
		}

		public override string ToString() { return Name; }
	}
}
