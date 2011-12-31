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

		public CSProjectFile()
		{
			this.ClassList = new List<CSClass>();
		}

		public static CSProjectFile Parse(string projectFile)
		{
			CSProjectFile returnValue = new CSProjectFile();
			CSharpParser parser = new CSharpParser();
			XDocument xdoc = XDocument.Load(projectFile);
			List<string> classFileList = GetFileList(xdoc, "Compile", ".cs");
			foreach(string classFile in classFileList)
			{
				using(var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(projectFile),classFile)))
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
									var classObject = returnValue.ClassList.SingleOrDefault(i=>i.ClassName == typeDefinitionNode.Name && i.Namespace == namespaceNode.FullName);
									if(classObject == null)
									{
										classObject = new CSClass
										{
											Namespace = namespaceNode.FullName,
											ClassName = typeDefinitionNode.Name
										};
										returnValue.ClassList.Add(classObject);
									}
									classObject.Parse(typeDefinitionNode, classFile);

									var attributeSectionList = typeDefinitionNode.Children.Where(i=>i is AttributeSection);
									foreach(AttributeSection attributeSectionNode in attributeSectionList)
									{
										foreach (var attributeNode in attributeSectionNode.Attributes)
										{
											var attribute = CSAttribute.Parse(attributeNode);
											classObject.AttributeList.Add(attribute);
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

		private static List<string> GetFileList(XDocument xdoc, string elementName, string extension)
		{
			List<string> returnValue = new List<string>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == elementName && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(extension)));
			foreach (var node in nodes)
			{
				string path = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				returnValue.Add(path);
			}
			return returnValue;
		}
	}
}
