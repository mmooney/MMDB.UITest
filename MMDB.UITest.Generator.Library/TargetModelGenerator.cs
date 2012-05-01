using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MMDB.UITest.Generator.Library
{
	public class TargetModelGenerator
	{
		public TargetClassManager TargetClassManager { get; set; }

		public TargetModelGenerator(TargetClassManager targetClassManager)
		{
			this.TargetClassManager = targetClassManager;
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
	}
}
