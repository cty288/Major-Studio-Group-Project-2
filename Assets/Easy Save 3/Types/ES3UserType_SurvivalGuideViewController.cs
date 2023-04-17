using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("tableBounds", "CurrentDroppingItem", "Container", "canDrag")]
	public class ES3UserType_SurvivalGuideViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SurvivalGuideViewController() : base(typeof(SurvivalGuideViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SurvivalGuideViewController)obj;
			
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePropertyByRef("CurrentDroppingItem", SurvivalGuideViewController.CurrentDroppingItem);
			writer.WritePropertyByRef("Container", instance.Container);
			writer.WritePrivateField("canDrag", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SurvivalGuideViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "tableBounds":
					instance = (SurvivalGuideViewController)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "CurrentDroppingItem":
						SurvivalGuideViewController.CurrentDroppingItem = reader.Read<DraggableItems>();
						break;
					case "Container":
						instance.Container = reader.Read<AbstractDroppableItemContainerViewController>();
						break;
					case "canDrag":
					instance = (SurvivalGuideViewController)reader.SetPrivateField("canDrag", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SurvivalGuideViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SurvivalGuideViewControllerArray() : base(typeof(SurvivalGuideViewController[]), ES3UserType_SurvivalGuideViewController.Instance)
		{
			Instance = this;
		}
	}
}