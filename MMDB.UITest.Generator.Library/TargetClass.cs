using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;
using System.IO;
using ICSharpCode.NRefactory.CSharp;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClass
	{
		public string SourceClassName { get; set; }
		public string SourceClassNamespace { get; set;}
		public string TargetClassName { get; set; }
		public string TargetClassNamespace { get; set; }
		public List<TargetField> TargetFieldList { get; set; }
		public string DesignerFilePath { get; set; }
		public string UserFilePath { get; set; }
		public string PageUrl { get; set; }

		public TargetClass()
		{
			this.TargetFieldList = new List<TargetField>();
		}


		public static bool IsTargetClass(CSClass csClass)
		{
			return csClass.AttributeList.Any(i=>i.TypeName == "UIClient" && i.TypeNamespace  ==  typeof(UIClientAttribute).Namespace);
		}

		internal static TargetClass TryLoad(CSClass csClass)
		{
			TargetClass returnValue = null;
			var uiClientAttribute = csClass.AttributeList.SingleOrDefault(i=>i.TypeName == "UIClient" && i.TypeNamespace  ==  typeof(UIClientAttribute).Namespace);
			if(uiClientAttribute != null)
			{
				returnValue = new TargetClass
				{
					SourceClassName = uiClientAttribute.GetAttributeParameter(0, "SourceClassName", true),
					SourceClassNamespace = uiClientAttribute.GetAttributeParameter(1, "SourceClassNamespace", true)
				};

				//If there is only one field, that is the user and designer file.
				//If there are two or more files and one ends with ".designer.cs", that is the designer file and the the first of the others is the user file
				//If there are two or more files and none ends with ".designer.cs", then the first one is the designer and user file
				if(csClass.FilePathList.Count == 1)
				{
					returnValue.DesignerFilePath = csClass.FilePathList[0];
					returnValue.UserFilePath = csClass.FilePathList[0];
				}
				else if (csClass.FilePathList.Count > 1)
				{
					returnValue.DesignerFilePath = csClass.FilePathList.FirstOrDefault(i=>i.EndsWith(".designer.cs", StringComparison.CurrentCultureIgnoreCase));
					if(string.IsNullOrEmpty(returnValue.DesignerFilePath))
					{
						returnValue.DesignerFilePath = csClass.FilePathList[0];
						returnValue.UserFilePath = csClass.FilePathList[0];
					}
					else 
					{
						returnValue.UserFilePath = csClass.FilePathList.FirstOrDefault(i=>i != returnValue.DesignerFilePath);
					}
				}
			}
			return returnValue;
		}

		public static TargetClass Create(TargetProject targetProject, SourceWebProject sourceProject, SourceWebPage sourcePage)
		{
			string relativeSourceNamespace;
			if(sourcePage.Namespace.StartsWith(sourceProject.RootNamespace))
			{
				relativeSourceNamespace = sourcePage.Namespace.Substring(sourceProject.RootNamespace.Length+1);
			}
			else 
			{
				relativeSourceNamespace = sourcePage.Namespace;
			}
			string targetClassName = sourcePage.ClassName + "PageClient";
			string targetNamespace = targetProject.RootNamespace + ".Client.Pages." + relativeSourceNamespace;
			TargetClass returnValue = new TargetClass()
			{
				SourceClassName = sourcePage.ClassName,
				SourceClassNamespace = sourcePage.Namespace,
				TargetClassName = targetClassName,
				TargetClassNamespace = targetNamespace,
				DesignerFilePath = Path.Combine(targetProject.Directory,targetNamespace.Replace('.', '\\'), targetClassName + ".designer.cs"),
				UserFilePath = Path.Combine(targetProject.Directory, targetNamespace.Replace('.', '\\'), targetClassName + ".cs"),
			};
			return returnValue;
		}

		public void AddFieldsToFile(List<TargetField> list)
		{
		}

		private void CreateUserFile(string userFilePath)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using MMDB.UITest.Core;");
			sb.AppendLine("using WatiN.Core;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", this.TargetClassNamespace));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("\tpublic partial class {0}", this.TargetClassName));
			sb.AppendLine("\t{");
			sb.AppendLine();
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			CSharpParser parser = new CSharpParser();
			var compilationUnit = parser.Parse(sb.ToString());
			if(!Directory.Exists(Path.GetDirectoryName(userFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(userFilePath));
			}
			using(StreamWriter writer = new StreamWriter(userFilePath))
			{
				CSharpOutputVisitor outputVistor = new CSharpOutputVisitor(writer, new CSharpFormattingOptions());
				compilationUnit.AcceptVisitor(outputVistor, null);
			}
		}

        public void EnsureFiles(string targetProjectPath)
		{
			if (!File.Exists(this.UserFilePath))
			{
				this.CreateUserFile(this.UserFilePath);
			}
			if(!File.Exists(this.DesignerFilePath))
			{
				this.CreateDesignerFile(this.DesignerFilePath);
			}
			CSProjectFile.EnsureFileInclude(targetProjectPath, this.UserFilePath, null);
			CSProjectFile.EnsureFileInclude(targetProjectPath, this.DesignerFilePath, this.UserFilePath);
		}

		private void CreateDesignerFile(string designerFilePath)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using MMDB.UITest.Core;");
			sb.AppendLine("using WatiN.Core;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", this.TargetClassNamespace));
			sb.AppendLine("{");
				sb.AppendLine(string.Format("partial class {0} : {1}", this.TargetClassName, typeof(BasePageClient).FullName));
				sb.AppendLine("{");
					sb.AppendLine();
					sb.AppendLine(string.Format("public {0} (Browser browser) : base(browser) {{}}", this.TargetClassName));
					sb.AppendLine();
					sb.AppendLine(string.Format("protected override string ExpectedUrl {{get {{ return \"{0}\"; }}}}", this.PageUrl));
					sb.AppendLine();
				sb.AppendLine("}");
			sb.AppendLine("}");
			CSharpParser parser = new CSharpParser();
			var compilationUnit = parser.Parse(sb.ToString());
			if (!Directory.Exists(Path.GetDirectoryName(designerFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(designerFilePath));
			}
			using (StreamWriter writer = new StreamWriter(designerFilePath))
			{
				CSharpOutputVisitor outputVistor = new CSharpOutputVisitor(writer, new CSharpFormattingOptions());
				compilationUnit.AcceptVisitor(outputVistor, null);
			}
		}

    }
}
