using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("lastOpenedPage", "useCameraMask", "hideTime")]
	public class ES3UserType_SurvivalGuidePanelViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SurvivalGuidePanelViewController() : base(typeof(SurvivalGuidePanelViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SurvivalGuidePanelViewController)obj;
			
			writer.WritePrivateField("lastOpenedPage", instance);
			writer.WritePrivateFieldByRef("useCameraMask", instance);
			writer.WritePrivateField("hideTime", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SurvivalGuidePanelViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "lastOpenedPage":
					instance = (SurvivalGuidePanelViewController)reader.SetPrivateField("lastOpenedPage", reader.Read<System.Int32>(), instance);
					break;
					case "useCameraMask":
					instance = (SurvivalGuidePanelViewController)reader.SetPrivateField("useCameraMask", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "hideTime":
					instance = (SurvivalGuidePanelViewController)reader.SetPrivateField("hideTime", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SurvivalGuidePanelViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SurvivalGuidePanelViewControllerArray() : base(typeof(SurvivalGuidePanelViewController[]), ES3UserType_SurvivalGuidePanelViewController.Instance)
		{
			Instance = this;
		}
	}
}