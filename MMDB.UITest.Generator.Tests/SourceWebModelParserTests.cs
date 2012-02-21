using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.Generator.Library;
using MMDB.UITest.DotNetParser;
using Moq;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.Generator.Tests
{
	[TestFixture]
	class SourceWebModelParserTests
	{
		[Test]
		public void BasicTest()
		{
			CSProjectFile projectFile = new CSProjectFile()
			{
				RootNamespace = "Test.Namespace",
				ClassList = new List<CSClass>
				{
					new CSClass { ClassFullName="Test1.TestClass1", FilePathList=new List<string> {"TestClass1.aspx"} },
					new CSClass { ClassFullName="Test1.TestMaster", FilePathList=new List<string> {"TestMasterPage.Master"} },
					new CSClass { ClassFullName="Test1.TestUserControl", FilePathList=new List<string> {"TestUserControl.ascx"} }
				},
				WebFormContainers = new List<WebFormContainer>()
				{
					new WebFormContainer { ClassFullName="Test1.TestClass1", CodeBehindFile="TestClass1.aspx.cs", ContainerType=EnumWebFormContainerType.WebPage, FilePath="C:\\Test\\TestClass1.aspx"},
					new WebFormContainer { ClassFullName="Test1.TestMasterPage", CodeBehindFile="TestMasterPage.Master.cs", ContainerType=EnumWebFormContainerType.MasterPage, FilePath="C:\\Test\\TestMasterPage.Master"},
					new WebFormContainer { ClassFullName="Test1.TestUserControl", CodeBehindFile="TestUserControl.ascx.cs", ContainerType=EnumWebFormContainerType.UserControl, FilePath="C:\\Test\\TestUserControl.ascx" }
				},
				ClassFileDependencyList = new List<ClassFileDependency>
				{
					new ClassFileDependency { ClassFullName="Test1.TestClass1", DependentUponFile="TestClass1.aspx" },
					new ClassFileDependency { ClassFullName="Test1.TestMasterPage", DependentUponFile="TestMasterPage.Master" },
					new ClassFileDependency { ClassFullName="Test1.TestUserControl", DependentUponFile="TestUserControl.ascx" }
				}
			};
			Mock<ProjectParser> projectParser = new Mock<ProjectParser>(null,null);
			projectParser.Setup(i=>i.ParseString(It.IsAny<string>(), It.IsAny<string>())).Returns(projectFile);
			SourceWebModelParser parser = new SourceWebModelParser(projectParser.Object);
			SourceWebProject result = parser.LoadString(string.Empty, "C:\\Test\\Test.csproj");
			Assert.IsNotNull(result);
			Assert.AreEqual(projectFile.RootNamespace, result.RootNamespace);
			
			Assert.AreEqual(1, result.WebPageList.Count);
			Assert.AreEqual("Test1.TestClass1", result.WebPageList[0].ClassFullName);
			Assert.AreEqual("/TestClass1.aspx", result.WebPageList[0].PageUrl);

			Assert.AreEqual(1, result.MasterPageList.Count);
			Assert.AreEqual("Test1.TestMasterPage", result.MasterPageList[0].ClassFullName);
			Assert.AreEqual("/TestMasterPage.Master", result.MasterPageList[0].PageUrl);

			Assert.AreEqual(1, result.UserControlList.Count);
			Assert.AreEqual("Test1.TestUserControl", result.UserControlList[0].ClassFullName);
		}
	}
}
