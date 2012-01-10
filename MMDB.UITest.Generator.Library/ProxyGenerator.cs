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

            CSProjectFile csProject = CSProjectFile.Parse(csProjFilePath);
            project.RootNamespace = csProject.RootNamespace;

            foreach (var csClass in csProject.ClassList)
            {
                if (csClass.DependentUponFilePathList.Any(i => i.EndsWith(".master", StringComparison.CurrentCultureIgnoreCase)))
                {
                    var pageObject = project.MasterPageList.SingleOrDefault(i => i.ClassFullName == csClass.ClassFullName);
                    if (pageObject == null)
                    {
                        pageObject = new SourceMasterPage();
                        project.MasterPageList.Add(pageObject);
                    }
                    ProxyGenerator.PopulatePageObject(pageObject, csClass);
                    pageObject.PageUrl = csClass.DependentUponFilePathList.Single(i => i.EndsWith(".master", StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return project;
        }
        
        public static void UpdateProxyProject(string targetProjectPath, SourceWebProject sourceProject)
		{
			XDocument xdoc = XDocument.Load(targetProjectPath);
			TargetProject targetProject = TargetProject.Load(targetProjectPath);
			List<TargetClass> classesToAddToProject = new List<TargetClass>();
			foreach(var masterPage in sourceProject.MasterPageList)
			{
				//Find existing object
				var targetClass = targetProject.TargetClassList.SingleOrDefault(i=>i.SourceClassFullName == masterPage.ClassFullName);
				if(targetClass == null)
				{
					targetClass = TargetClass.Create(targetProject, sourceProject, masterPage);
					classesToAddToProject.Add(targetClass);
				}
				targetClass.PageUrl = masterPage.PageUrl;
				var comparison = UIObjectComparison.Compare(masterPage, targetClass);
				targetClass.EnsureFiles(targetProjectPath);
				targetClass.AddFieldsToFile(targetProjectPath, targetClass.DesignerFilePath, comparison.FieldsToAdd);
				//If does not exist, create it
				//For each missing field, add it
			}
			foreach(var webPage in sourceProject.WebPageList)
			{
				var targetClass = targetProject.TargetClassList.SingleOrDefault(i=>i.SourceClassFullName == webPage.ClassFullName);
			}
		}

		private static string GenerateTargetFilePath(string basePath, SourceWebPage page)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(page.ClassFullName, out typeName, out typeNamespace);
			if(!string.IsNullOrEmpty(typeName))
			{
				return Path.Combine(basePath, "Client", "Pages", typeNamespace, typeName + "Page.designer.cs");
			}
			else 
			{
				return Path.Combine(basePath, "Client", "Pages", typeName + "Page.designer.cs");
			}
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
					CSClass classObject = CSClass.Parse(namespaceNode, typeDefinitionNode, pageObject.PageUrl);
					PopulatePageObject(pageObject, classObject);
				}
			}
		}

		private static void PopulatePageObject(SourceMasterPage pageObject, CSClass classObject)
		{
			pageObject.ClassFullName = classObject.ClassFullName;
			foreach (var fieldObject in classObject.FieldList)
			{
				switch(fieldObject.TypeClassFullName)
				{
					case "System.Web.UI.WebControls.Literal":
						{
							var control = new LiteralControl
							{
								FieldName = fieldObject.FieldName,
								ClassName = fieldObject.TypeClassName,
								NamespaceName = fieldObject.TypeNamespaceName
							};
							pageObject.Controls.Add(control);
						} 
						break;
					case "System.Web.UI.WebControls.ContentPlaceHolder":
						pageObject.ContentHolderIDs.Add(fieldObject.FieldName);
						break;
					default:
						{
							var control = new SourceWebControl()
							{
								FieldName = fieldObject.FieldName,
								ClassName = fieldObject.TypeClassName,
								NamespaceName = fieldObject.TypeNamespaceName
							};
							pageObject.Controls.Add(control);
						}
						break;
				}
			}
		}


		private static List<string> GetMasterFileList(XDocument xdoc)
		{
			List<string> masterFileList = new List<string>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == "Content" && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(".master")));
			foreach (var node in nodes)
			{
				string path = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				masterFileList.Add(path);
			}
			return masterFileList;
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

		private static List<string> GetClassFiles(XDocument xdoc)
		{
			List<string> classFileList = new List<string>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == "Compile" && i.Attributes().Any(j => j.Name.LocalName == "Include" && j.Value.EndsWith(".cs")));
			foreach (var node in nodes)
			{
				string path = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				classFileList.Add(path);
			}
			return classFileList;
		}
	}
}
