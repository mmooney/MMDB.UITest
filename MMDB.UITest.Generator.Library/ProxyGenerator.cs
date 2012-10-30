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
		private static string ConvertToUrl(string filePath)
		{
			return filePath.Replace('\\','/');
		}
        
        public static void UpdateProxyProject(string targetProjectPath, SourceWebProject sourceProject)
		{
			XDocument xdoc = XDocument.Load(targetProjectPath);
			var targetModelGenerator = new TargetModelGenerator();
			TargetProject targetProject = targetModelGenerator.LoadFromProjectFile(targetProjectPath);
			var projectComparison = targetModelGenerator.CompareProject(targetProject, sourceProject);
			targetModelGenerator.UpdateProjectFile(targetProject, projectComparison);
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

		private static void PopulateMasterPageObject(SourceMasterPage pageObject, CSClass classObject)
		{
			pageObject.ClassFullName = classObject.ClassFullName;
			foreach (var fieldObject in classObject.FieldList)
			{
				switch(fieldObject.TypeFullName)
				{
					case "System.Web.UI.WebControls.ContentPlaceHolder":
						pageObject.ContentHolderIDs.Add(fieldObject.FieldName);
						break;
				}
			}
			PopulateWebPageObject(pageObject, classObject);
		}

		private static void PopulateWebPageObject(SourceWebPage pageObject, CSClass classObject)
		{
			pageObject.ClassFullName = classObject.ClassFullName;
			foreach (var fieldObject in classObject.FieldList)
			{
				switch (fieldObject.TypeFullName)
				{
					case "System.Web.UI.WebControls.Literal":
						{
							var control = new LiteralControl
							{
								FieldName = fieldObject.FieldName,
								ClassName = fieldObject.TypeName,
								NamespaceName = fieldObject.TypeNamespace
							};
							pageObject.Controls.Add(control);
						}
						break;
					default:
						{
							var control = new SourceWebControl()
							{
								FieldName = fieldObject.FieldName,
								ClassName = fieldObject.TypeName,
								NamespaceName = fieldObject.TypeNamespace
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
