using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("overrideSorting", "targetDisplay", "sortingLayerID", "sortingLayerName")]
	public class ES3UserType_Canvas : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Canvas() : base(typeof(UnityEngine.Canvas)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Canvas)obj;
			
			writer.WriteProperty("overrideSorting", instance.overrideSorting, ES3Type_bool.Instance);
			writer.WriteProperty("targetDisplay", instance.targetDisplay, ES3Type_int.Instance);
			writer.WriteProperty("sortingLayerID", instance.sortingLayerID, ES3Type_int.Instance);
			writer.WriteProperty("sortingLayerName", instance.sortingLayerName, ES3Type_string.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Canvas)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "overrideSorting":
						instance.overrideSorting = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "targetDisplay":
						instance.targetDisplay = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingLayerID":
						instance.sortingLayerID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingLayerName":
						instance.sortingLayerName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CanvasArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CanvasArray() : base(typeof(UnityEngine.Canvas[]), ES3UserType_Canvas.Instance)
		{
			Instance = this;
		}
	}
}