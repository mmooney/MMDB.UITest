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
		private ClassParser ClassParser { get; set; }

		private class CSProjectCompileElement
		{
			public string FilePath { get; set; }
			public string DependentUponFilePath { get; set; }
		}

		public ProjectParser()
		{
			this.ClassParser = new ClassParser();
		}

		public ProjectParser(ClassParser classParser)
		{
			this.ClassParser = classParser;
		}

		public virtual CSProjectFile ParseFile(string projectFilePath)
		{
			string data = File.ReadAllText(projectFilePath);
			return ParseString(data, projectFilePath);
		}

		public virtual CSProjectFile ParseString(string data, string projectFilePath)
		{
			string workingProjectFilePath = Path.GetFullPath(projectFilePath);
			CSProjectFile returnValue = new CSProjectFile();
			XDocument xdoc = XDocument.Parse(data);
			var rootNamespaceNode = xdoc.Descendants().FirstOrDefault(i => i.Name.LocalName == "RootNamespace");
			if (rootNamespaceNode != null)
			{
				returnValue.RootNamespace = rootNamespaceNode.Value;
			}
			else
			{
				returnValue.RootNamespace = Path.GetFileNameWithoutExtension(workingProjectFilePath);
			}
			var classFileList = GetFileList(xdoc, "Compile", ".cs");
			foreach (var classFile in classFileList)
			{
				string filePath = Path.Combine(Path.GetDirectoryName(workingProjectFilePath), classFile.FilePath);
				filePath = Path.GetFullPath(filePath);
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

		public static void EnsureFileInclude(string projectPath, string includeFilePath, string dependentFilePath)
		{
			using(var stream = new FileStream(projectPath, FileMode.Open))
			{
				EnsureFileInclude(stream, projectPath, includeFilePath, dependentFilePath);
			}
		}

		public static void EnsureFileInclude(Stream stream, string projectPath, string includeFilePath, string dependentFilePath)
		{
			string workingFilePath = includeFilePath;
			if (workingFilePath.StartsWith(Path.GetDirectoryName(projectPath), StringComparison.CurrentCultureIgnoreCase))
			{
				workingFilePath = workingFilePath.Substring(Path.GetDirectoryName(projectPath).Length + 1);
			}
			string workingDependentFilePath = dependentFilePath;
			if (!string.IsNullOrEmpty(workingDependentFilePath))
			{
				workingDependentFilePath = Path.GetFileName(workingDependentFilePath);
			}

			stream.Position = 0;
			XDocument xdoc = XDocument.Load(stream);
			bool anyChange = false;
			var compileNode = xdoc.Descendants().SingleOrDefault(i => i.Name.LocalName == "Compile" && i.Attributes().Any(j => j.Name.LocalName == "Include" && string.Equals(j.Value, workingFilePath, StringComparison.CurrentCultureIgnoreCase)));
			if (compileNode == null)
			{
				var itemGroupNode = xdoc.Descendants().Where(i => i.Name.LocalName == "ItemGroup").OrderByDescending(i => i.Descendants().Any(j => j.Name.LocalName == "Compile")).First();
				if (itemGroupNode == null)
				{
					throw new Exception("Unable to find ItemGroup node");
				}
				compileNode = new XElement(XName.Get("Compile", itemGroupNode.Name.NamespaceName));
				compileNode.Add(new XAttribute("Include", workingFilePath));
				itemGroupNode.Add(compileNode);
				anyChange = true;
			}
			var dependentUponNode = compileNode.Elements().SingleOrDefault(i => i.Name.LocalName == "DependentUpon");
			if (dependentUponNode != null)
			{
				if (!string.IsNullOrEmpty(workingDependentFilePath))
				{
					if (!string.Equals(dependentUponNode.Value, workingDependentFilePath, StringComparison.CurrentCultureIgnoreCase))
					{
						dependentUponNode.Value = workingDependentFilePath;
						anyChange = true;
					}
				}
				else
				{
					dependentUponNode.Remove();
					anyChange = true;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(workingDependentFilePath))
				{
					dependentUponNode = new XElement(XName.Get("DependentUpon", compileNode.Name.NamespaceName));
					dependentUponNode.Value = workingDependentFilePath;
					compileNode.Add(dependentUponNode);
					anyChange = true;
				}
			}
			if (anyChange)
			{
				stream.Position = 0;
				xdoc.Save(stream);
			}
		}

	}
}
