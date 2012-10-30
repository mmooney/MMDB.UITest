using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;
using System.IO;

namespace MMDB.UITest.Generator.Library
{
	public class TargetModelParser
	{
		private TargetClassManager TargetClassManager { get; set; }
		public TargetModelParser()
		{
			this.TargetClassManager = new TargetClassManager();
		}

		public TargetProject LoadFromProjectFile(CSProjectFile targetProjectFile, string targetProjectFileName)
		{
			TargetProject returnValue = new TargetProject()
			{
				Directory = Path.GetDirectoryName(targetProjectFileName),
				FileName = Path.GetFileName(targetProjectFileName),
				RootNamespace = targetProjectFile.RootNamespace
			};
			foreach(var csClass in targetProjectFile.ClassList)
			{
				var targetClass = this.TargetClassManager.TryLoadTargetClass(csClass);
				if(targetClass != null)
				{
					returnValue.TargetClassList.Add(targetClass);
				}
			}
			return returnValue;
		}
	}
}
