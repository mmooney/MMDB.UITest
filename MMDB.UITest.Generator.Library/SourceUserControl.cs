﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.UITest.Generator.Library
{
	public class SourceUserControl : SourceContainer
	{
		public override EnumSourceObjectType SourceObjectType
		{
			get { return EnumSourceObjectType.UserControl; }
		}
	}
}
