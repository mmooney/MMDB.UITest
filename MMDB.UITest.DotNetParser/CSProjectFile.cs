using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSProjectFile
	{
		public List<CSClass> ClassList { get; set; }
		public string RootNamespace { get; set; }

		private class CSProjectComplileElement
		{
			public string FilePath { get; set; }
			public string DependentUponFilePath { get; set; }
		}

		public CSProjectFile()
		{
			this.ClassList = new List<CSClass>();
		}

		public static CSProjectFile Parse(string projectFile)
		{
			CSProjectFile returnValue = new CSProjectFile();
			CSharpParser parser = new CSharpParser();
			XDocument xdoc = XDocument.Load(projectFile);
			var rootNamespaceNode = xdoc.Descendants().FirstOrDefault(i=>i.Name.LocalName == "RootNamspace");
			if(rootNamespaceNode != null)
			{
				returnValue.RootNamespace = rootNamespaceNode.Value; 
			}
			else 
			{
				returnValue.RootNamespace = Path.GetFileNameWithoutExtension(projectFile);
			}
			var classFileList = GetFileList(xdoc, "Compile", ".cs");
			foreach(var classFile in classFileList)
			{
				string filePath = Path.Combine(Path.GetDirectoryName(projectFile),classFile.FilePath);
				if(File.Exists(filePath))
				{
					using(var reader = new StreamReader(filePath))
					{
						var compilationUnit = parser.Parse(reader);
						foreach (var node1 in compilationUnit.Children)
						{
							if (node1 is NamespaceDeclaration)
							{
								var namespaceNode = (NamespaceDeclaration)node1;
								foreach (var node2 in namespaceNode.Children)
								{
									if (node2 is TypeDeclaration)
									{
										var typeDefinitionNode = (TypeDeclaration)node2;
										string fullTypeName = namespaceNode.FullName + "." + typeDefinitionNode.Name;
										var classObject = returnValue.ClassList.SingleOrDefault(i=>i.ClassFullName == fullTypeName);
										if(classObject == null)
										{
											classObject = new CSClass
											{
												ClassFullName = fullTypeName
											};
											returnValue.ClassList.Add(classObject);
										}
										classObject.Parse(typeDefinitionNode, classFile.FilePath);
										if(!string.IsNullOrEmpty(classFile.DependentUponFilePath) && !classObject.DependentUponFilePathList.Contains(classFile.DependentUponFilePath, StringComparer.CurrentCultureIgnoreCase))
										{
											classObject.DependentUponFilePathList.Add(classFile.DependentUponFilePath);
										}
									}
								}
							}
						}
					}
				}
			}
			return returnValue;
		}

        public static void EnsureFileInclude(string projectPath, string filePath, string dependentFilePath)
        {
			string workingFilePath = filePath;
			if(workingFilePath.StartsWith(Path.GetDirectoryName(projectPath), StringComparison.CurrentCultureIgnoreCase))
			{
				workingFilePath = workingFilePath.Substring(Path.GetDirectoryName(projectPath).Length+1);
			}
			string workingDependentFilePath = dependentFilePath;
			if(!string.IsNullOrEmpty(workingDependentFilePath))
			{
				workingDependentFilePath = Path.GetFileName(workingDependentFilePath);
			}

			XDocument xdoc = XDocument.Load(projectPath);
			bool anyChange = false;
			var compileNode = xdoc.Descendants().SingleOrDefault(i => i.Name.LocalName == "Compile" && i.Attributes().Any(j => j.Name.LocalName == "Include" && string.Equals(j.Value, workingFilePath, StringComparison.CurrentCultureIgnoreCase)));
			if (compileNode == null)
			{
				var itemGroupNode = xdoc.Descendants().Where(i=>i.Name.LocalName == "ItemGroup").OrderByDescending(i=>i.Descendants().Any(j=>j.Name.LocalName == "Compile")).First();
				if(itemGroupNode == null)
				{
					throw new Exception("Unable to find ItemGroup node");
				}
				compileNode = new XElement(XName.Get("Compile", itemGroupNode.Name.NamespaceName));
				compileNode.Add(new XAttribute("Include", workingFilePath));
				itemGroupNode.Add(compileNode);
				anyChange = true;
			}
			var dependentUponNode = compileNode.Elements().SingleOrDefault(i=>i.Name.LocalName == "DependentUpon");
			if(dependentUponNode != null)
			{
				if(!string.IsNullOrEmpty(workingDependentFilePath))
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
			if(anyChange)
			{
				xdoc.Save(projectPath);
			}
		}

		private static List<CSProjectComplileElement> GetFileList(XDocument xdoc, string elementName, string extension)
		{
			var returnValue = new List<CSProjectComplileElement>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == elementName && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(extension)));
			foreach (var node in nodes)
			{
				var item = new CSProjectComplileElement();
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
