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

		//public static CSProjectFile Parse(string projectFileName)
		//{
		//    string data = File.ReadAllText(projectFileName);
		//    return ParseString(data, projectFileName);
		//}

		//public static CSProjectFile ParseString(string data, string projectFilePath)
		//{
		//    CSProjectFile returnValue = new CSProjectFile();
		//    CSharpParser parser = new CSharpParser();
		//    XDocument xdoc = XDocument.Parse(data);
		//    var rootNamespaceNode = xdoc.Descendants().FirstOrDefault(i=>i.Name.LocalName == "RootNamspace");
		//    if(rootNamespaceNode != null)
		//    {
		//        returnValue.RootNamespace = rootNamespaceNode.Value; 
		//    }
		//    else 
		//    {
		//        returnValue.RootNamespace = Path.GetFileNameWithoutExtension(projectFilePath);
		//    }
		//    var classFileList = GetFileList(xdoc, "Compile", ".cs");
		//    foreach(var classFile in classFileList)
		//    {
		//        string filePath = Path.Combine(Path.GetDirectoryName(projectFilePath),classFile.FilePath);
		//        if(!File.Exists(filePath))
		//        {
		//            throw new Exception("Class file not found: " + filePath);
		//        }
		//        else
		//        {
		//            using(var reader = new StreamReader(filePath))
		//            {
		//                var compilationUnit = parser.Parse(reader, Path.GetFileName(filePath));
		//                foreach (var node1 in compilationUnit.Children)
		//                {
		//                    if (node1 is NamespaceDeclaration)
		//                    {
		//                        var namespaceNode = (NamespaceDeclaration)node1;
		//                        foreach (var node2 in namespaceNode.Children)
		//                        {
		//                            if (node2 is TypeDeclaration)
		//                            {
		//                                var typeDefinitionNode = (TypeDeclaration)node2;
		//                                var classObject = returnValue.ClassList.SingleOrDefault(i => i.NamespaceName == namespaceNode.FullName && i.ClassName == typeDefinitionNode.Name);
		//                                if(classObject == null)
		//                                {
		//                                    classObject = new CSClass
		//                                    {
		//                                        NamespaceName = namespaceNode.FullName,
		//                                        ClassName = typeDefinitionNode.Name
		//                                    };
		//                                    returnValue.ClassList.Add(classObject);
		//                                }
		//                                ClassParser.BuildClass(classObject, typeDefinitionNode, classFile.FilePath);
		//                                if(!string.IsNullOrEmpty(classFile.DependentUponFilePath))
		//                                {
		//                                    string relativeDependentUponFilePath = Path.Combine(Path.GetDirectoryName(classFile.FilePath), classFile.DependentUponFilePath);
		//                                    if(!string.IsNullOrEmpty(classFile.DependentUponFilePath) && !classObject.DependentUponFilePathList.Contains(relativeDependentUponFilePath, StringComparer.CurrentCultureIgnoreCase))
		//                                    {
		//                                        classObject.DependentUponFilePathList.Add(relativeDependentUponFilePath);
		//                                    }
		//                                }
		//                            }
		//                        }
		//                    }
		//                }
		//            }
		//        }
		//    }
		//    return returnValue;
		//}

		//private static List<CSProjectComplileElement> GetFileList(XDocument xdoc, string elementName, string extension)
		//{
		//    var returnValue = new List<CSProjectComplileElement>();
		//    var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == elementName && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(extension)));
		//    foreach (var node in nodes)
		//    {
		//        var item = new CSProjectComplileElement();
		//        item.FilePath = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
		//        var dependentUponNode = node.Elements().SingleOrDefault(i=>i.Name.LocalName == "DependentUpon");
		//        if(dependentUponNode != null)
		//        {
		//            item.DependentUponFilePath = dependentUponNode.Value;
		//        }
		//        returnValue.Add(item);
		//    }
		//    return returnValue;
		//}
	}
}
