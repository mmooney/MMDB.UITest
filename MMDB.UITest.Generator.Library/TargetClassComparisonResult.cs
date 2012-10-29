using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.UITest.DotNetParser;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClassComparisonResult
	{
		public string SourceClassName { get; set; }
		public string SourceClassNamespaceName { get; set; }
		public string SourceClassFullName 
		{ 
			get
			{
				return DotNetParserHelper.BuildFullName(this.SourceClassNamespaceName, this.SourceClassName);
			}
			set
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.SourceClassName = className;
				this.SourceClassNamespaceName = namespaceName;
			}
		}

		public string TargetClassName { get; set; }
		public string TargetNamespaceName { get; set; }
		public string TargetClassFullName 
		{ 
			get 
			{ 
				return DotNetParserHelper.BuildFullName(this.TargetNamespaceName, this.TargetClassName);
			}
			set 
			{
				string className;
				string namespaceName;
				DotNetParserHelper.SplitType(value, out className, out namespaceName);
				this.TargetClassName = className;
				this.TargetNamespaceName = namespaceName;
			}
		}
		public string UserFileRelativePath { get; set; }
		public string DesignerFileRelativePath { get; set; }
		public EnumSourceObjectType SourceObjectType { get; set; }
		public List<TargetField> FieldsToAdd { get; set; }
		public List<TargetField> FieldsToUpdate { get; set; }
		public List<TargetField> FieldsToDelete { get; set; }
		public string ExpectedUrl { get; set; }
		public bool IsDirty { get; set; }

		public TargetClassComparisonResult()
		{
			this.FieldsToAdd = new List<TargetField>();
			this.FieldsToUpdate = new List<TargetField>();
			this.FieldsToDelete = new List<TargetField>();
			this.IsDirty = false;
		}

		public static TargetClassComparisonResult Compare(SourceWebPage webPage, TargetClass targetClass)
		{
			TargetClassComparisonResult comparison = new TargetClassComparisonResult();
			foreach(var control in webPage.Controls)
			{
				var targetField = targetClass.TargetFieldList.FirstOrDefault(i=>i.SourceFieldName == control.FieldName
																		&& i.SourceClassFullName == control.ClassFullName);
				if(targetField == null)
				{
					targetField = new TargetField()
					{
						SourceFieldName = control.FieldName,
						SourceClassFullName = control.ClassFullName
					};
					comparison.FieldsToAdd.Add(targetField);
				}
			}
			return comparison;
		}
	}
}
