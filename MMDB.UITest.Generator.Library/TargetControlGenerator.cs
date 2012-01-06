using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public static class TargetControlGenerator
	{
		public static void AddLiteralControl(TypeDeclaration classObject, TargetField field)
		{
			//throw new NotImplementedException();
		}

		public static void AddHyperLinkControl(TypeDeclaration classObject, TargetField field, string fullTypeName)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("[{0}(\"{1}\",\"{2}\")] ", typeof(UIClientPropertyAttribute).FullName, field.FieldName, fullTypeName);
			sb.AppendFormat("public Link {0}", field.FieldName);
			sb.AppendFormat("{{ get {{ return this.TryGetLink(\"ctrl00_{0}\"); }} }}", field.FieldName);
			using(StringReader reader = new StringReader(sb.ToString()))
			{
				CSharpParser parser = new CSharpParser();
				var memberList = parser.ParseTypeMembers(reader);
				foreach(var member in memberList)
				{
					var property = (PropertyDeclaration)member;
					var role = new ICSharpCode.NRefactory.Role<ICSharpCode.NRefactory.CSharp.AttributedNode>("Member");
					property.Remove();
					classObject.AddChild(property, TypeDeclaration.MemberRole);
				}
			}
		}
	}
}
