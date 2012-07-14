using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class TargetProjectComparisonResult
	{
		public List<TargetClassComparisonResult> ClassesToAdd { get; set; }
		public List<TargetClassComparisonResult> ClassesToUpdate { get; set; }

		public TargetProjectComparisonResult()
		{
			this.ClassesToAdd = new List<TargetClassComparisonResult>();
			this.ClassesToUpdate = new List<TargetClassComparisonResult>();
		}
	}
}
