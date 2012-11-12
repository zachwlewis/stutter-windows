using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

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
					writer.WriteElementString("IsComplete", st.IsComplete.ToString());

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
					string name = "";
					string description = "";
					uint estimate = 0;
					uint actual  = 0;
					bool complete = false;
					try { name = el.Element((XName)"Name").Value; } catch { continue; }
					try { description = el.Element((XName)"Description").Value; } catch { }
					try { estimate = (uint)Convert.ToInt32(el.Element((XName)"EstimatedPoints").Value); } catch { }
					try { actual = (uint)Convert.ToInt32(el.Element((XName)"ActualPoints").Value); } catch { }
					try { complete = Convert.ToBoolean(el.Element((XName)"IsComplete").Value); } catch { }
					tasks.Add(new StutterTask(name, description, estimate, actual, complete));
				}
			}

			return tasks;
		}
	}
}
