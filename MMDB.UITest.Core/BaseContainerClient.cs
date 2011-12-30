using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MMDB.UITest.Core
{
	public class BaseContainerClient : IDisposable
	{
		public Browser Browser { get; private set; }

		public BaseContainerClient(Browser browser)
		{
			this.Browser = browser;
		}

		public TextField GetTextField(string controlID)
		{
			TextField returnValue = TryGetTextField(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("TextField \"{0}\" not found", controlID));
			}
			return returnValue;
		}

		public TextField TryGetTextField(string controlID)
		{
			controlID = this.FormatControlID(controlID);
			TextField returnValue = this.Browser.TextFields.Where(i => i.Id == controlID).SingleOrDefault();
			if (returnValue == null)
			{
				returnValue = this.Browser.TextFields.Where(i => i.Name == controlID).SingleOrDefault();
			}
			return returnValue;
		}

		public Div GetDiv(string controlID)
		{
			return this.GetElement<Div>(controlID);
		}

		public Div TryGetDiv(string controlID)
		{
			return this.TryGetElement<Div>(controlID);
		}

		public Span TryGetSpan(string controlID)
		{
			return this.TryGetElement<Span>(controlID);
		}

		public Span GetSpan(string controlID)
		{
			Span returnValue = TryGetElement<Span>(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("Span \"{0}\" not found ({1})", controlID, this.FormatControlID("")));
			}
			return returnValue;
		}

		public FileUpload GetFileUpload(string controlID)
		{
			FileUpload returnValue = TryGetElement<FileUpload>(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("FileUpload \"{0}\" not found ({1})", controlID, this.FormatControlID("")));
			}
			return returnValue;
		}

		public T GetElement<T>(string controlID) where T : Element
		{
			T returnValue = TryGetElement<T>(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("{0} \"{1}\" not found", typeof(T).Name, controlID));
			}
			return returnValue;
		}

		public T TryGetElement<T>(string controlID) where T : Element
		{
			controlID = this.FormatControlID(controlID);
			T returnValue = this.Browser.ElementsOfType<T>().Where(i => i.Id == controlID).SingleOrDefault();
			if (returnValue == null)
			{
				returnValue = this.Browser.ElementsOfType<T>().Where(i => i.Name == controlID).SingleOrDefault();
			}
			return returnValue;
		}

		public CheckBox GetCheckBox(string controlID)
		{
			CheckBox returnValue = TryGetCheckBox(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("CheckBox \"{0}\" not found", controlID));
			}
			return returnValue;
		}

		public CheckBox TryGetCheckBox(string controlID)
		{
			controlID = this.FormatControlID(controlID);
			CheckBox returnValue = this.Browser.CheckBoxes.Where(i => i.Id == controlID).SingleOrDefault();
			if (returnValue == null)
			{
				returnValue = this.Browser.CheckBoxes.Where(i => i.Name == controlID).SingleOrDefault();
			}
			return returnValue;
		}

		public Button GetButton(string controlID)
		{
			Button returnValue = TryGetButton(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("Button \"{0}\" not found", controlID));
			}
			return returnValue;
		}

		public Button TryGetButton(string controlID)
		{
			controlID = this.FormatControlID(controlID);
			Button returnValue = this.Browser.Buttons.Where(i => i.Id == controlID).SingleOrDefault();
			if (returnValue == null)
			{
				returnValue = this.Browser.Buttons.Where(i => i.Name == controlID).SingleOrDefault();
			}
			return returnValue;
		}

		public Link GetLink(string controlID)
		{
			Link returnValue = TryGetLink(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("Link \"{0}\" not found", controlID));
			}
			return returnValue;
		}

		public Link GetLinkByUrl(string url)
		{
			Link returnValue = TryGetLinkByUrl(url);
			if (returnValue == null)
			{
				throw new Exception(string.Format("Link with URL \"{0}\" not found", url));
			}
			return returnValue;
		}

		public Link TryGetLinkByUrl(string url)
		{
			return this.Browser.Links.Where(i => !string.IsNullOrEmpty(i.Url) && i.Url.ToLower() == url.ToLower()).SingleOrDefault();
		}

		public Link GetLinkByText(string text)
		{
			Link returnValue = TryGetLinkByText(text);
			if (returnValue == null)
			{
				throw new Exception(string.Format("Link with text \"{0}\" not found", text));
			}
			return returnValue;
		}

		public Link TryGetLinkByText(string text)
		{
			return this.TryGetElementByText<Link>(text);
		}

		public T TryGetElementByText<T>(string text) where T : Element
		{
			return this.Browser.ElementsOfType<T>().Where(i => i.Text == text).SingleOrDefault();
		}

		public Link TryGetLink(string controlID)
		{
			controlID = this.FormatControlID(controlID);
			Link returnValue = this.Browser.Links.Where(i => i.Id == controlID).SingleOrDefault();
			if (returnValue == null)
			{
				returnValue = this.Browser.Links.Where(i => i.Name == controlID).SingleOrDefault();
			}
			return returnValue;
		}

		public SelectList TryGetSelectList(string controlID)
		{
			return TryGetElement<SelectList>(controlID);
		}

		public SelectList GetSelectList(string controlID)
		{
			SelectList returnValue = TryGetElement<SelectList>(controlID);
			if (returnValue == null)
			{
				throw new Exception(string.Format("SelectList \"{0}\" not found", controlID));
			}
			return returnValue;
		}

		public virtual string FormatControlID(string controlID)
		{
			return controlID;
		}

		public RadioButton GetRadioButton(string controlID)
		{
			return this.GetElement<RadioButton>(controlID);
		}

		public Image GetImage(string controlID)
		{
			return this.GetElement<Image>(controlID);
		}

		public Table GetTable(string controlID)
		{
			return this.GetElement<Table>(controlID);
		}

		#region IDisposable Members

		public void Dispose()
		{
			//do nothing, just want the using pattern
		}

		#endregion
	}
}
