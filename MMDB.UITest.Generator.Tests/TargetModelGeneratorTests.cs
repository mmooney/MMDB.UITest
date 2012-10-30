using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.Generator.Library;
using Moq;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.Tests
{
	public class TargetModelGeneratorTests
	{
		[TestFixture]
		public class CompareProject 
		{
			[Test]
			public void OneSourceClass_NewTargetProject_ShouldCreateTargetClass()
			{
				var sourceProject = new SourceWebProject 
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace"
				};
				var targetModelGenerator = new TargetModelGenerator();
				var projectResult = targetModelGenerator.CompareProject(targetProject, sourceProject);
				Assert.AreEqual(1, projectResult.ClassesToAdd.Count);
				Assert.AreEqual(0, projectResult.ClassesToUpdate.Count);
				TestValidators.ValidateTargetClassComparisonResult(projectResult.ClassesToAdd[0],
																	@"Client\Pages\Test1\TestItemPageClient.designer.cs",
																	@"Client\Pages\Test1\TestItemPageClient.cs",
																	@"TestSourceNamespace.Test1.TestItem",
																	@"TestTargetNamespace.Client.Pages.Test1.TestItemPageClient", 
																	EnumSourceObjectType.WebPage,
																	"TestWebPage.aspx");
			}

			[Test]
			public void DifferentSourceNamespace_CreatesTargetClassWithDifferentNamespace()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "SomeOtherNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace"
				};
				var targetModelGenerator = new TargetModelGenerator();
				var projectResult = targetModelGenerator.CompareProject(targetProject, sourceProject);
				Assert.AreEqual(1, projectResult.ClassesToAdd.Count);
				Assert.AreEqual(0, projectResult.ClassesToUpdate.Count);
				TestValidators.ValidateTargetClassComparisonResult(projectResult.ClassesToAdd[0],
																	@"Client\Pages\SomeOtherNamespace\Test1\TestItemPageClient.designer.cs", 
																	@"Client\Pages\SomeOtherNamespace\Test1\TestItemPageClient.cs", 
																	@"SomeOtherNamespace.Test1.TestItem", 
																	@"TestTargetNamespace.Client.Pages.SomeOtherNamespace.Test1.TestItemPageClient", 
																	EnumSourceObjectType.WebPage, 
																	"TestWebPage.aspx");
			}

			[Test]
			public void ExistingTargetClassWithExpectedNameAndLocation_ShouldUpdateExistingClass()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"Client\Pages\Test1\TestItemPageClient.designer.cs",
							UserFilePath = @"Client\Pages\Test1\TestItemPageClient.cs",
							TargetClassFullName = "TestTargetNamespace.Client.Pages.Test1.TestItemPageClient",
							SourceObjectType = EnumSourceObjectType.WebPage,
							ExpectedUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				var projectResult = targetModelGenerator.CompareProject(targetProject, sourceProject);
				Assert.AreEqual(0, projectResult.ClassesToAdd.Count);
				Assert.AreEqual(1, projectResult.ClassesToUpdate.Count);
			}

			[Test]
			public void ExistingTargetClassInDifferentLocation_ShouldUpdateExistingClass()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"SomeOtherLocation\TestItemPageClient.designer.cs",
							UserFilePath = @"SomeOtherLocation\TestItemPageClient.cs",
							TargetClassFullName = "SomeOtherLocation.TestItemPageClient",
							SourceObjectType = EnumSourceObjectType.WebPage,
							ExpectedUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				var projectResult = targetModelGenerator.CompareProject(targetProject, sourceProject);
				Assert.AreEqual(0, projectResult.ClassesToAdd.Count);
				Assert.AreEqual(1, projectResult.ClassesToUpdate.Count);
			}

			[Test]
			public void ExistingTargetClassWithDifferentName_ShouldUpdateExistingTargetClass()
			{
				var sourceProject = new SourceWebProject
				{
					WebPageList = new List<SourceWebPage>()
					{
						new SourceWebPage
						{
							ClassFullName = "TestSourceNamespace.Test1.TestItem",
							PageUrl = "TestWebPage.aspx"
						}
					},
					RootNamespace = "TestSourceNamespace"
				};

				var targetProject = new TargetProject
				{
					RootNamespace = "TestTargetNamespace",
					TargetClassList = new List<TargetClass>() 
					{
						new TargetClass
						{
							SourceClassFullName = "TestSourceNamespace.Test1.TestItem",
							DesignerFilePath = @"SomeOtherLocation\SomeOtherClassName.designer.cs",
							UserFilePath = @"SomeOtherLocation\SomeOtherClassName.cs",
							TargetClassFullName = "SomeOtherLocation.SomeOtherClassName",
							SourceObjectType = EnumSourceObjectType.WebPage,
							ExpectedUrl = "TestWebPage.aspx"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				var projectResult = targetModelGenerator.CompareProject(targetProject, sourceProject);
				Assert.AreEqual(0, projectResult.ClassesToAdd.Count);
				Assert.AreEqual(1, projectResult.ClassesToUpdate.Count);
				TestValidators.ValidateTargetClassComparisonResult(projectResult.ClassesToUpdate[0],
																	@"SomeOtherLocation\SomeOtherClassName.designer.cs", 
																	@"SomeOtherLocation\SomeOtherClassName.cs", 
																	"TestSourceNamespace.Test1.TestItem", 
																	"SomeOtherLocation.SomeOtherClassName", 
																	EnumSourceObjectType.WebPage,
																	"TestWebPage.aspx");
			}

		}

		[TestFixture]
		public class CompareClass
		{
			[Test]
			public void NewTargetClassWithLink_ShouldCreateNewTargetClassWithLink() 
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "NewLinkField"
				        }
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetField: targetClass.TargetFieldList[0],
											isDirty:true, 
											sourceClassFullName:"System.Web.UI.WebControls.HyperLink",
											sourceFieldName: "NewLinkField",
											targetControlType: EnumTargetControlType.Link,
											targetFieldName: "NewLinkField");
			}

			[Test]
			public void NewTargetClassWithTextBox_ShouldCreateNewTargetClassWithTextBox()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
						new SourceWebControl
						{
				            ClassFullName = "System.Web.UI.WebControls.TextBox",
				            FieldName = "TestTargetField"
						}
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0], 
											isDirty:true,
											sourceClassFullName: "System.Web.UI.WebControls.TextBox",
											sourceFieldName:"TestTargetField",
											targetControlType: EnumTargetControlType.TextBox,
											targetFieldName: "TestTargetField");
			}

			[Test]
			public void NewTargetClassWithLabel_ShouldCreateNewTargetClassWithLabel()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
						new SourceWebControl
						{
				            ClassFullName = "System.Web.UI.WebControls.Label",
				            FieldName = "TestTargetField"
						}
				    }
				};
				TargetClass targetClass = new TargetClass();
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty:true,
											sourceClassFullName:"System.Web.UI.WebControls.Label",
											sourceFieldName:"TestTargetField",
											targetControlType: EnumTargetControlType.Label,
											targetFieldName:"TestTargetField");
			}

			[Test]
			public void ExistingClassExistingIrrelevantFields() 
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "NewTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "TestSourcenamespace.TestSourceClass",
							SourceFieldName = "ExistingTargetField",
							TargetControlType = EnumTargetControlType.Unknown,
							TargetFieldName = "ExistingTargetFieldName"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(2, targetClass.TargetFieldList.Count);

				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty:false,
											sourceClassFullName: "TestSourcenamespace.TestSourceClass",
											sourceFieldName: "ExistingTargetField",
											targetControlType: EnumTargetControlType.Unknown,
											targetFieldName: "ExistingTargetFieldName");

				TestValidators.ValidateTargetField(targetClass.TargetFieldList[1],
											isDirty:true,
											sourceClassFullName: "System.Web.UI.WebControls.HyperLink",
											sourceFieldName: "NewTargetField", 
											targetControlType: EnumTargetControlType.Link, 
											targetFieldName: "NewTargetField");
			}

			[Test]
			public void ExistingClassExistingField()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "System.Web.UI.WebControls.HyperLink",
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.Link,
							TargetFieldName = "TestTargetField"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty:false,
											sourceClassFullName: "System.Web.UI.WebControls.HyperLink",
											sourceFieldName: "TestTargetField",
											targetControlType: EnumTargetControlType.Link, 
											targetFieldName: "TestTargetField");
			}

			[Test]
			public void ExistingClassExistingFieldChangedSourceType_ShouldUpdateExistingTargetField()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "System.Web.UI.WebControls.TextBox",
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.Link,
							TargetFieldName = "TestTargetField"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty: true,
											sourceClassFullName: "System.Web.UI.WebControls.HyperLink", 
											sourceFieldName: "TestTargetField", 
											targetControlType: EnumTargetControlType.Link, 
											targetFieldName: "TestTargetField");

			}

			[Test]
			public void ExistingClassChangedTargetFieldName_ShouldKeepExistingTargetFieldName()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = "System.Web.UI.WebControls.HyperLink",
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = "System.Web.UI.WebControls.HyperLink",
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.Link,
							TargetFieldName = "TestTargetFieldChanged"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty: false,
											sourceClassFullName: typeof(System.Web.UI.WebControls.HyperLink).FullName,
											sourceFieldName: "TestTargetField",
											targetControlType: EnumTargetControlType.Link,
											targetFieldName: "TestTargetFieldChanged");
			}

			[Test]
			public void ExistingClassChangedTargetFieldType_ShouldUpdateTargetFieldType()
			{
				SourceWebPage sourceWebPage = new SourceWebPage
				{
					ClassFullName = "Test1.TestSourceClass",
					PageUrl = "TestSourceClass.aspx",
					Controls = new List<SourceWebControl>()
				    {
				        new SourceWebControl
				        {
				            ClassFullName = typeof(System.Web.UI.WebControls.HyperLink).FullName,
				            FieldName = "TestTargetField"
				        }
				    }
				};
				var targetClass = new TargetClass()
				{
					TargetFieldList = new List<TargetField>() 
					{
						new TargetField
						{
							IsDirty = false,
							SourceClassFullName = typeof(System.Web.UI.WebControls.HyperLink).FullName,
							SourceFieldName = "TestTargetField",
							TargetControlType = EnumTargetControlType.TextBox,
							TargetFieldName = "TestTargetField"
						}
					}
				};
				var targetModelGenerator = new TargetModelGenerator();
				targetClass = targetModelGenerator.UpdateClass(sourceWebPage, targetClass);
				Assert.AreEqual(1, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetClass.TargetFieldList[0],
											isDirty: true,
											sourceClassFullName: typeof(System.Web.UI.WebControls.HyperLink).FullName,
											sourceFieldName: "TestTargetField",
											targetControlType: EnumTargetControlType.Link,
											targetFieldName: "TestTargetField");
			}
		}

		//[TestFixture]
		//public class LoadProjectFile
		//{
		//    [Test]
		//    public void LoadBasicFile()
		//    {
		//        var targetModelGenerator = new TargetModelGenerator();
		//        CSProjectFile projectFile = new CSProjectFile() 
		//        {
		//            RootNamespace = "TargetNamespace",
		//            ClassList = new List<CSClass>()
		//            {
		//                new CSClass
		//                {
		//                    AttributeList
		//                }
		//            }
		//        }
		//        Mock<TargetClassManager> targetClassManager = new Mock<TargetClassManager>();
		//        var targetProject = targetModelGenerator.LoadFromProjectFile(projectFile, Guid.NewGuid().ToString());
		//        Assert.Fail();
		//    }
		//}

	}
}
