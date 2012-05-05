using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSProperty
	{
		public EnumProtectionLevel ProtectionLevel { get; set; }
		public string PropertyName { get; set; }
		public string TypeName { get; set; }
		public string TypeNamespace { get; set; }
		public string TypeFullName 
		{ 
			get
			{
				return DotNetParserHelper.BuildFullName(this.TypeNamespace, this.TypeName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.TypeName = className;
				this.TypeNamespace = namespaceName;
			}
		}

		public List<CSAttribute> AttributeList { get; set; }

		public CSProperty()
		{
			this.AttributeList = new List<CSAttribute>();
		}

		public static CSProperty Parse(PropertyDeclaration propertyNode)
		{
			CSProperty returnValue = new CSProperty();
			returnValue.ProtectionLevel = EnumProtectionLevel.Private;
			if ((propertyNode.Modifiers & Modifiers.Public) == Modifiers.Public)
			{
				returnValue.ProtectionLevel = EnumProtectionLevel.Public;
			}
			else if ((propertyNode.Modifiers & Modifiers.Private) == Modifiers.Private)
			{
				returnValue.ProtectionLevel = EnumProtectionLevel.Private;
			}
			else if ((propertyNode.Modifiers & Modifiers.Protected) == Modifiers.Protected)
			{
				returnValue.ProtectionLevel = EnumProtectionLevel.Protected;
			}
			else if ((propertyNode.Modifiers & Modifiers.Internal) == Modifiers.Internal)
			{
				returnValue.ProtectionLevel = EnumProtectionLevel.Internal;
			}
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(propertyNode.ReturnType.ToString(), out typeName, out typeNamespace);
			returnValue.TypeName = typeName;
			returnValue.TypeNamespace = typeNamespace;
			returnValue.PropertyName = propertyNode.Name;
			foreach (var attributeSectionNode in propertyNode.Attributes)
			{
				foreach (var attributeNode in attributeSectionNode.Attributes)
				{
					var attribute = CSAttribute.Parse(attributeNode);
					returnValue.AttributeList.Add(attribute);
				}
			}
			return returnValue;
		}
	}
}
