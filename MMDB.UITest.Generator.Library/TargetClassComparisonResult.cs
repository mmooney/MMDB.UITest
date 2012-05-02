using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class TargetClassComparisonResult
	{
		public string ClassFullName { get; set; }
		public string UserFileRelativePath { get; set; }
		public string DesignerFileRelativePath { get; set; }
		public List<TargetField> FieldsToAdd { get; set; }
		public List<TargetField> FieldsToDelete { get; set; }

		public TargetClassComparisonResult()
		{
			this.FieldsToAdd = new List<TargetField>();
			this.FieldsToDelete = new List<TargetField>();
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
