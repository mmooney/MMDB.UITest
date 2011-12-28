using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public class CSField
	{
		public EnumProtectionLevel ProtectionLevel { get; set; }
		public string FieldName { get; set; }
		public string TypeName { get; set; }
		public string TypeNamespace { get; set; }

		public static CSField Parse(FieldDeclaration fieldNode)
		{
			CSField fieldObject = new CSField();
			if ((fieldNode.Modifiers | Modifiers.Public) == Modifiers.Public)
			{
				fieldObject.ProtectionLevel = EnumProtectionLevel.Public;
			}
			else if ((fieldNode.Modifiers | Modifiers.Private) == Modifiers.Private)
			{
				fieldObject.ProtectionLevel = EnumProtectionLevel.Private;
			}
			else if ((fieldNode.Modifiers | Modifiers.Protected) == Modifiers.Protected)
			{
				fieldObject.ProtectionLevel = EnumProtectionLevel.Protected;
			}
			else if ((fieldNode.Modifiers | Modifiers.Internal) == Modifiers.Internal)
			{
				fieldObject.ProtectionLevel = EnumProtectionLevel.Internal;
			}
			var memberTypeNode = (MemberType)fieldNode.Children.Single(i => i is MemberType);
			fieldObject.TypeName = memberTypeNode.MemberName;
			fieldObject.TypeNamespace = BuildNamespace(memberTypeNode);
			var variableInitializer = (VariableInitializer)fieldNode.Children.Single(i=>i is VariableInitializer);
			fieldObject.FieldName = variableInitializer.Name;
			return fieldObject;
		}

		private static string BuildNamespace(MemberType memberTypeNode)
		{
			string returnValue = string.Empty;
			var child = (MemberType)memberTypeNode.Children.SingleOrDefault(i=>i is MemberType);
			if(child != null && !child.IsDoubleColon)
			{
				returnValue = BuildNamespace(child);
			}
			if(!string.IsNullOrEmpty(returnValue))
			{
				returnValue += ".";
			}
			returnValue += child.MemberName;
			return returnValue;
		}
	}
}
