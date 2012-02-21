﻿using System;
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

		[Test]
		public void BasicPageControls()
		{
			var projectParser = new Mock<ProjectParser>(null,null);
			CSProjectFile projectFile = new CSProjectFile()
			{
				ClassList = new List<CSClass>()
				{
					new CSClass 
					{
						ClassFullName = "Test1.TestClass1",
						FieldList = new List<CSField>()
						{
							new CSField { FieldName="form1", ProtectionLevel=EnumProtectionLevel.Protected, TypeFullName="global::System.Web.UI.HtmlControls.HtmlForm"},
							new CSField { FieldName="_lblTest", ProtectionLevel=EnumProtectionLevel.Protected, TypeFullName="global::System.Web.UI.WebControls.Label" }
						}
					}
				},
				WebFormContainers = new List<WebFormContainer>()
				{
					new WebFormContainer 
					{
						ClassFullName="Test1.TestClass1", 
						CodeBehindFile="TestClass.aspx.cs", 
						FilePath="C:\\Test\\TestClass.aspx", 
						ContainerType= EnumWebFormContainerType.WebPage,
						Controls = new List<WebFormServerControl>()
						{
							new WebFormServerControl { TagName="form", ControlID="form1" },
							new WebFormServerControl { TagName="asp:label", ControlID="_lblTest" }
						}
					}
				},
				ClassFileDependencyList = new List<ClassFileDependency>()
				{
					new ClassFileDependency { ClassFullName="Test1.TestClass1", DependentUponFile="TestClass.aspx"}
				}
			};
			projectParser.Setup(i=>i.ParseString(It.IsAny<string>(), It.IsAny<string>())).Returns(projectFile);
			var parser = new SourceWebModelParser(projectParser.Object);
			var result = parser.LoadString(string.Empty, "C:\\Test\\Test.csproj");

			Assert.AreEqual(1, result.WebPageList.Count);
			Assert.AreEqual("/TestClass.aspx", result.WebPageList[0].PageUrl);
			Assert.AreEqual("Test1.TestClass1", result.WebPageList[0].ClassFullName);

			Assert.AreEqual(2, result.WebPageList[0].Controls.Count);
			Assert.AreEqual("System.Web.UI.HtmlControls.HtmlForm", result.WebPageList[0].Controls[0].ClassFullName);
			Assert.AreEqual("form1", result.WebPageList[0].Controls[0].FieldName);
			Assert.AreEqual("System.Web.UI.WebControls.Label", result.WebPageList[0].Controls[1].ClassFullName);
			Assert.AreEqual("_lblTest", result.WebPageList[0].Controls[1].FieldName);
		}

		[Test]
		public void BasicUserControlControls()
		{
			var projectParser = new Mock<ProjectParser>(null, null);
			CSProjectFile projectFile = new CSProjectFile()
			{
				ClassList = new List<CSClass>()
				{
					new CSClass 
					{
						ClassFullName = "Test1.TestClass1",
						FieldList = new List<CSField>()
						{
							new CSField { FieldName="_lblTest", ProtectionLevel=EnumProtectionLevel.Protected, TypeFullName="global::System.Web.UI.WebControls.Label" },
							new CSField { FieldName="_txtTest", ProtectionLevel=EnumProtectionLevel.Protected, TypeFullName="global::System.Web.UI.WebControls.TextBox" }
						}
					}
				},
				WebFormContainers = new List<WebFormContainer>()
				{
					new WebFormContainer 
					{
						ClassFullName="Test1.TestClass1", 
						CodeBehindFile="TestClass.ascx.cs", 
						FilePath="C:\\Test\\TestClass.ascx", 
						ContainerType= EnumWebFormContainerType.UserControl,
						Controls = new List<WebFormServerControl>()
						{
							new WebFormServerControl { TagName="asp:label", ControlID="_lblTest" },
							new WebFormServerControl { TagName="asp:textbox", ControlID="_txtTest" }
						}
					}
				},
				ClassFileDependencyList = new List<ClassFileDependency>()
				{
					new ClassFileDependency { ClassFullName="Test1.TestClass1", DependentUponFile="TestClass.ascx"}
				}
			};
			projectParser.Setup(i => i.ParseString(It.IsAny<string>(), It.IsAny<string>())).Returns(projectFile);
			var parser = new SourceWebModelParser(projectParser.Object);
			var result = parser.LoadString(string.Empty, "C:\\Test\\Test.csproj");

			Assert.AreEqual(1, result.UserControlList.Count);
			Assert.AreEqual("Test1.TestClass1", result.UserControlList[0].ClassFullName);

			Assert.AreEqual(2, result.UserControlList[0].Controls.Count);
			Assert.AreEqual("System.Web.UI.WebControls.Label", result.UserControlList[0].Controls[0].ClassFullName);
			Assert.AreEqual("_lblTest", result.UserControlList[0].Controls[0].FieldName);
			Assert.AreEqual("System.Web.UI.WebControls.TextBox", result.UserControlList[0].Controls[1].ClassFullName);
			Assert.AreEqual("_txtTest", result.UserControlList[0].Controls[1].FieldName);
		}
	}
}
