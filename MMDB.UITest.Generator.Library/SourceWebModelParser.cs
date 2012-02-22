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

		public SourceWebProject LoadFromProjectFile(CSProjectFile csProject, string projectFilePath)
		{
			SourceWebProject returnValue = new SourceWebProject()
			{
				RootNamespace = csProject.RootNamespace
			};
			var webPageList = csProject.WebFormContainers.Where(i => i.ContainerType == EnumWebFormContainerType.WebPage);
			foreach (var webPage in webPageList)
			{
				var csClass = csProject.ClassList.SingleOrDefault(i => i.ClassFullName == webPage.ClassFullName);
				if (csClass != null)
				{
					SourceWebPage sourceWebPage = new SourceWebPage()
					{
						ClassFullName = webPage.ClassFullName,
						PageUrl = ConvertToUrl(webPage.FilePath, projectFilePath),
						Controls = LoadControls(webPage, csClass)
					};
					returnValue.WebPageList.Add(sourceWebPage);
				}
			}
			var masterPageList = csProject.WebFormContainers.Where(i => i.ContainerType == EnumWebFormContainerType.MasterPage);
			foreach (var masterPage in masterPageList)
			{
				SourceMasterPage sourceMasterPage = new SourceMasterPage()
				{
					ClassFullName = masterPage.ClassFullName,
					PageUrl = ConvertToUrl(masterPage.FilePath, projectFilePath)
				};
				returnValue.MasterPageList.Add(sourceMasterPage);
			}
			var userControlList = csProject.WebFormContainers.Where(i => i.ContainerType == EnumWebFormContainerType.UserControl);
			foreach (var userControl in userControlList)
			{
				var csClass = csProject.ClassList.SingleOrDefault(i => i.ClassFullName == userControl.ClassFullName);
				SourceUserControl sourceUserControl = new SourceUserControl()
				{
					ClassFullName = userControl.ClassFullName,
					Controls = LoadControls(userControl, csClass)
				};
				returnValue.UserControlList.Add(sourceUserControl);
			}
			return returnValue;
		}

		private List<SourceWebControl> LoadControls(WebFormContainer webPage, CSClass csClass)
		{
			List<SourceWebControl> returnList = new List<SourceWebControl>();
			foreach(var serverControl in webPage.Controls)
			{
				var classField = csClass.FieldList.SingleOrDefault(i=>i.FieldName == serverControl.ControlID);
				if(classField != null)
				{
					SourceWebControl sourceWebControl = new SourceWebControl
					{
						ClassFullName = classField.TypeFullName,
						FieldName = (serverControl.Prefix??string.Empty) + classField.FieldName
					};
					returnList.Add(sourceWebControl);
				}
			}
			return returnList;
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
