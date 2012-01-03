using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.DotNetParser
{
	public static class DotNetParserHelper
	{
		public static void SplitType(string rawTypeValue, out string typeName, out string typeNamespace)
		{
			string workingValue = rawTypeValue;
			if (workingValue.IndexOf("::") > 0)
			{
				workingValue = workingValue.Substring(workingValue.IndexOf("::") + 2);
			}
			if(workingValue.Contains('.'))
			{
				typeName = workingValue.Substring(workingValue.LastIndexOf('.')+1);
				typeNamespace = workingValue.Substring(0, workingValue.LastIndexOf('.'));
			}
			else
			{
				typeName = workingValue;
				typeNamespace = string.Empty;
			}
		}

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
