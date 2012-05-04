using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Generator.Library;

namespace MMDB.UITest.Generator.Tests
{
	public class TargetClassManagerTests
	{
		public class TryLoadTargetClass
		{
			[Test]
			public void BasicTest()
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
								new CSAttribute.CSAttributeArgument { ArgumentName = "SourceClassFullName", ArguementValue = "Test.Test1.SourceClassFullNameValue" }
							}
						}
					}
				};
				var targetClassManager = new TargetClassManager();
				var targetClass = targetClassManager.TryLoadTargetClass(csClass);
				Assert.IsNotNull(targetClass);
				Assert.AreEqual("Test.Test1.SourceClassFullNameValue", targetClass.SourceClassFullName);
				Assert.AreEqual("Test.Target.TargetClassName", targetClass.TargetClassFullName);
				Assert.AreNotEqual(0,targetClass.TargetFieldList.Count);
				Assert.AreNotEqual(EnumTargetObjectType.Unknown, targetClass.TargetObjectType);
				Assert.IsNotNullOrEmpty(targetClass.PageUrl);
			}
		}
	}
}
