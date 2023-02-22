using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("table", "originalLocalPosition", "tableBounds", "CurrentDroppingItem", "Container", "canDrag")]
	public class ES3UserType_VintageCameraViewController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_VintageCameraViewController() : base(typeof(VintageCameraViewController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (VintageCameraViewController)obj;
			
			writer.WritePrivateFieldByRef("table", instance);
			writer.WritePrivateField("originalLocalPosition", instance);
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePropertyByRef("CurrentDroppingItem", VintageCameraViewController.CurrentDroppingItem);
			writer.WritePropertyByRef("Container", instance.Container);
			writer.WritePrivateField("canDrag", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (VintageCameraViewController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "table":
					instance = (VintageCameraViewController)reader.SetPrivateField("table", reader.Read<AbstractDroppableItemContainerViewController>(), instance);
					break;
					case "originalLocalPosition":
					instance = (VintageCameraViewController)reader.SetPrivateField("originalLocalPosition", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "tableBounds":
					instance = (VintageCameraViewController)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "CurrentDroppingItem":
						VintageCameraViewController.CurrentDroppingItem = reader.Read<DraggableItems>();
						break;
					case "Container":
						instance.Container = reader.Read<AbstractDroppableItemContainerViewController>();
						break;
					case "canDrag":
					instance = (VintageCameraViewController)reader.SetPrivateField("canDrag", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_VintageCameraViewControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_VintageCameraViewControllerArray() : base(typeof(VintageCameraViewController[]), ES3UserType_VintageCameraViewController.Instance)
		{
			Instance = this;
		}
	}
}