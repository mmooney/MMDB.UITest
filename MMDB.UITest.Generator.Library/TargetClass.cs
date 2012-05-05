using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;
using MMDB.UITest.Core;
using System.IO;
using ICSharpCode.NRefactory.CSharp;
using MMDB.Shared;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClass
	{
		public EnumTargetObjectType TargetObjectType { get; set; }
		public string SourceClassFullName { get; set; }
		public string TargetClassFullName { get; set; }
		public List<TargetField> TargetFieldList { get; set; }
		public string DesignerFilePath { get; set; }
		public string UserFilePath { get; set; }
		public string ExpectedUrl { get; set; }

		public TargetClass()
		{
			this.TargetFieldList = new List<TargetField>();
		}


		public static bool IsTargetClass(CSClass csClass)
		{
			return csClass.AttributeList.Any(i=>i.TypeName == typeof(UIClientPageAttribute).Name && i.TypeNamespace  ==  typeof(UIClientPageAttribute).Namespace);
		}

		internal static TargetClass TryLoad(CSClass csClass)
		{
			TargetClass returnValue = null;
			var uiClientPageAttribute = csClass.AttributeList.SingleOrDefault(i => i.TypeName == typeof(UIClientPageAttribute).Name && i.TypeNamespace == typeof(UIClientPageAttribute).Namespace);
			if(uiClientPageAttribute != null)
			{
				returnValue = new TargetClass
				{
					SourceClassFullName = Convert.ToString(uiClientPageAttribute.GetAttributeParameter(0, "SourceClassFullName", true)),
					TargetClassFullName = csClass.ClassFullName
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

				foreach(var csProperty in csClass.PropertyList)
				{
					var targetField = TargetField.TryLoad(csProperty);
					if(targetField != null)
					{
						returnValue.TargetFieldList.Add(targetField);
					}
				}
			}
			return returnValue;
		}

		public static TargetClass Create(TargetProject targetProject, SourceWebProject sourceProject, SourceWebPage sourcePage)
		{
			string sourceTypeName;
			string sourceTypeNamespace;
			DotNetParserHelper.SplitType(sourcePage.ClassFullName, out sourceTypeName, out sourceTypeNamespace);

			string relativeSourceNamespace;
			if(sourceTypeNamespace == sourceProject.RootNamespace)
			{
				relativeSourceNamespace = string.Empty;
			}
			else if(sourceTypeNamespace.StartsWith(sourceProject.RootNamespace))
			{
				relativeSourceNamespace = sourceTypeNamespace.Substring(sourceProject.RootNamespace.Length + 1);
			}
			else 
			{
				relativeSourceNamespace = sourceTypeNamespace;
			}
			string targetClassName = sourceTypeName + "PageClient";
			string targetNamespace = targetProject.RootNamespace + ".Client.Pages." + relativeSourceNamespace;
			TargetClass returnValue = new TargetClass()
			{
				SourceClassFullName = sourcePage.ClassFullName,
				TargetClassFullName = targetNamespace + "." + targetClassName,
				DesignerFilePath = Path.Combine(targetNamespace.Replace('.', '\\'), targetClassName + ".designer.cs"),
				UserFilePath = Path.Combine(targetNamespace.Replace('.', '\\'), targetClassName + ".cs"),
			};
			return returnValue;
		}

		public void AddFieldsToFile(string targetProjectPath, string relativeFilePath, List<TargetField> list)
		{
			CompilationUnit compilationUnit;
			bool anyChanges = false;
			string filePath = Path.Combine(Path.GetDirectoryName(targetProjectPath), relativeFilePath);
			using(StreamReader reader = new StreamReader(filePath))
			{
				CSharpParser parser = new CSharpParser();
				compilationUnit = parser.Parse(reader, Path.GetFileName(filePath));
			}
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(this.TargetClassFullName, out typeName, out typeNamespace);
			var typeDeclarationList = compilationUnit.Descendants.Where(i => i is TypeDeclaration);
			var classObject = (TypeDeclaration)compilationUnit.Descendants.Single(i=>i is TypeDeclaration && ((TypeDeclaration)i).Name == typeName);
			foreach(var field in list)
			{
				switch(field.SourceClassFullName)
				{
					//case "System.Web.UI.WebControls.Literal":
					//    TargetControlGenerator.AddLiteralControl(classObject, field);
					//    anyChanges = true;
					//    break;
					case "System.Web.UI.WebControls.HyperLink":
						TargetControlGenerator.AddHyperLinkControl(classObject, field, field.SourceClassFullName);
						anyChanges = true;
						break;
				}
			}
			if(anyChanges)
			{
				using(StreamWriter writer = new StreamWriter(filePath))
				{
					CSharpOutputVisitor visitor = new CSharpOutputVisitor(writer, new CSharpFormattingOptions());
					compilationUnit.AcceptVisitor(visitor);
				}
			}
		}

		private void CreateUserFile(string userFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(this.TargetClassFullName, out typeName, out typeNamespace);
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
			string userFilePath = Path.Combine(Path.GetDirectoryName(targetProjectPath), this.UserFilePath);
			if (!File.Exists(userFilePath))
			{
				this.CreateUserFile(userFilePath);
			}

			string designerFilePath = Path.Combine(Path.GetDirectoryName(targetProjectPath), this.DesignerFilePath);
			if (!File.Exists(designerFilePath))
			{
				switch(this.TargetObjectType)
				{
					case EnumTargetObjectType.MasterPage:
						this.CreateDesignerMasterPageFile(designerFilePath);
						break;
					case EnumTargetObjectType.WebPage:
						this.CreateWebPageFile(designerFilePath);
						break;
					default:
						throw new UnknownEnumValueException(this.TargetObjectType);
				}
			}
			
			ProjectParser parser = new ProjectParser();
			parser.EnsureFileInclude(targetProjectPath, this.UserFilePath, null);
			parser.EnsureFileInclude(targetProjectPath, this.DesignerFilePath, this.UserFilePath);
		}

		private void CreateWebPageFile(string designerFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(this.TargetClassFullName, out typeName, out typeNamespace);
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
			sb.AppendLine(string.Format("[{0}(\"{1}\")]", typeof(UIClientPageAttribute).FullName, this.SourceClassFullName));
			sb.AppendLine(string.Format("partial class {0} : {1}", typeName, typeof(BasePageClient).FullName));
			sb.AppendLine("{");
			sb.AppendLine();
			sb.AppendLine(string.Format("public {0} (Browser browser) : base(browser) {{}}", typeName));
			sb.AppendLine();
			sb.AppendLine(string.Format("protected override string ExpectedUrl {{ get {{ return \"{0}\"; }} }}", this.ExpectedUrl));

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

		private void CreateDesignerMasterPageFile(string designerFilePath)
		{
			string typeName;
			string typeNamespace;
			DotNetParserHelper.SplitType(this.TargetClassFullName, out typeName, out typeNamespace);
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
				sb.AppendLine(string.Format("[{0}(\"{1}\")]", typeof(UIClientPageAttribute).FullName, this.SourceClassFullName));
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

    }
}
