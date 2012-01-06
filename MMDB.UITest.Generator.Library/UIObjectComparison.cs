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
				var targetField = targetClass.TargetFieldList.FirstOrDefault(i=>i.FieldName == control.FieldName
																		&& i.TypeFullName == control.TypeFullName);
				if(targetField == null)
				{
					targetField = new TargetField()
					{
						FieldName = control.FieldName,
						TypeFullName = control.TypeFullName
					};
					comparison.FieldsToAdd.Add(targetField);
				}
			}
			return comparison;
		}
	}
}
