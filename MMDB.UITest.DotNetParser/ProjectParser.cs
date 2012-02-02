using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace MMDB.UITest.DotNetParser
{
	public class ProjectParser
	{
		private IClassParser ClassParser { get; set; }

		private class CSProjectCompileElement
		{
			public string FilePath { get; set; }
			public string DependentUponFilePath { get; set; }
		}

		public ProjectParser()
		{
			this.ClassParser = new ClassParser();
		}

		public ProjectParser(IClassParser classParser)
		{
			this.ClassParser = classParser;
		}

		public CSProjectFile ParseString(string data, string projectFilePath)
		{
			CSProjectFile returnValue = new CSProjectFile();
			XDocument xdoc = XDocument.Parse(data);
			var rootNamespaceNode = xdoc.Descendants().FirstOrDefault(i => i.Name.LocalName == "RootNamspace");
			if (rootNamespaceNode != null)
			{
				returnValue.RootNamespace = rootNamespaceNode.Value;
			}
			else
			{
				returnValue.RootNamespace = Path.GetFileNameWithoutExtension(projectFilePath);
			}
			var classFileList = GetFileList(xdoc, "Compile", ".cs");
			foreach (var classFile in classFileList)
			{
				string filePath = Path.Combine(Path.GetDirectoryName(projectFilePath), classFile.FilePath);
				returnValue.ClassList = this.ClassParser.ParseFile(filePath, returnValue.ClassList, new string[] {classFile.DependentUponFilePath});
			}
			return returnValue;
		}

		private static List<CSProjectCompileElement> GetFileList(XDocument xdoc, string elementName, string extension)
		{
			var returnValue = new List<CSProjectCompileElement>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == elementName && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(extension)));
			foreach (var node in nodes)
			{
				var item = new CSProjectCompileElement();
				item.FilePath = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				var dependentUponNode = node.Elements().SingleOrDefault(i=>i.Name.LocalName == "DependentUpon");
				if(dependentUponNode != null)
				{
					item.DependentUponFilePath = dependentUponNode.Value;
				}
				returnValue.Add(item);
			}
			return returnValue;
		}
	}
}
