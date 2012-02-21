using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using System.IO;
using System.Text.RegularExpressions;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebModelParser
	{
		private ProjectParser ProjectParser { get; set; }

		public SourceWebModelParser(ProjectParser projectParser = null)
		{
			this.ProjectParser = projectParser ?? new ProjectParser();
		}

		public SourceWebProject LoadFile(string projectFilePath)
		{
			string data = File.ReadAllText(projectFilePath);
			return this.LoadString(data, projectFilePath);
		}

		public SourceWebProject LoadString(string projectFileData, string projectFilePath)
		{
			var csProject = this.ProjectParser.ParseString(projectFileData, projectFilePath);
			SourceWebProject returnValue = new SourceWebProject()
			{
				RootNamespace = csProject.RootNamespace
			};
			var webPageList = csProject.WebFormContainers.Where(i=>i.ContainerType == EnumWebFormContainerType.WebPage);
			foreach(var webPage in webPageList)
			{
				SourceWebPage sourceWebPage = new SourceWebPage()
				{
					ClassFullName = webPage.ClassFullName,
					PageUrl = ConvertToUrl(webPage.FilePath, projectFilePath)
				};
				returnValue.WebPageList.Add(sourceWebPage);
			}
			var masterPageList = csProject.WebFormContainers.Where(i=>i.ContainerType == EnumWebFormContainerType.MasterPage);
			foreach(var masterPage in masterPageList)
			{
				SourceMasterPage sourceMasterPage = new SourceMasterPage()
				{
					ClassFullName = masterPage.ClassFullName,
					PageUrl = ConvertToUrl(masterPage.FilePath, projectFilePath)
				};
				returnValue.MasterPageList.Add(sourceMasterPage);
			}
			var userControlList = csProject.WebFormContainers.Where(i=>i.ContainerType == EnumWebFormContainerType.UserControl);
			foreach(var userControl in userControlList)
			{
				SourceUserControl sourceUserControl = new SourceUserControl()
				{
					ClassFullName = userControl.ClassFullName
				};
				returnValue.UserControlList.Add(sourceUserControl);
			}
			return returnValue;
		}

		private string ConvertToUrl(string filePath, string projectFilePath)
		{
			string projectDirectory = Path.GetDirectoryName(projectFilePath);
			string relativeFilePath = filePath.Replace(projectDirectory, "");
			return relativeFilePath.Replace("\\","/");
		}

		public void DummyFunction()
		{
			//SourceWebPage returnValue = null;
			//string aspxFile = classObject.DependentUponFilePathList.SingleOrDefault(i => i.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase));
			//if (aspxFile != null)
			//{
			//    string aspxData;
			//    using (StreamReader reader = new StreamReader(Path.Combine(Path.GetDirectoryName(projectPath), aspxFile)))
			//    {
			//        aspxData = reader.ReadToEnd();
			//        ICSharpCode.NRefactory.CSharp.CSharpParser parser = new ICSharpCode.NRefactory.CSharp.CSharpParser();
			//        var unit = parser.Parse(reader, aspxFile);
			//    }
			//    Regex re = new Regex("<%@( )*Page .+ MasterPageFile=\\\"~/([\\w\\d/.]+)\\\"");
			//    var match = re.Match(aspxData);
			//    if (match.Groups.Count > 0)
			//    {
			//        returnValue = new SourceMasterContentPage()
			//        {
			//            ClassFullName = classObject.ClassFullName,
			//            MasterPageUrl = match.Groups[match.Groups.Count - 1].Value
			//        };
			//    }
			//    else
			//    {
			//        returnValue = new SourceWebPage
			//        {
			//            ClassFullName = classObject.ClassFullName
			//        };
			//    }
			//}
			//return returnValue;
		}
	}
}
