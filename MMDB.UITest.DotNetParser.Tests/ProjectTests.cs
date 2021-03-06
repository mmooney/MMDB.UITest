﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Xml.Linq;
using MMDB.UITest.DotNetParser.WebForms;

namespace MMDB.UITest.DotNetParser.Tests
{
	public class ProjectTests
	{
		public class ParseString
		{
			[Test]
			public void TestBasicProject()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<PropertyGroup>
						<Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
							<OutputType>Library</OutputType>
							<AppDesignerFolder>Properties</AppDesignerFolder>
							<RootNamespace>TestRootNamespace</RootNamespace>
							<RestorePackages>true</RestorePackages>
						</PropertyGroup>
						<ItemGroup>
							<Compile Include=""TestClass1.cs"" />
						</ItemGroup>
					</Project>
				";
				Mock<ClassParser> classParser = new Mock<ClassParser>();
				List<CSClass> classList = new List<CSClass>();
				var testClass1 = new CSClass()
				{
					ClassName = "TestClass1",
					NamespaceName = "Test.Test1"
				};
				classList.Add(testClass1);
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>())).Returns(classList);

				ProjectParser parser = new ProjectParser(classParser.Object);

				CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");
			
				Assert.AreEqual("TestRootNamespace", project.RootNamespace);
				Assert.AreEqual(1, project.ClassList.Count);
				Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);
			}

			[Test]
			public void TestWebFormFiles()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestClass1.aspx.cs"">
								<DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
						<ItemGroup>
							<Content Include=""TestClass1.aspx"" />
							<Content Include=""TestControl.ascx"" />
							<Content Include=""TestMaster.Master"" />
						</ItemGroup>
					</Project>
				";
				Mock<ClassParser> classParser = new Mock<ClassParser>();
				List<CSClass> classList = new List<CSClass>();
				var testClass1 = new CSClass()
				{
					ClassName = "TestClass1",
					NamespaceName = "Test.Test1"
				};
				classList.Add(testClass1);
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>())).Returns(classList);

				Mock<CSWebFormParser> webFormParser =new Mock<CSWebFormParser>();
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx"))
					.Returns(new WebFormContainer { ClassFullName = "Test.Test1.TestClass1", CodeBehindFile = "TestClass1.aspx.cs", ContainerType = EnumWebFormContainerType.WebPage });
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestControl.ascx"))
					.Returns(new WebFormContainer { ClassFullName = "Test.Test1.TestControl", CodeBehindFile = "TestControl.ascx.cs", ContainerType = EnumWebFormContainerType.UserControl });
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestMaster.Master"))
					.Returns(new WebFormContainer { ClassFullName = "Test.Test1.TestMaster", CodeBehindFile = "TestMaster.Master.cs", ContainerType = EnumWebFormContainerType.MasterPage });

				ProjectParser parser = new ProjectParser(classParser.Object, webFormParser.Object);

				CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");

				Assert.AreEqual(1, project.ClassList.Count);
				Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);

				Assert.AreEqual(3, project.WebFormContainers.Count);
				Assert.AreEqual("Test.Test1.TestClass1", project.WebFormContainers[0].ClassFullName);
				Assert.AreEqual("Test.Test1.TestControl", project.WebFormContainers[1].ClassFullName);
				Assert.AreEqual("Test.Test1.TestMaster", project.WebFormContainers[2].ClassFullName);
			}

			[Test]
			public void TestDependentFiles()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestClass1.aspx.cs"">
								<DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
							<Compile Include=""TestClass1.aspx.SomethingElse.cs"">
								<DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
							<Compile Include=""TestClass2.aspx.cs"">
								<DependentUpon>TestClass2.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
						<ItemGroup>
							<Content Include=""TestClass1.aspx"" />
							<Content Include=""TestClass2.aspx"" />
						</ItemGroup>
					</Project>
				";
				Mock<ClassParser> classParser = new Mock<ClassParser>();
				List<CSClass> classList = new List<CSClass>();
				var testClass1 = new CSClass()
				{
					ClassFullName = "Test.Test1.TestClass1",
					PropertyList = new List<CSProperty>() {new CSProperty { PropertyName = "TestProperty1", TypeName = "int" }}
				};
				var testClass1SomethingElse = new CSClass()
				{
					ClassFullName = "Test.Test1.TestClass1",
					PropertyList = new List<CSProperty>() {new CSProperty { PropertyName = "TestProperty2", TypeName = "int" }}
				};
				var testClass2 = new CSClass()
				{
					ClassFullName = "Test.Test2.TestClass2",
					PropertyList = new List<CSProperty>() {new CSProperty { PropertyName = "TestProperty3", TypeName = "int" }}
				};
				classList.Add(testClass1);
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>()))
							.Returns((string filePath, string projectPath, IEnumerable<CSClass> inputClassList) => AppendClassToList(inputClassList, testClass1));
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx.SomethingElse.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>()))
							.Returns((string filePath, string projectPath, IEnumerable<CSClass> inputClassList) => AppendClassToList(inputClassList, testClass1SomethingElse));
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestClass2.aspx.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>()))
							.Returns((string filePath, string projectPath, IEnumerable<CSClass> inputClassList) => AppendClassToList(inputClassList, testClass2));
			
				Mock<CSWebFormParser> webFormParser = new Mock<CSWebFormParser>();
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestClass1.aspx"))
							.Returns(new WebFormContainer { ClassFullName = "Test.Test1.TestClass1", CodeBehindFile = "TestClass1.aspx", ContainerType = EnumWebFormContainerType.WebPage });
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestClass2.aspx"))
							.Returns(new WebFormContainer { ClassFullName = "Test.Test2.TestClass2", CodeBehindFile = "TestClass2.aspx", ContainerType = EnumWebFormContainerType.WebPage });

				ProjectParser parser = new ProjectParser(classParser.Object, webFormParser.Object);

				CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");

				Assert.AreEqual(2, project.ClassList.Count);

				Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);
				Assert.AreEqual(2, project.ClassList[0].PropertyList.Count);

				Assert.AreEqual("Test.Test2.TestClass2", project.ClassList[1].ClassFullName);
				Assert.AreEqual(1, project.ClassList[1].PropertyList.Count);

				Assert.AreEqual(2, project.ClassFileDependencyList.Count);

				Assert.AreEqual("TestClass1", project.ClassFileDependencyList[0].ClassName);
				Assert.AreEqual("Test.Test1", project.ClassFileDependencyList[0].NamespaceName);
				Assert.AreEqual("Test.Test1.TestClass1", project.ClassFileDependencyList[0].ClassFullName);
				Assert.AreEqual("TestClass1.aspx", project.ClassFileDependencyList[0].DependentUponFile);

				Assert.AreEqual("TestClass2", project.ClassFileDependencyList[1].ClassName);
				Assert.AreEqual("Test.Test2", project.ClassFileDependencyList[1].NamespaceName);
				Assert.AreEqual("Test.Test2.TestClass2", project.ClassFileDependencyList[1].ClassFullName);
				Assert.AreEqual("TestClass2.aspx", project.ClassFileDependencyList[1].DependentUponFile);
			}

			[Test] 
			public void TestRelativeDependentUponFilePath()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs"">
								<DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
							<Compile Include=""TestDirectory1\TestDirectory2\TestClass2.aspx.cs"">
								<DependentUpon>TestClass2.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
						<ItemGroup>
							<Content Include=""TestDirectory\TestClass1.aspx"" />
							<Content Include=""TestDirectory1\TestDirectory2\TestClass2.aspx"" />
						</ItemGroup>
					</Project>
				";
				Mock<ClassParser> classParser = new Mock<ClassParser>();
				List<CSClass> classList = new List<CSClass>();
				var testClass1 = new CSClass()
				{
					ClassFullName = "Test.Test1.TestClass1"
				};
				var testClass2 = new CSClass()
				{
					ClassFullName = "Test.Test2.TestClass2"
				};
				classList.Add(testClass1);
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestDirectory\\TestClass1.aspx.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>()))
							.Returns((string filePath, string projectPath, IEnumerable<CSClass> inputClassList) => AppendClassToList(inputClassList, testClass1));
				classParser.Setup(i => i.ParseFile("C:\\Test\\TestDirectory1\\TestDirectory2\\TestClass2.aspx.cs", It.IsAny<string>(), It.IsAny<IEnumerable<CSClass>>()))
							.Returns((string filePath, string projectPath, IEnumerable<CSClass> inputClassList) => AppendClassToList(inputClassList, testClass2));

				Mock<CSWebFormParser> webFormParser = new Mock<CSWebFormParser>();
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestDirectory\\TestClass1.aspx"))
							.Returns(new WebFormContainer { ClassFullName = "Test.Test1.TestClass1", CodeBehindFile = "TestClass1.aspx.cs", ContainerType = EnumWebFormContainerType.WebPage });
				webFormParser.Setup(i => i.ParseFile("C:\\Test\\TestDirectory1\\TestDirectory2\\TestClass2.aspx"))
							.Returns(new WebFormContainer { ClassFullName = "Test.Test2.TestClass2", CodeBehindFile = "TestClass2.aspx.cs", ContainerType = EnumWebFormContainerType.WebPage });

				ProjectParser parser = new ProjectParser(classParser.Object, webFormParser.Object);

				CSProjectFile project = parser.ParseString(data, "C:\\Test\\TestProject.csproj");

				Assert.AreEqual(2, project.ClassList.Count);

				Assert.AreEqual("Test.Test1.TestClass1", project.ClassList[0].ClassFullName);
				Assert.AreEqual("Test.Test2.TestClass2", project.ClassList[1].ClassFullName);

				Assert.AreEqual(2, project.ClassFileDependencyList.Count);
				Assert.AreEqual("Test.Test1.TestClass1", project.ClassFileDependencyList[0].ClassFullName);
				Assert.AreEqual("TestDirectory\\TestClass1.aspx", project.ClassFileDependencyList[0].DependentUponFile);
				Assert.AreEqual("Test.Test2.TestClass2", project.ClassFileDependencyList[1].ClassFullName);
				Assert.AreEqual("TestDirectory1\\TestDirectory2\\TestClass2.aspx", project.ClassFileDependencyList[1].DependentUponFile);

				Assert.AreEqual(2, project.WebFormContainers.Count);

				Assert.AreEqual("Test.Test1.TestClass1", project.WebFormContainers[0].ClassFullName);
				Assert.AreEqual("TestClass1.aspx.cs", project.WebFormContainers[0].CodeBehindFile);
				Assert.AreEqual(EnumWebFormContainerType.WebPage, project.WebFormContainers[0].ContainerType);

				Assert.AreEqual("Test.Test2.TestClass2", project.WebFormContainers[1].ClassFullName);
				Assert.AreEqual("TestClass2.aspx.cs", project.WebFormContainers[1].CodeBehindFile);
				Assert.AreEqual(EnumWebFormContainerType.WebPage, project.WebFormContainers[1].ContainerType);
			}

			private List<CSClass> AppendClassToList(IEnumerable<CSClass> inputClassList, CSClass classObject)
			{
				if (inputClassList == null)
				{
					inputClassList = new List<CSClass>();
				}
				var newList = new List<CSClass>(inputClassList ?? new CSClass[] { });
				var existingClass = inputClassList.SingleOrDefault(i => i.ClassFullName == classObject.ClassFullName);
				if (existingClass == null)
				{
					newList.Add(classObject);
				}
				else
				{
					existingClass.AttributeList = existingClass.AttributeList.Union(classObject.AttributeList).ToList();
					existingClass.PropertyList = existingClass.PropertyList.Union(classObject.PropertyList).ToList();
					existingClass.FieldList = existingClass.FieldList.Union(classObject.FieldList).ToList();
				}
				return newList;
			}
		}

		public class EnsureFileInclude
		{

			[Test]
			public void TestEnsureFileInclude()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
					</Project>
				";
				ProjectParser parser = new ProjectParser();
				bool anyChange;
				string actualResult = parser.EnsureFileInclude(data, "C:\\Test\\TestProject.csproj", "C:\\Test\\TestDirectory\\TestClass1.aspx.cs", "C:\\Test\\TestDirectory\\TestClass1.aspx", out anyChange);
				Assert.IsTrue(anyChange);
				string expectedResult =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs"">
							  <DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
					</Project>
				";
				this.CompareXml(expectedResult, actualResult);
			}

			[Test]
			public void TestEnsureFileIncludeRemoveDependentNode()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs"">
							  <DependentUpon>SomethingElse.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
					</Project>
				";
				ProjectParser parser = new ProjectParser();
				bool anyChange;
				string actualResult = parser.EnsureFileInclude(data, "C:\\Test\\TestProject.csproj", "C:\\Test\\TestDirectory\\TestClass1.aspx.cs", null, out anyChange);
				Assert.IsTrue(anyChange);
				string expectedResult =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs""/>
						</ItemGroup>
					</Project>
				";
				this.CompareXml(expectedResult, actualResult);
			}

			[Test]
			public void TestEnsureFileIncludeExistingDependentNode()
			{
				string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs"">
							  <DependentUpon>SomethingElse.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
					</Project>
				";
				ProjectParser parser = new ProjectParser();
				bool anyChange;
				string actualResult = parser.EnsureFileInclude(data, "C:\\Test\\TestProject.csproj", "C:\\Test\\TestDirectory\\TestClass1.aspx.cs", "C:\\Test\\TestDirectory\\TestClass1.aspx", out anyChange);
				Assert.IsTrue(anyChange);
				string expectedResult =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
						<ItemGroup>
							<Compile Include=""TestDirectory\TestClass1.aspx.cs"">
							  <DependentUpon>TestClass1.aspx</DependentUpon>
							</Compile>
						</ItemGroup>
					</Project>
				";
				this.CompareXml(expectedResult, actualResult);
			}

			[Test]
			public void TestEnsureFileIncludeInvalidProjectFile()
			{
				var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
					<test>
					</test>
				";

				ProjectParser parser = new ProjectParser();
				bool anyChange;
				Assert.Throws(typeof(InvalidDataException), ()=>{parser.EnsureFileInclude(data, "C:\\Test\\Test.csproj", "C:\\Test\\TestFile.aspx", null, out anyChange);});
			} 

			private void CompareXml(string expectedResult, string actualResult)
			{
				var expectedResultXml = XDocument.Parse(expectedResult);
				var actualResultXml = XDocument.Parse(actualResult);
				XmlAssert.AreEqual(expectedResultXml.Root, actualResultXml.Root);
			}
		}

	}
}
