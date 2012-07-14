using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MMDB.UITest.Generator.Library;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.App
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//string inputProject = @"C:\Users\admin\Dropbox\Code\screwturn-screwturn-wiki-4-cf9155b27d4c\WebApplication\WebApplication.csproj";
			string inputProjectPath = @"..\..\..\..\BugNet\src\BugNET_WAP\BugNET_WAP.csproj";
			//string outputProjectPath = @"C:\Users\admin\Dropbox\Code\MMDB.UITest\ScrewturnWikiProxySample\ScrewturnWikiProxySample.csproj";
			string outputProjectPath = @"..\..\..\BugNetProxySample\BugNetProxySample.csproj";
			var sourceProjectFile = new ProjectParser().ParseFile(inputProjectPath);
			var sourceProject= new SourceWebModelParser().LoadFromProjectFile(sourceProjectFile, inputProjectPath);
			ProxyGenerator.UpdateProxyProject(outputProjectPath, sourceProject);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());

		}
	}
}
