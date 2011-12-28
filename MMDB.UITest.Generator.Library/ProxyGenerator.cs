using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using System.Diagnostics;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Generator.Library.WebControls;

namespace MMDB.UITest.Generator.Library
{
	public static class ProxyGenerator
	{
		public static SourceWebProject LoadWebPages(string csProjFilePath)
		{
			SourceWebProject project = new SourceWebProject();
			List<SourceWebPage> returnList = new List<SourceWebPage>();
			XDocument xdoc = XDocument.Load(csProjFilePath);

			List<string> aspxFileList = GetAspxFileList(xdoc);
			List<string> masterFileList = GetMasterFileList(xdoc);

			string basePath = Path.GetDirectoryName(csProjFilePath);

			foreach(string masterFile in masterFileList)
			{
				string masterFilePath = Path.Combine(basePath, masterFile);
				string designerFilePath = Path.Combine(basePath, masterFile + ".designer.cs");
				CSharpParser parser = new CSharpParser();
				var pageObject = new SourceMasterPage()
				{
					PagePath = masterFile
				};
				using (StreamReader reader = new StreamReader(designerFilePath))
				{
					var compilationUnit = parser.Parse(reader);
					ParseMasterPage(pageObject, compilationUnit);
				}
				using(StreamReader reader = new StreamReader(masterFilePath))
				{
					var compilationUnit = parser.Parse(reader);
					ParseMasterPage(pageObject, compilationUnit);
				}
				project.MasterPageList.Add(pageObject);
			}
			//foreach(var aspxFile in aspxFileList)
			//{
			//    string csFilePath = Path.Combine(basePath,aspxFile+".designer.cs");
			//    if(File.Exists(csFilePath))
			//    {
			//        CSharpParser parser = new CSharpParser();
			//        using(StreamReader reader = new StreamReader(csFilePath))
			//        {
			//            var compilationUnit = parser.Parse(reader);
			//            var webPage = new SourceWebPage() 
			//            {
			//                Url = aspxFile
			//            };

			//            foreach(var element in compilationUnit.Descendants)
			//            {
			//                Debug.WriteLine("--{0}: {1}",csFilePath,element.GetType().Name);
			//                if(element is SimpleType)
			//                {
			//                    var x = 0;
			//                }
			//                if(element is MemberType)
			//                {
			//                    var x = 0;
			//                }
			//            }
			//            //csharpTreeView.Nodes.Clear();
			//            //foreach (var element in compilationUnit.Children)
			//            //{
			//            //    csharpTreeView.Nodes.Add(MakeTreeNode(element));
			//            //}
			//            //SelectCurrentNode(csharpTreeView.Nodes);
			//            //resolveButton.Enabled = true;
			//            //findReferencesButton.Enabled = true;
			//        }
			//    }
			//}
			return project;
		}

		private static void ParseMasterPage(SourceMasterPage pageObject, CompilationUnit compilationUnit)
		{
			foreach(var node in compilationUnit.Children)
			{
				if(node is NamespaceDeclaration)
				{
					var namespaceNode = (NamespaceDeclaration)node;
					LoadMasterPageNamespace(pageObject, namespaceNode);
				}
			}
		}

		private static void LoadMasterPageNamespace(SourceMasterPage pageObject, NamespaceDeclaration namespaceNode)
		{
			foreach(var node in namespaceNode.Children)
			{
				if(node is TypeDeclaration)
				{
					var typeDefinitionNode = (TypeDeclaration)node;
					CSClass classObject = CSClass.Parse(namespaceNode, typeDefinitionNode);
					foreach(var fieldObject in classObject.FieldList)
					{
						if(fieldObject.TypeNamespace == "System.Web.UI.WebControls")
						{
							if(fieldObject.TypeName == "Literal")
							{
								var control = new LiteralControl
								{
									FieldName = fieldObject.FieldName,
									TypeName = fieldObject.TypeName,
									TypeNamespace = fieldObject.TypeNamespace
								};
								pageObject.Controls.Add(control);
							}
							if(fieldObject.TypeName == "ContentPlaceHolder")
							{
								pageObject.ContentHolderIDs.Add(fieldObject.FieldName);
							}
						}
					}
				}
			}
		}


		private static List<string> GetMasterFileList(XDocument xdoc)
		{
			List<string> aspxFileList = new List<string>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == "Content" && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(".master")));
			foreach (var node in nodes)
			{
				string path = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				aspxFileList.Add(path);
			}
			return aspxFileList;
		}

		private static List<string> GetAspxFileList(XDocument xdoc)
		{
			List<string> aspxFileList = new List<string>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == "Content" && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(".aspx")));
			foreach (var node in nodes)
			{
				string path = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				aspxFileList.Add(path);
			}
			return aspxFileList;
		}
	}
}
