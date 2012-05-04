using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MMDB.UITest.Generator.Library
{
	public class TargetModelGenerator
	{
		public TargetModelGenerator()
		{
		}

		//public TargetProjectComparisonResult CompareProject(TargetProject targetProject, SourceWebProject sourceProject)
		//{
		//    TargetProjectComparisonResult result = new TargetProjectComparisonResult();
		//    foreach (var masterPage in sourceProject.MasterPageList)
		//    {
		//        //Find existing object
		//        var targetClass = targetProject.TargetClassList.SingleOrDefault(i => i.SourceClassFullName == masterPage.ClassFullName);
		//        if (targetClass == null)
		//        {
		//            TargetClassComparisonResult classResult = new TargetClassComparisonResult()
		//            {
		//                a
		//            }
		//            //If does not exist, create it
		//            targetClass = TargetClass.Create(targetProject, sourceProject, masterPage);
		//            result.ClassesToAdd.Add(targetClass);
		//        }
		//        targetClass.PageUrl = masterPage.PageUrl;
		//        targetClass.TargetObjectType = EnumTargetObjectType.MasterPage;
		//        this.TargetClassManager.UpdateTargetClass(masterPage, targetClass);
		//    }
		//    foreach (var webPage in sourceProject.WebPageList)
		//    {
		//        var targetClass = targetProject.TargetClassList.SingleOrDefault(i => i.SourceClassFullName == webPage.ClassFullName);
		//        if (targetClass == null)
		//        {
		//            targetClass = TargetClass.Create(targetProject, sourceProject, webPage);
		//            result.ClassesToAdd.Add(targetClass);
		//        }
		//        targetClass.PageUrl = webPage.PageUrl;
		//        targetClass.TargetObjectType = EnumTargetObjectType.WebPage;
		//        this.TargetClassManager.UpdateTargetClass(webPage, targetClass);
		//    }
		//    return result;
		//}

		public TargetProject UpdateProject(TargetProject targetProject, SourceWebProject sourceProject)
		{
			foreach(var sourcePage in sourceProject.WebPageList)
			{
				var targetClass = targetProject.TargetClassList.FirstOrDefault(i=>i.SourceClassFullName == sourcePage.ClassFullName);
				if(targetClass == null)
				{
					string relativeSourceNamespace;
					if(sourcePage.NamespaceName == sourceProject.RootNamespace)
					{
						relativeSourceNamespace = string.Empty;
					}
					else if(sourcePage.NamespaceName.StartsWith(sourceProject.RootNamespace))
					{
						relativeSourceNamespace = sourcePage.NamespaceName.Substring(sourceProject.RootNamespace.Length + 1);
					}
					else 
					{
						relativeSourceNamespace = sourcePage.NamespaceName;
					}
					string targetClassName = sourcePage.ClassName + "PageClient";
					string targetDirectory = @"Client\Pages\" + relativeSourceNamespace.Replace(".","\\");;
					string targetNamespace = targetProject.RootNamespace + ".Client.Pages." + relativeSourceNamespace;
					targetClass = new TargetClass
					{
						SourceClassFullName = sourcePage.ClassFullName,
						TargetClassFullName = targetNamespace + "." + targetClassName,
						DesignerFilePath = Path.Combine(targetDirectory, targetClassName + ".designer.cs"),
						UserFilePath = Path.Combine(targetDirectory, targetClassName + ".cs"),
						TargetObjectType = EnumTargetObjectType.WebPage,
						PageUrl = sourcePage.PageUrl
					};
					targetProject.TargetClassList.Add(targetClass);
				}
			}
			return targetProject;
		}

		public TargetClass UpdateClass(SourceWebPage sourceClass, TargetClass targetClass)
		{
			foreach(var sourceControl in sourceClass.Controls)
			{
				var targetControl = targetClass.TargetFieldList.SingleOrDefault(i=>i.SourceFieldName == sourceControl.FieldName);
				if(targetControl == null)
				{
				    targetControl = new TargetField
				    {
				        SourceClassFullName = sourceControl.ClassFullName,
				        SourceFieldName = sourceControl.FieldName,
						TargetControlType = GetTargetControlType(sourceControl.ClassFullName),
				        IsDirty = true,
						TargetFieldName = sourceControl.FieldName
				    };
				    targetClass.TargetFieldList.Add(targetControl);
				}
				else if(targetControl.SourceClassFullName != sourceControl.ClassFullName)
				{
				    targetControl.SourceClassFullName = sourceControl.ClassFullName;
				    targetControl.IsDirty = true;
				}
			}
			return targetClass;
		}

		private EnumTargetControlType GetTargetControlType(string sourceClassFullName)
		{
			EnumTargetControlType returnValue = EnumTargetControlType.Unknown;
			switch(sourceClassFullName)
			{
				case "System.Web.UI.WebControls.HyperLink":
					returnValue = EnumTargetControlType.Link;
					break;
				case "System.Web.UI.WebControls.TextBox":
					returnValue = EnumTargetControlType.TextBox;
					break;
				case "System.Web.UI.WebControls.Label":
					returnValue = EnumTargetControlType.Label;
					break;
			}
			return returnValue;
		}
	}
}
