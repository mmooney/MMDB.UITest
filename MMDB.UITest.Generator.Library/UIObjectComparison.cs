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

		public static UIObjectComparison Compare(SourceWebPage webPage, TargetClass targetClass)
		{
			UIObjectComparison comparison = new UIObjectComparison();
			foreach(var control in webPage.Controls)
			{
				var targetField = targetClass.TargetFieldList.FirstOrDefault(i=>i.FieldName == control.FieldName
																		&& i.TypeFullName == control.ClassFullName);
				if(targetField == null)
				{
					targetField = new TargetField()
					{
						FieldName = control.FieldName,
						TypeFullName = control.ClassFullName
					};
					comparison.FieldsToAdd.Add(targetField);
				}
			}
			return comparison;
		}
	}
}
