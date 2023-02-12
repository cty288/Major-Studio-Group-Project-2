using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("newsID", "sprites", "tableBounds", "CurrentDroppingItem", "Container", "canDrag", "enabled", "name")]
	public class ES3UserType_NewspaperViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NewspaperViewController() : base(typeof(NewspaperViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NewspaperViewController)obj;
			
			writer.WritePrivateField("newsID", instance);
			writer.WritePrivateField("sprites", instance);
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePropertyByRef("CurrentDroppingItem", NewspaperViewController.CurrentDroppingItem);
			writer.WritePropertyByRef("Container", instance.Container);
			writer.WritePrivateField("canDrag", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NewspaperViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "newsID":
					instance = (NewspaperViewController)reader.SetPrivateField("newsID", reader.Read<System.String>(), instance);
					break;
					case "sprites":
					instance = (NewspaperViewController)reader.SetPrivateField("sprites", reader.Read<System.Collections.Generic.List<UnityEngine.Sprite>>(), instance);
					break;
					case "tableBounds":
					instance = (NewspaperViewController)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "CurrentDroppingItem":
						NewspaperViewController.CurrentDroppingItem = reader.Read<DraggableItems>();
						break;
					case "Container":
						instance.Container = reader.Read<AbstractDroppableItemContainerViewController>();
						break;
					case "canDrag":
					instance = (NewspaperViewController)reader.SetPrivateField("canDrag", reader.Read<System.Boolean>(), instance);
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


	public class ES3UserType_NewspaperViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NewspaperViewControllerArray() : base(typeof(NewspaperViewController[]), ES3UserType_NewspaperViewController.Instance)
		{
			Instance = this;
		}
	}
}