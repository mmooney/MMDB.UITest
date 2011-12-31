using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public static class DotNetParserHelper
	{
		public static string BuildNamespace(MemberType memberTypeNode)
		{
			string returnValue = string.Empty;
			var child = (MemberType)memberTypeNode.Children.SingleOrDefault(i => i is MemberType);
			returnValue = child.ToString();
			if(returnValue.IndexOf("::") > 0)
			{
				returnValue = returnValue.Substring(returnValue.IndexOf("::")+2);
			}
			return returnValue;
			//if (child != null && !child.IsDoubleColon)
			//{
			//    returnValue = BuildNamespace(child);
			//}
			//if (!string.IsNullOrEmpty(returnValue))
			//{
			//    returnValue += ".";
			//}
			//if(child != null)
			//{
			//    returnValue += child.MemberName;
			//}
			//return returnValue;
		}
	}
}
