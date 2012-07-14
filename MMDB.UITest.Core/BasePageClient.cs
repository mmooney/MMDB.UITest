using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MMDB.UITest.Core
{
	public abstract class BasePageClient : BaseContainerClient
	{
		public BasePageClient(Browser browser) : base(browser)
		{
			this.ValidateUrl();
		}

		public virtual void ValidateUrl()
		{
			if(this.ExpectedUrlList.Count() > 0)
			{
				string absolutePath = this.Browser.Uri.AbsolutePath;
				if(!this.ExpectedUrlList.Contains(absolutePath, StringComparer.CurrentCultureIgnoreCase))
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Incorrect URL");
					sb.Append("\tFound: " + absolutePath);
					if(this.ExpectedUrlList.Count() == 1)
					{
						sb.Append("\t:Expected: " + this.ExpectedUrlList.Single());
					}
					else 
					{
						sb.Append("\tExpected:");
						foreach(string expectedUrl in this.ExpectedUrlList)
						{
							sb.Append("\t-" + expectedUrl);
						}
					}
					throw new Exception(sb.ToString());
				}
			}
		}

		protected abstract IEnumerable<string> ExpectedUrlList { get; }
	}
}
