using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("lastOpenedPage")]
	public class ES3UserType_ImportantNewspaperPanelViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ImportantNewspaperPanelViewController() : base(typeof(ImportantNewspaperPanelViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ImportantNewspaperPanelViewController)obj;
			
			writer.WritePrivateField("lastOpenedPage", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ImportantNewspaperPanelViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "lastOpenedPage":
					instance = (ImportantNewspaperPanelViewController)reader.SetPrivateField("lastOpenedPage", reader.Read<System.Collections.Generic.Dictionary<System.Int32, System.Int32>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_ImportantNewspaperPanelViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ImportantNewspaperPanelViewControllerArray() : base(typeof(ImportantNewspaperPanelViewController[]), ES3UserType_ImportantNewspaperPanelViewController.Instance)
		{
			Instance = this;
		}
	}
}