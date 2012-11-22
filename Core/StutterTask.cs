using System;
using System.Windows;
using System.Windows.Media;

namespace Stutter.Core
{
	public sealed class StutterTask : IComparable<StutterTask>
	{
		public string Name { get; set; }
		public string Description { get; set; }
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

		public double IsCompletedAlpha
		{
			get
			{
				return IsComplete ? 0.5d : 1.0d;
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

		/// <summary>Is the progress of the task visible?</summary>
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

		/// <summary>Default constructor for a StutterTask</summary>
		/// <param name="name">The name of a task.</param>
		/// <param name="description">A detailed description of a task.</param>
		/// <param name="estimatedPoints">The estimated value of the task.</param>
		/// <param name="actualPoints">The actual value of the task.</param>
		/// <param name="isComplete">Is the task completed?</param>
		public StutterTask(string name, string description = "", uint estimatedPoints = 0, uint actualPoints = 0, bool isComplete = false)
		{
			Name = name;
			Description = description;
			EstimatedPoints = estimatedPoints;
			ActualPoints = actualPoints;
			IsComplete = isComplete;
		}

		/// <summary>Toggles the completion status of the task.</summary>
		public void ToggleCompletion()
		{
			IsComplete = !IsComplete;
		}

		public override string ToString() { return Name + Description + EstimatedPoints; }

		int IComparable<StutterTask>.CompareTo(StutterTask t) { return IsComplete ? t.IsComplete ? 0 : 1 : t.IsComplete ? -1 : 0; }
	}
}
