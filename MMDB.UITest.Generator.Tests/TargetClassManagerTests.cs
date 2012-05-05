using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Generator.Library;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Tests
{
	public class TargetClassManagerTests
	{
		public class TryLoadTargetClass
		{
			[Test]
			public void BasicTargetClassTest()
			{
				CSClass csClass = new CSClass()
				{
					ClassFullName = "Test.Target.TargetClassName",
					AttributeList = new List<CSAttribute>() 
					{
						new CSAttribute
						{
							TypeFullName = typeof(UIClientPageAttribute).FullName,
							ArgumentList = new List<CSAttribute.CSAttributeArgument>()
							{
								new CSAttribute.CSAttributeArgument { ArgumentName = "SourceClassFullName", ArguementValue = "Test.Test1.SourceClassFullNameValue" },
								new CSAttribute.CSAttributeArgument { ArgumentName = "ExpectedUrl", ArguementValue = "SourceClassFullName.aspx" }
							}
						}
					},
					FilePathList = new List<string>()
					{
						"TargetClassName.aspx.designer.cs",
						"TargetClassName.aspx.cs"
					},
					PropertyList = new List<CSProperty>()
					{
						new CSProperty()
						{
							TypeFullName = typeof(WatiN.Core.Link).FullName,
							PropertyName = "TestLink",
							ProtectionLevel = EnumProtectionLevel.Public,
							AttributeList = new List<CSAttribute>() 
							{
								new CSAttribute() 
								{
									TypeFullName = typeof(UIClientPropertyAttribute).FullName,
									ArgumentList = new List<CSAttribute.CSAttributeArgument>()
									{
										new CSAttribute.CSAttributeArgument() { ArgumentName="SourceFieldTypeFullName", ArguementValue=typeof(System.Web.UI.WebControls.HyperLink).FullName },
										new CSAttribute.CSAttributeArgument() { ArgumentName="SourceFieldName", ArguementValue="TestLink" }
									}
								}
							}
						},
						new CSProperty()
						{
							TypeFullName = typeof(WatiN.Core.TextField).FullName,
							PropertyName = "TestTextBox",
							ProtectionLevel = EnumProtectionLevel.Public,					
							AttributeList = new List<CSAttribute>() 
							{
								new CSAttribute() 
								{
									TypeFullName = typeof(UIClientPropertyAttribute).FullName,
									ArgumentList = new List<CSAttribute.CSAttributeArgument>()
									{
										new CSAttribute.CSAttributeArgument() { ArgumentName="SourceFieldTypeFullName", ArguementValue=typeof(System.Web.UI.WebControls.TextBox).FullName },
										new CSAttribute.CSAttributeArgument() { ArgumentName="SourceFieldName", ArguementValue="TestTextBox" }
									}
								}
							}
						},
					}
				};
				var targetClassManager = new TargetClassManager();
				var targetClass = targetClassManager.TryLoadTargetClass(csClass);
				Assert.IsNotNull(targetClass);
				Assert.AreEqual("Test.Test1.SourceClassFullNameValue", targetClass.SourceClassFullName);
				Assert.AreEqual("Test.Target.TargetClassName", targetClass.TargetClassFullName);
				Assert.AreEqual(2, targetClass.TargetFieldList.Count);
				TestValidators.ValidateTargetField(targetField: targetClass.TargetFieldList[0],
											isDirty: false,
											sourceClassFullName: "System.Web.UI.WebControls.HyperLink",
											sourceFieldName: "TestLink",
											targetControlType: EnumTargetControlType.Link,
											targetFieldName: "TestLink");
				TestValidators.ValidateTargetField(targetField: targetClass.TargetFieldList[1],
											isDirty: false,
											sourceClassFullName: "System.Web.UI.WebControls.TextBox",
											sourceFieldName: "TestTextBox",
											targetControlType: EnumTargetControlType.TextBox,
											targetFieldName: "TestTextBox");
				Assert.AreEqual("SourceClassFullName.aspx", targetClass.ExpectedUrl);
			}

			[Test]
			public void SingleExpectedUrl()
			{
				Assert.Fail();
			}

			[Test]
			public void MultipleExpectedUrl()
			{
				Assert.Fail();
			}
		}
	}
}
