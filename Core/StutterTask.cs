using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutter.Core
{
	public class StutterTask
	{
		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public uint EstimatedPoints { get; set; }
		public uint ActualPoints { get; set; }

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
