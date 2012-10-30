using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MMDB.UITest.DotNetParser;
using MMDB.Shared;
using ICSharpCode.NRefactory.CSharp;
using MMDB.UITest.Core;

namespace MMDB.UITest.Generator.Library
{
	public class TargetModelWriter
	{
		public void ApplyChanges(TargetProjectComparisonResult comparison, string outputProjectPath)
		{
			foreach(var newClass in comparison.ClassesToAdd)
			{
				string userFilePath = Path.Combine(Path.GetDirectoryName(outputProjectPath), newClass.UserFileRelativePath);
				if (!File.Exists(userFilePath))
				{
					this.CreateUserFile(newClass, userFilePath);
				}
				string designerFilePath = Path.Combine(Path.GetDirectoryName(outputProjectPath), newClass.DesignerFileRelativePath);
				if (!File.Exists(designerFilePath))
				{
					switch (newClass.SourceObjectType)
					{
						case EnumSourceObjectType.MasterPage:
							this.CreateDesignerMasterPageFile(newClass, designerFilePath);
							break;
						case EnumSourceObjectType.WebPage:
							this.CreateDesignerWebPageFile(newClass, designerFilePath);
							break;
						default:
							throw new UnknownEnumValueException(newClass.SourceObjectType);
					}
				}
				ProjectParser parser = new ProjectParser();
				parser.EnsureFileInclude(outputProjectPath, newClass.UserFileRelativePath, null);
				parser.EnsureFileInclude(outputProjectPath, newClass.DesignerFileRelativePath, newClass.UserFileRelativePath);
			}
		}

		private void CreateDesignerWebPageFile(TargetClassComparisonResult targetClass, string designerFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(targetClass.TargetClassFullName, out typeName, out typeNamespace);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using MMDB.UITest.Core;");
			sb.AppendLine("using WatiN.Core;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", typeNamespace));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("[{0}(\"{1}\")]", typeof(UIClientPageAttribute).FullName, targetClass.SourceClassFullName));
			sb.AppendLine(string.Format("partial class {0} : {1}", typeName, typeof(BasePageClient).FullName));
			sb.AppendLine("{");
			sb.AppendLine();
			sb.AppendLine(string.Format("public {0} (Browser browser) : base(browser) {{}}", typeName));
			sb.AppendLine();
			sb.AppendLine(string.Format("protected override string ExpectedUrl {{ get {{ return \"{0}\"; }} }}", targetClass.ExpectedUrl));

			sb.AppendLine("}");
			sb.AppendLine("}");
			CSharpParser parser = new CSharpParser();
			var compilationUnit = parser.Parse(sb.ToString(), Path.GetFileName(designerFilePath));
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

		private void CreateDesignerMasterPageFile(TargetClassComparisonResult targetClass, string designerFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(targetClass.TargetClassFullName, out typeName, out typeNamespace);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using MMDB.UITest.Core;");
			sb.AppendLine("using WatiN.Core;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", typeNamespace));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("[{0}(\"{1}\")]", typeof(UIClientPageAttribute).FullName, targetClass.SourceClassFullName));
			sb.AppendLine(string.Format("partial class {0} : {1}", typeName, typeof(BaseMasterPageClient).FullName));
			sb.AppendLine("{");
			sb.AppendLine();
			sb.AppendLine(string.Format("public {0} (Browser browser) : base(browser) {{}}", typeName));
			sb.AppendLine();
			sb.AppendLine("}");
			sb.AppendLine("}");
			CSharpParser parser = new CSharpParser();
			var compilationUnit = parser.Parse(sb.ToString(), Path.GetFileName(designerFilePath));
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

		private void CreateUserFile(TargetClassComparisonResult targetClass, string userFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(targetClass.TargetClassFullName, out typeName, out typeNamespace);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using MMDB.UITest.Core;");
			sb.AppendLine("using WatiN.Core;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", typeNamespace));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("\tpublic partial class {0}", typeName));
			sb.AppendLine("\t{");
			sb.AppendLine();
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			CSharpParser parser = new CSharpParser();
			var compilationUnit = parser.Parse(sb.ToString(), Path.GetFileName(userFilePath));
			if (!Directory.Exists(Path.GetDirectoryName(userFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(userFilePath));
			}
			using (StreamWriter writer = new StreamWriter(userFilePath))
			{
				CSharpOutputVisitor outputVistor = new CSharpOutputVisitor(writer, new CSharpFormattingOptions());
				compilationUnit.AcceptVisitor(outputVistor, null);
			}
		}
	}
}
