using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("indicateCanvas", "dateCanvas", "renderers", "news", "newspaperSystem", "selfRenderer", "sprites", "dragStartPos", "tableBounds", "pointerDownTime", "CurrentDroppingItem", "Container", "canDrag", "enabled", "name")]
	public class ES3UserType_NewspaperViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NewspaperViewController() : base(typeof(NewspaperViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NewspaperViewController)obj;
			
			writer.WritePrivateFieldByRef("indicateCanvas", instance);
			writer.WritePrivateFieldByRef("dateCanvas", instance);
			writer.WritePrivateField("renderers", instance);
			writer.WritePrivateField("news", instance);
			writer.WritePrivateField("newspaperSystem", instance);
			writer.WritePrivateFieldByRef("selfRenderer", instance);
			writer.WritePrivateField("sprites", instance);
			writer.WritePrivateField("dragStartPos", instance);
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePrivateField("pointerDownTime", instance);
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
					
					case "indicateCanvas":
					instance = (NewspaperViewController)reader.SetPrivateField("indicateCanvas", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "dateCanvas":
					instance = (NewspaperViewController)reader.SetPrivateField("dateCanvas", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "renderers":
					instance = (NewspaperViewController)reader.SetPrivateField("renderers", reader.Read<System.Collections.Generic.List<UnityEngine.SpriteRenderer>>(), instance);
					break;
					case "news":
					instance = (NewspaperViewController)reader.SetPrivateField("news", reader.Read<Newspaper>(), instance);
					break;
					case "newspaperSystem":
					instance = (NewspaperViewController)reader.SetPrivateField("newspaperSystem", reader.Read<NewspaperSystem>(), instance);
					break;
					case "selfRenderer":
					instance = (NewspaperViewController)reader.SetPrivateField("selfRenderer", reader.Read<UnityEngine.SpriteRenderer>(), instance);
					break;
					case "sprites":
					instance = (NewspaperViewController)reader.SetPrivateField("sprites", reader.Read<System.Collections.Generic.List<UnityEngine.Sprite>>(), instance);
					break;
					case "dragStartPos":
					instance = (NewspaperViewController)reader.SetPrivateField("dragStartPos", reader.Read<UnityEngine.Vector2>(), instance);
					break;
					case "tableBounds":
					instance = (NewspaperViewController)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "pointerDownTime":
					instance = (NewspaperViewController)reader.SetPrivateField("pointerDownTime", reader.Read<System.DateTime>(), instance);
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