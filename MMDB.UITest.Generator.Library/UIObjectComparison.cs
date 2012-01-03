using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class UIObjectComparison
	{
		public List<TargetField> FieldsToAdd { get; set; }
		public List<TargetField> FieldsToDelete { get; set; }

		public UIObjectComparison()
		{
			this.FieldsToAdd = new List<TargetField>();
			this.FieldsToDelete = new List<TargetField>();
		}

		public static UIObjectComparison Compare(SourceMasterPage masterPage, TargetClass targetClass)
		{
			UIObjectComparison comparison = new UIObjectComparison();
			foreach(var control in masterPage.Controls)
			{
				var targetField = targetClass.TargetFieldList.SingleOrDefault(i=>i.FieldName == control.FieldName
																		&& i.TypeName == control.TypeName
																		&& i.TypeNamespace == control.TypeNamespace);
				if(targetField == null)
				{
					targetField = new TargetField()
					{
						FieldName = control.FieldName,
						TypeName = control.TypeName,
						TypeNamespace = control.TypeNamespace
					};
					comparison.FieldsToAdd.Add(targetField);
				}
			}
			return comparison;
		}
	}
}
