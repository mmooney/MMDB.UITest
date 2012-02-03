using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;
using System.IO;

namespace MMDB.UITest.Generator.Library
{
	public class TargetProject
	{
		public List<TargetClass> TargetClassList { get; set;  }
		public string Directory { get; set; }
		public string FileName { get; set; }
		public string RootNamespace { get; set; }

		public TargetProject()
		{
			this.TargetClassList = new List<TargetClass>();
		}

		public static TargetProject Load(string targetProjectPath)
		{
			ProjectParser parser = new ProjectParser();
			CSProjectFile csProject = parser.ParseFile(targetProjectPath);
			TargetProject returnValue = new TargetProject()
			{
				Directory = Path.GetDirectoryName(targetProjectPath),
				FileName = Path.GetFileName(targetProjectPath),
				RootNamespace = csProject.RootNamespace
			};
			foreach(var csClass in csProject.ClassList)
			{
				var targetClass = TargetClass.TryLoad(csClass);
				if(targetClass != null)
				{
					returnValue.TargetClassList.Add(targetClass);
				}
			}
			return returnValue;
		}
	}
}
