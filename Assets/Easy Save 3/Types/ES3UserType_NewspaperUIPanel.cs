using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("newspaperMarkers", "enabled", "name")]
	public class ES3UserType_NewspaperUIPanel : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NewspaperUIPanel() : base(typeof(NewspaperUIPanel)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NewspaperUIPanel)obj;
			
			writer.WritePrivateField("newspaperMarkers", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NewspaperUIPanel)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "newspaperMarkers":
					instance = (NewspaperUIPanel)reader.SetPrivateField("newspaperMarkers", reader.Read<System.Collections.Generic.Dictionary<System.String, UnityEngine.GameObject>>(), instance);
					break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_NewspaperUIPanelArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NewspaperUIPanelArray() : base(typeof(NewspaperUIPanel[]), ES3UserType_NewspaperUIPanel.Instance)
		{
			Instance = this;
		}
	}
}