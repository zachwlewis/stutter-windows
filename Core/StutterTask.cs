using System;
using System.Windows;
using System.Windows.Media;

namespace Stutter.Core
{
	public sealed class StutterTask : IComparable<StutterTask>
	{
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public uint EstimatedPoints { get; set; }
		public uint ActualPoints { get; set; }
		public bool IsComplete { get; set; }

		// TODO: Seperate the display of the class from the implementation of the class.
		public uint PointValue
		{
			get { return Math.Min(ActualPoints, EstimatedPoints); }
		}

		public uint PointMaximum
		{
			get { return Math.Max(ActualPoints, EstimatedPoints); }
		}

		public Visibility IsEstimatedPointsVisible
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

		public Visibility IsActualPointsVisible
		{
			get
			{
				if (Stutter.Properties.Settings.Default.IsTaskValueVisible)
				{
					if (ActualPoints == 0 && EstimatedPoints == 0) { return Visibility.Collapsed; }
					else { return Visibility.Visible; }
				}
				else { return Visibility.Collapsed; }
			}
		}

		public Brush EstimatedPointsBrush
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

		public Brush IsCompleteBrush
		{
			get
			{
				return IsComplete ? new SolidColorBrush(Colors.PaleGoldenrod) : null;
			}
		}

		public StutterTask(string name, string description = "", uint estimatedPoints = 0, uint actualPoints = 0, bool isComplete = false)
		{
			Name = name;
			Description = description;
			EstimatedPoints = estimatedPoints;
			ActualPoints = actualPoints;
			IsComplete = isComplete;
		}

		public void ToggleCompletion()
		{
			IsComplete = !IsComplete;
		}

		public override string ToString() { return Name + Description + EstimatedPoints; }

		int IComparable<StutterTask>.CompareTo(StutterTask t) { return IsComplete ? t.IsComplete ? 0 : 1 : t.IsComplete ? -1 : 0; }
	}
}
