using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public class TargetProject
	{
		public List<TargetClass> TargetClassList { get; set;  }

		public TargetProject()
		{
			this.TargetClassList = new List<TargetClass>();
		}

		public static TargetProject Load(string targetProjectPath)
		{
			TargetProject returnValue = new TargetProject();
			CSProjectFile csProject = CSProjectFile.Parse(targetProjectPath);
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
