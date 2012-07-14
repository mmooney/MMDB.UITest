using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using System.IO;
using System.Text.RegularExpressions;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebPage : SourceContainer
	{
		public override EnumSourceObjectType SourceObjectType
		{
			get { return EnumSourceObjectType.WebPage; }
		}
		public string PageUrl { get; set; }

		public static SourceWebPage TryLoad(string sourceProjectPath, CSClass csClass)
		{
			throw new NotImplementedException();
			//SourceWebPage returnValue = null;
			//string aspxFile = csClass.DependentUponFilePathList.SingleOrDefault(i => i.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase));
			//if (aspxFile != null)
			//{
			//    string aspxData;
			//    using (StreamReader reader = new StreamReader(Path.Combine(Path.GetDirectoryName(sourceProjectPath), aspxFile)))
			//    {
			//        aspxData = reader.ReadToEnd();
			//        ICSharpCode.NRefactory.CSharp.CSharpParser parser = new ICSharpCode.NRefactory.CSharp.CSharpParser();
			//        var unit = parser.Parse(reader, aspxFile);
			//    }
			//    Regex re = new Regex("<%@( )*Page .+ MasterPageFile=\\\"~/([\\w\\d/.]+)\\\"");
			//    var match = re.Match(aspxData);
			//    if (match.Groups.Count > 0)
			//    {
			//        returnValue = new SourceMasterContentPage()
			//        {
			//            ClassFullName = csClass.ClassFullName,
			//            MasterPageUrl = match.Groups[match.Groups.Count-1].Value
			//        };
			//    }
			//    else
			//    {
			//        returnValue = new SourceWebPage
			//        {
			//            ClassFullName = csClass.ClassFullName
			//        };
			//    }
			//}
			//return returnValue;
		}
	}
}
