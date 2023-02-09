using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sortingOrder")]
	public class ES3UserType_Canvas : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Canvas() : base(typeof(UnityEngine.Canvas)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Canvas)obj;
			
			writer.WriteProperty("sortingOrder", instance.sortingOrder, ES3Type_int.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Canvas)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "sortingOrder":
						instance.sortingOrder = reader.Read<System.Int32>(ES3Type_int.Instance);
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