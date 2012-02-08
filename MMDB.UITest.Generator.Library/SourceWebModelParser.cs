using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using System.IO;
using System.Text.RegularExpressions;

namespace MMDB.UITest.Generator.Library
{
	public class SourceWebModelParser
	{
		public SourceWebPage TryLoad(string projectPath, CSClass classObject)
		{
			throw new NotImplementedException();
			//SourceWebPage returnValue = null;
			//string aspxFile = classObject.DependentUponFilePathList.SingleOrDefault(i => i.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase));
			//if (aspxFile != null)
			//{
			//    string aspxData;
			//    using (StreamReader reader = new StreamReader(Path.Combine(Path.GetDirectoryName(projectPath), aspxFile)))
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
			//            ClassFullName = classObject.ClassFullName,
			//            MasterPageUrl = match.Groups[match.Groups.Count - 1].Value
			//        };
			//    }
			//    else
			//    {
			//        returnValue = new SourceWebPage
			//        {
			//            ClassFullName = classObject.ClassFullName
			//        };
			//    }
			//}
			//return returnValue;
		}
	}
}
