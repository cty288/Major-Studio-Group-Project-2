using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("anchorMin", "anchorMax", "anchoredPosition", "sizeDelta", "pivot", "anchoredPosition3D", "offsetMin", "offsetMax", "drivenByObject", "drivenProperties", "position", "localPosition", "eulerAngles", "localEulerAngles", "right", "up", "forward", "rotation", "localRotation", "localScale", "parent", "parentInternal", "hasChanged", "hierarchyCapacity", "constrainProportionsScale", "name")]
	public class ES3UserType_RectTransform : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_RectTransform() : base(typeof(UnityEngine.RectTransform)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.RectTransform)obj;
			
			writer.WriteProperty("anchorMin", instance.anchorMin, ES3Type_Vector2.Instance);
			writer.WriteProperty("anchorMax", instance.anchorMax, ES3Type_Vector2.Instance);
			writer.WriteProperty("anchoredPosition", instance.anchoredPosition, ES3Type_Vector2.Instance);
			writer.WriteProperty("sizeDelta", instance.sizeDelta, ES3Type_Vector2.Instance);
			writer.WriteProperty("pivot", instance.pivot, ES3Type_Vector2.Instance);
			writer.WriteProperty("anchoredPosition3D", instance.anchoredPosition3D, ES3Type_Vector3.Instance);
			writer.WriteProperty("offsetMin", instance.offsetMin, ES3Type_Vector2.Instance);
			writer.WriteProperty("offsetMax", instance.offsetMax, ES3Type_Vector2.Instance);
			writer.WritePrivatePropertyByRef("drivenByObject", instance);
			writer.WritePrivateProperty("drivenProperties", instance);
			writer.WriteProperty("position", instance.position, ES3Type_Vector3.Instance);
			writer.WriteProperty("localPosition", instance.localPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("eulerAngles", instance.eulerAngles, ES3Type_Vector3.Instance);
			writer.WriteProperty("localEulerAngles", instance.localEulerAngles, ES3Type_Vector3.Instance);
			writer.WriteProperty("right", instance.right, ES3Type_Vector3.Instance);
			writer.WriteProperty("up", instance.up, ES3Type_Vector3.Instance);
			writer.WriteProperty("forward", instance.forward, ES3Type_Vector3.Instance);
			writer.WriteProperty("rotation", instance.rotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("localRotation", instance.localRotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("localScale", instance.localScale, ES3Type_Vector3.Instance);
			writer.WritePropertyByRef("parent", instance.parent);
			writer.WritePrivatePropertyByRef("parentInternal", instance);
			writer.WriteProperty("hasChanged", instance.hasChanged, ES3Type_bool.Instance);
			writer.WriteProperty("hierarchyCapacity", instance.hierarchyCapacity, ES3Type_int.Instance);
			writer.WritePrivateProperty("constrainProportionsScale", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.RectTransform)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "anchorMin":
						instance.anchorMin = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "anchorMax":
						instance.anchorMax = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "anchoredPosition":
						instance.anchoredPosition = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "sizeDelta":
						instance.sizeDelta = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "pivot":
						instance.pivot = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "anchoredPosition3D":
						instance.anchoredPosition3D = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "offsetMin":
						instance.offsetMin = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "offsetMax":
						instance.offsetMax = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "drivenByObject":
					instance = (UnityEngine.RectTransform)reader.SetPrivateProperty("drivenByObject", reader.Read<UnityEngine.Object>(), instance);
					break;
					case "drivenProperties":
					instance = (UnityEngine.RectTransform)reader.SetPrivateProperty("drivenProperties", reader.Read<UnityEngine.DrivenTransformProperties>(), instance);
					break;
					case "position":
						instance.position = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "localPosition":
						instance.localPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "eulerAngles":
						instance.eulerAngles = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "localEulerAngles":
						instance.localEulerAngles = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "right":
						instance.right = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "up":
						instance.up = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "forward":
						instance.forward = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "rotation":
						instance.rotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "localRotation":
						instance.localRotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "localScale":
						instance.localScale = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "parent":
						instance.parent = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "parentInternal":
					instance = (UnityEngine.RectTransform)reader.SetPrivateProperty("parentInternal", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "hasChanged":
						instance.hasChanged = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "hierarchyCapacity":
						instance.hierarchyCapacity = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "constrainProportionsScale":
					instance = (UnityEngine.RectTransform)reader.SetPrivateProperty("constrainProportionsScale", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_RectTransformArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_RectTransformArray() : base(typeof(UnityEngine.RectTransform[]), ES3UserType_RectTransform.Instance)
		{
			Instance = this;
		}
	}
}