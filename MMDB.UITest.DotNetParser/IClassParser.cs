using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.DotNetParser
{
	public interface IClassParser
	{
		List<CSClass> ParseFile(string filePath, IEnumerable<CSClass> existingClassList = null, IEnumerable<string> dependententUponFileList = null);

		List<CSClass> ParseString(string data, string filePath, IEnumerable<CSClass> existingClassList = null, IEnumerable<string> dependententUponFileList = null);
	}
}
