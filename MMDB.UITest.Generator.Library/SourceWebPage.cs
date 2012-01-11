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
		public string PageUrl { get; set; }

		public static SourceWebPage TryLoad(string sourceProjectPath, CSClass csClass)
		{
			SourceWebPage returnValue = null;
			string aspxFile = csClass.DependentUponFilePathList.SingleOrDefault(i => i.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase));
			if (aspxFile != null)
			{
				string aspxData;
				using (StreamReader reader = new StreamReader(Path.Combine(Path.GetDirectoryName(sourceProjectPath), aspxFile)))
				{
					aspxData = reader.ReadToEnd();
				}
				Regex re = new Regex("<%@[ ]*Page .+ MasterPageFile=\\\"~/([\\w\\d]+\\.[\\w\\d]+)\\\"");
				var match = re.Match(aspxData);
				if (match.Groups.Count > 1)
				{
					returnValue = new SourceMasterContentPage()
					{
						ClassFullName = csClass.ClassFullName,
						MasterPageUrl = match.Groups[match.Groups.Count - 1].Value
					};
				}
				else
				{
					returnValue = new SourceWebPage
					{
						ClassFullName = csClass.ClassFullName
					};
				}
			}
			return returnValue;
		}
	}
}
