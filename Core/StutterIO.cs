using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Stutter.Core
{
	public class StutterIO
	{
		public static void SaveTaskListToXML(List<StutterTask> tasks, string filename)
		{
			using (XmlWriter writer = XmlWriter.Create(filename))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("Tasks");

				foreach (StutterTask st in tasks)
				{
					writer.WriteStartElement("Task");

					writer.WriteElementString("Name", st.Name);
					writer.WriteElementString("Description", st.Description);
					writer.WriteElementString("EstimatedPoints", st.EstimatedPoints.ToString());
					writer.WriteElementString("ActualPoints", st.ActualPoints.ToString());

					writer.WriteEndElement();
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}

		public static List<StutterTask> LoadTaskListFromXML(string filename)
		{
			List<StutterTask> tasks = new List<StutterTask>();

			if (!File.Exists(filename))
			{
				// The file doesn't exist.
				// Return the empty list.
				return tasks;
			}

			using (XmlReader reader = XmlReader.Create(filename))
			{
				try
				{
					reader.MoveToContent();
					reader.ReadStartElement("Tasks");
				}
				catch (XmlException ex)
				{
					// TODO: Tactfully deliver the exception to the user.
					Console.WriteLine(ex.ToString());
					return tasks;
				}

				while (reader.Name == "Task")
				{
					XElement el = (XElement) XNode.ReadFrom(reader);
					string name = el.Element((XName)"Name").Value;
					string description = el.Element((XName)"Description").Value;
					uint estimate = (uint)Convert.ToInt32(el.Element((XName)"EstimatedPoints").Value);
					uint actual = (uint)Convert.ToInt32(el.Element((XName)"ActualPoints").Value);
					tasks.Add(new StutterTask(name, description, estimate, actual));
				}
			}

			return tasks;
		}
	}
}
