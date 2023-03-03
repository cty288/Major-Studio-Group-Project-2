using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("markerPrefab", "markerPositions", "enabled", "name")]
	public class ES3UserType_NewspaperMarker : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NewspaperMarker() : base(typeof(ScreenSpaceCameraMarker)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ScreenSpaceCameraMarker)obj;
			
			writer.WritePrivateFieldByRef("markerPrefab", instance);
			writer.WritePrivateField("markerPositions", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ScreenSpaceCameraMarker)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "markerPrefab":
					instance = (ScreenSpaceCameraMarker)reader.SetPrivateField("markerPrefab", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "markerPositions":
					instance = (ScreenSpaceCameraMarker)reader.SetPrivateField("markerPositions", reader.Read<System.Collections.Generic.List<System.Collections.Generic.List<UnityEngine.Vector3>>>(), instance);
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


	public class ES3UserType_NewspaperMarkerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NewspaperMarkerArray() : base(typeof(ScreenSpaceCameraMarker[]), ES3UserType_NewspaperMarker.Instance)
		{
			Instance = this;
		}
	}
}