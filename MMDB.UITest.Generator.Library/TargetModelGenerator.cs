using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.Library
{
	public class TargetModelGenerator
	{
		protected TargetClassManager TargetClassManager { get; set; }

		public TargetModelGenerator(TargetClassManager targetClassManager = null)
		{
			this.TargetClassManager = targetClassManager ?? new TargetClassManager();
		}

		public TargetProject LoadFromProjectFile(string targetProjectPath)
		{
			ProjectParser parser = new ProjectParser();
			var csProject = parser.ParseFile(targetProjectPath);
			return LoadFromProjectFile(csProject, targetProjectPath);
		}

		public TargetProject LoadFromProjectFile(CSProjectFile csProject, string targetProjectPath)
		{
			TargetProject returnValue = new TargetProject()
			{
				Directory = Path.GetDirectoryName(targetProjectPath),
				FileName = Path.GetFileName(targetProjectPath),
				RootNamespace = csProject.RootNamespace
			};
			foreach (var csClass in csProject.ClassList)
			{
				var targetClass = this.TargetClassManager.TryLoadTargetClass(csClass);
				if (targetClass != null)
				{
					returnValue.TargetClassList.Add(targetClass);
				}
			}
			return returnValue;
		}

		public TargetProjectComparisonResult CompareProject(TargetProject targetProject, SourceWebProject sourceProject)
		{
		    TargetProjectComparisonResult result = new TargetProjectComparisonResult();
			//foreach (var masterPage in sourceProject.MasterPageList)
			//{
			//    //Find existing object
			//    var targetClass = targetProject.TargetClassList.SingleOrDefault(i => i.SourceClassFullName == masterPage.ClassFullName);
			//    if (targetClass == null)
			//    {
			//        TargetClassComparisonResult classResult = new TargetClassComparisonResult()
			//        {
			//            a
			//        }
			//        //If does not exist, create it
			//        targetClass = TargetClass.Create(targetProject, sourceProject, masterPage);
			//        result.ClassesToAdd.Add(targetClass);
			//    }
			//    targetClass.PageUrl = masterPage.PageUrl;
			//    targetClass.TargetObjectType = EnumTargetObjectType.MasterPage;
			//    this.TargetClassManager.UpdateTargetClass(masterPage, targetClass);
			//}
		    foreach (var sourcePage in sourceProject.WebPageList)
		    {
		        var targetClass = targetProject.TargetClassList.SingleOrDefault(i => i.SourceClassFullName == sourcePage.ClassFullName);
		        if (targetClass == null)
		        {
					TargetClassComparisonResult classResult = this.CompareClass(sourceProject, sourcePage, targetProject, null);
		            result.ClassesToAdd.Add(classResult);
		        }
		    }
		    return result;
		}

		//public TargetProject UpdateProject(TargetProject targetProject, SourceWebProject sourceProject)
		//{
		//    //List<TargetClass> classesToAddToProject = new List<TargetClass>();
		//    //foreach (var masterPage in sourceProject.MasterPageList)
		//    //{
		//    //    //Find existing object
		//    //    var targetClass = targetProject.TargetClassList.SingleOrDefault(i => i.SourceClassFullName == masterPage.ClassFullName);
		//    //    if (targetClass == null)
		//    //    {
		//    //        //If does not exist, create it
		//    //        targetClass = TargetClass.Create(targetProject, sourceProject, masterPage);
		//    //        classesToAddToProject.Add(targetClass);
		//    //    }
		//    //    targetClass.ExpectedUrl = masterPage.PageUrl;
		//    //    targetClass.TargetObjectType = EnumTargetObjectType.MasterPage;
		//    //    //For each missing field, add it
		//    //    var comparison = TargetClassComparisonResult.Compare(masterPage, targetClass);
		//    //    targetClass.EnsureFiles(targetProjectPath);
		//    //    targetClass.AddFieldsToFile(targetProjectPath, targetClass.DesignerFilePath, comparison.FieldsToAdd);
		//    //}
		//    foreach (var sourcePage in sourceProject.WebPageList)
		//    {
		//        var targetClass = targetProject.TargetClassList.FirstOrDefault(i=>i.SourceClassFullName == sourcePage.ClassFullName);
		//        if(targetClass == null)
		//        {
		//            string relativeSourceNamespace;
		//            if(sourcePage.NamespaceName == sourceProject.RootNamespace)
		//            {
		//                relativeSourceNamespace = string.Empty;
		//            }
		//            else if(sourcePage.NamespaceName.StartsWith(sourceProject.RootNamespace))
		//            {
		//                relativeSourceNamespace = sourcePage.NamespaceName.Substring(sourceProject.RootNamespace.Length + 1);
		//            }
		//            else 
		//            {
		//                relativeSourceNamespace = sourcePage.NamespaceName;
		//            }
		//            string targetClassName = sourcePage.ClassName + "PageClient";
		//            string targetDirectory = @"Client\Pages\" + relativeSourceNamespace.Replace(".","\\");;
		//            string targetNamespace = targetProject.RootNamespace + ".Client.Pages." + relativeSourceNamespace;
		//            targetClass = new TargetClass
		//            {
		//                SourceClassFullName = sourcePage.ClassFullName,
		//                TargetClassFullName = targetNamespace + "." + targetClassName,
		//                DesignerFilePath = Path.Combine(targetDirectory, targetClassName + ".designer.cs"),
		//                UserFilePath = Path.Combine(targetDirectory, targetClassName + ".cs"),
		//                TargetObjectType = EnumTargetObjectType.WebPage,
		//                ExpectedUrl = sourcePage.PageUrl
		//            };
		//            targetProject.TargetClassList.Add(targetClass);
		//        }
		//    }
		//    return targetProject;
		//}

		public TargetClassComparisonResult CompareClass(SourceWebProject sourceProject, SourceWebPage sourceClass, TargetProject targetProject, TargetClass targetClass)
		{
			TargetClassComparisonResult result;
			if(targetClass == null)
			{
				string relativeSourceNamespace;
				if (sourceClass.NamespaceName == sourceProject.RootNamespace)
				{
					relativeSourceNamespace = string.Empty;
				}
				else if (sourceClass.NamespaceName.StartsWith(sourceProject.RootNamespace))
				{
					relativeSourceNamespace = sourceClass.NamespaceName.Substring(sourceProject.RootNamespace.Length + 1);
				}
				else
				{
					relativeSourceNamespace = sourceClass.NamespaceName;
				}
				string targetClassName = sourceClass.ClassName + "PageClient";
				string targetDirectory = @"Client\Pages\" + relativeSourceNamespace.Replace(".", "\\"); ;
				string targetNamespace = targetProject.RootNamespace + ".Client.Pages." + relativeSourceNamespace;
				result = new TargetClassComparisonResult
				{
					SourceClassFullName = sourceClass.ClassFullName,
					TargetClassName = targetClassName,
					TargetNamespaceName = targetClassName,
					DesignerFileRelativePath = Path.Combine(targetDirectory, targetClassName + ".designer.cs"),
					UserFileRelativePath = Path.Combine(targetDirectory, targetClassName + ".cs"),
					SourceObjectType = EnumSourceObjectType.WebPage,
					ExpectedUrl = sourceClass.PageUrl,
					IsDirtry = true
				};
			}
			else 
			{
				result = new TargetClassComparisonResult
				{
					SourceClassFullName = targetClass.SourceClassFullName,
					TargetClassFullName = targetClass.TargetClassFullName,
					DesignerFileRelativePath = targetClass.DesignerFilePath,
					UserFileRelativePath = targetClass.UserFilePath
				};
				if(targetClass.ExpectedUrl != sourceClass.PageUrl)
				{
					result.ExpectedUrl = sourceClass.PageUrl;
					result.IsDirtry = true;
				}
				if(targetClass.SourceObjectType != sourceClass.SourceObjectType)
				{
					result.SourceObjectType = sourceClass.SourceObjectType;
					result.IsDirtry = true;
				}
			}
			foreach(var sourceControl in sourceClass.Controls)
			{
				TargetField targetControl = null;
				if(targetClass != null)
				{
					targetControl = targetClass.TargetFieldList.SingleOrDefault(i => i.SourceFieldName == sourceControl.FieldName);
				}
				if(targetControl == null)
				{
				    targetControl = new TargetField
				    {
				        SourceClassFullName = sourceControl.ClassFullName,
				        SourceFieldName = sourceControl.FieldName,
						TargetControlType = this.TargetClassManager.GetTargetControlType(sourceControl.ClassFullName),
				        IsDirty = true,
						TargetFieldName = sourceControl.FieldName
				    };
				    result.FieldsToAdd.Add(targetControl);
					result.IsDirtry = true;
				}
				else if(targetControl.SourceClassFullName != sourceControl.ClassFullName)
				{
				    targetControl.SourceClassFullName = sourceControl.ClassFullName;
					result.FieldsToUpdate.Add(targetControl);
				    targetControl.IsDirty = true;
					result.IsDirtry = true;
				}
			}
			return result;
		}

		public void UpdateProjectFile(string targetProjectPath, TargetProjectComparisonResult projectComparison)
		{
			throw new NotImplementedException();
		}
	}
}
