﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.DotNetParser
{
	public class ProjectParser
	{
		private ClassParser ClassParser { get; set; }
		private CSWebFormParser WebFormParser { get; set; }

		public class CSProjectDependency
		{
			public string FilePath { get; set; }
			public string DependentUponFilePath { get; set; }
		}

		public ProjectParser(ClassParser classParser = null, CSWebFormParser webFormParser = null )
		{
			this.ClassParser = classParser ?? new ClassParser();
			this.WebFormParser = webFormParser ?? new CSWebFormParser();
		}

		public virtual CSProjectFile ParseFile(string projectFilePath)
		{
			string data = File.ReadAllText(projectFilePath);
			return ParseString(data, projectFilePath);
		}

		public virtual CSProjectFile ParseString(string data, string projectFilePath)
		{
			string workingProjectFilePath = Path.GetFullPath(projectFilePath);
			string projectDirectory = Path.GetDirectoryName(workingProjectFilePath);
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
				//ClassParser returns new class list
				var newClassList = this.ClassParser.ParseFile(filePath, projectDirectory, null);
				//Then cycle trough and try to merge them, while building dependency list
				foreach(var newClass in newClassList)
				{
					var existingClass = returnValue.ClassList.SingleOrDefault(i=>i.ClassFullName == newClass.ClassFullName);
					if(existingClass != null)
					{
						existingClass.Merge(newClass);
					}
					else 
					{
						returnValue.ClassList.Add(newClass);
					}
					if(!string.IsNullOrEmpty(classFile.DependentUponFilePath))
					{
						var anyDependency = returnValue.ClassFileDependencyList.Any(i=>i.ClassFullName == newClass.ClassFullName
																						&& i.DependentUponFile.Equals(classFile.DependentUponFilePath, StringComparison.CurrentCultureIgnoreCase));
						if(!anyDependency)
						{
							var newDepdendency = new ClassFileDependency()
							{
								ClassName = newClass.ClassName,
								NamespaceName = newClass.NamespaceName,
								DependentUponFile = classFile.DependentUponFilePath
							};
							returnValue.ClassFileDependencyList.Add(newDepdendency);
						}
					}
				}
			}
			var contentFileList = GetFileList(xdoc, "Content", new string[] {".aspx", ".ascx", ".master" });
			foreach(var contentFile in contentFileList)
			{
				string filePath = Path.Combine(Path.GetDirectoryName(workingProjectFilePath), contentFile.FilePath);
				filePath = Path.GetFullPath(filePath);
				//ClassParser returns new class list
				var webFormContainer = this.WebFormParser.ParseFile(filePath);
				returnValue.WebFormContainers.Add(webFormContainer);
			}
			return returnValue;
		}

		private static List<CSProjectDependency> GetFileList(XDocument xdoc, string elementName, string extension)
		{
			return GetFileList(xdoc, elementName, new string[] { extension });
		}

		private static List<CSProjectDependency> GetFileList(XDocument xdoc, string elementName, string[] extensionList)
		{
			var returnValue = new List<CSProjectDependency>();
			var nodes = xdoc.Descendants().Where(i => i.Name.LocalName == elementName && i.Attributes().Any(j => j.Name.LocalName == "Include" && extensionList.Contains(Path.GetExtension(j.Value), StringComparer.CurrentCultureIgnoreCase)));
			foreach (var node in nodes)
			{
				var item = new CSProjectDependency();
				item.FilePath = node.Attributes().Single(i => i.Name.LocalName == "Include").Value;
				var dependentUponNode = node.Elements().SingleOrDefault(i=>i.Name.LocalName == "DependentUpon");
				if(dependentUponNode != null)
				{
					string relativeDirectory = Path.GetDirectoryName(item.FilePath);
					string dependentUponPath = dependentUponNode.Value;
					if(!string.IsNullOrEmpty(relativeDirectory))
					{
						dependentUponPath = Path.Combine(relativeDirectory, dependentUponNode.Value);
					}
					item.DependentUponFilePath = dependentUponPath;
				}
				returnValue.Add(item);
			}
			return returnValue;
		}

		public void EnsureFileInclude(string projectPath, string includeFilePath, string dependentFilePath)
		{
			string data = File.ReadAllText(projectPath);
			bool anyChange;
			string result = EnsureFileInclude(data, projectPath, includeFilePath, dependentFilePath, out anyChange);
			if(anyChange)
			{
				File.WriteAllText(projectPath, result);
			}
		}

		public string EnsureFileInclude(string data, string projectPath, string includeFilePath, string dependentFilePath, out bool anyChange)
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

			XDocument xdoc = XDocument.Parse(data);
			anyChange = false;
			var compileNode = xdoc.Descendants().SingleOrDefault(i => i.Name.LocalName == "Compile" && i.Attributes().Any(j => j.Name.LocalName == "Include" && string.Equals(j.Value, workingFilePath, StringComparison.CurrentCultureIgnoreCase)));
			if (compileNode == null)
			{
				var itemGroupNode = xdoc.Descendants().Where(i => i.Name.LocalName == "ItemGroup").OrderByDescending(i => i.Descendants().Any(j => j.Name.LocalName == "Compile")).FirstOrDefault();
				if (itemGroupNode == null)
				{
					var projectNode = xdoc.Root;
					if(projectNode.Name.LocalName != "Project")
					{
						throw new InvalidDataException("Cannot parse Project root node, found " + projectNode.Name.LocalName);
					}
					itemGroupNode = new XElement(XName.Get("ItemGroup", projectNode.Name.NamespaceName));
					projectNode.Add(itemGroupNode);
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
			return xdoc.ToString();
		}

	}
}
