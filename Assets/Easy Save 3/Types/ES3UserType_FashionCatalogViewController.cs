using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("date", "week", "spriteIndex", "tableBounds", "canDrag")]
	public class ES3UserType_FashionCatalogViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_FashionCatalogViewController() : base(typeof(FashionCatalogViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (FashionCatalogViewController)obj;
			
			writer.WritePrivateField("date", instance);
			writer.WritePrivateField("week", instance);
			writer.WritePrivateField("spriteIndex", instance);
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePrivateField("canDrag", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (FashionCatalogViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "date":
					instance = (FashionCatalogViewController)reader.SetPrivateField("date", reader.Read<System.DateTime>(), instance);
					break;
					case "week":
					instance = (FashionCatalogViewController)reader.SetPrivateField("week", reader.Read<System.Int32>(), instance);
					break;
					case "spriteIndex":
					instance = (FashionCatalogViewController)reader.SetPrivateField("spriteIndex", reader.Read<System.Int32>(), instance);
					break;
					case "tableBounds":
					instance = (FashionCatalogViewController)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "canDrag":
					instance = (FashionCatalogViewController)reader.SetPrivateField("canDrag", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_FashionCatalogViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_FashionCatalogViewControllerArray() : base(typeof(FashionCatalogViewController[]), ES3UserType_FashionCatalogViewController.Instance)
		{
			Instance = this;
		}
	}
}