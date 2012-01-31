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
		public string TypeFullName 
		{ 
			get 
			{
				return DotNetParserHelper.BuildFullName(this.TypeNamespace, this.TypeName);
			}
		}

		public static List<CSField> Parse(FieldDeclaration fieldNode)
		{
			var returnList = new List<CSField>();
			EnumProtectionLevel protectionLevel = EnumProtectionLevel.None;
			if ((fieldNode.Modifiers & Modifiers.Public) == Modifiers.Public)
			{
				protectionLevel = EnumProtectionLevel.Public;
			}
			else if ((fieldNode.Modifiers & Modifiers.Private) == Modifiers.Private)
			{
				protectionLevel = EnumProtectionLevel.Private;
			}
			else if ((fieldNode.Modifiers & Modifiers.Protected) == Modifiers.Protected)
			{
				protectionLevel = EnumProtectionLevel.Protected;
			}
			else if ((fieldNode.Modifiers & Modifiers.Internal) == Modifiers.Internal)
			{
				protectionLevel = EnumProtectionLevel.Internal;
			}
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(fieldNode.ReturnType.ToString(), out typeName, out typeNamespace);
			foreach(var variableNode in fieldNode.Variables)
			{
				CSField fieldObject = new CSField
				{
					TypeNamespace = typeNamespace,
					TypeName = typeName,
					FieldName = variableNode.Name,
					ProtectionLevel = protectionLevel
				};
				returnList.Add(fieldObject);
			}
			return returnList;
		}
	}
}
