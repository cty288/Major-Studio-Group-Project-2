using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sprite", "flipX", "enabled", "shadowCastingMode", "receiveShadows", "motionVectorGenerationMode", "lightProbeUsage", "reflectionProbeUsage", "sortingLayerName", "sortingLayerID", "sortingOrder", "lightProbeProxyVolumeOverride", "probeAnchor", "lightmapIndex", "realtimeLightmapIndex", "lightmapScaleOffset", "realtimeLightmapScaleOffset", "materials", "material", "sharedMaterial", "sharedMaterials")]
	public class ES3UserType_SpriteRenderer : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SpriteRenderer() : base(typeof(UnityEngine.SpriteRenderer)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.SpriteRenderer)obj;
			
			writer.WritePropertyByRef("sprite", instance.sprite);
			writer.WriteProperty("flipX", instance.flipX, ES3Type_bool.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("shadowCastingMode", instance.shadowCastingMode, ES3Type_enum.Instance);
			writer.WriteProperty("receiveShadows", instance.receiveShadows, ES3Type_bool.Instance);
			writer.WriteProperty("motionVectorGenerationMode", instance.motionVectorGenerationMode, ES3Type_enum.Instance);
			writer.WriteProperty("lightProbeUsage", instance.lightProbeUsage, ES3Type_enum.Instance);
			writer.WriteProperty("reflectionProbeUsage", instance.reflectionProbeUsage, ES3Type_enum.Instance);
			writer.WriteProperty("sortingLayerName", instance.sortingLayerName, ES3Type_string.Instance);
			writer.WriteProperty("sortingLayerID", instance.sortingLayerID, ES3Type_int.Instance);
			writer.WriteProperty("sortingOrder", instance.sortingOrder, ES3Type_int.Instance);
			writer.WritePropertyByRef("lightProbeProxyVolumeOverride", instance.lightProbeProxyVolumeOverride);
			writer.WritePropertyByRef("probeAnchor", instance.probeAnchor);
			writer.WriteProperty("lightmapIndex", instance.lightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("realtimeLightmapIndex", instance.realtimeLightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("lightmapScaleOffset", instance.lightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("realtimeLightmapScaleOffset", instance.realtimeLightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("materials", instance.materials, ES3Type_MaterialArray.Instance);
			writer.WritePropertyByRef("material", instance.material);
			writer.WritePropertyByRef("sharedMaterial", instance.sharedMaterial);
			writer.WriteProperty("sharedMaterials", instance.sharedMaterials, ES3Type_MaterialArray.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.SpriteRenderer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "sprite":
						instance.sprite = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "flipX":
						instance.flipX = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "shadowCastingMode":
						instance.shadowCastingMode = reader.Read<UnityEngine.Rendering.ShadowCastingMode>(ES3Type_enum.Instance);
						break;
					case "receiveShadows":
						instance.receiveShadows = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "motionVectorGenerationMode":
						instance.motionVectorGenerationMode = reader.Read<UnityEngine.MotionVectorGenerationMode>(ES3Type_enum.Instance);
						break;
					case "lightProbeUsage":
						instance.lightProbeUsage = reader.Read<UnityEngine.Rendering.LightProbeUsage>(ES3Type_enum.Instance);
						break;
					case "reflectionProbeUsage":
						instance.reflectionProbeUsage = reader.Read<UnityEngine.Rendering.ReflectionProbeUsage>(ES3Type_enum.Instance);
						break;
					case "sortingLayerName":
						instance.sortingLayerName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "sortingLayerID":
						instance.sortingLayerID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingOrder":
						instance.sortingOrder = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "lightProbeProxyVolumeOverride":
						instance.lightProbeProxyVolumeOverride = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "probeAnchor":
						instance.probeAnchor = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "lightmapIndex":
						instance.lightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "realtimeLightmapIndex":
						instance.realtimeLightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "lightmapScaleOffset":
						instance.lightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "realtimeLightmapScaleOffset":
						instance.realtimeLightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "materials":
						instance.materials = reader.Read<UnityEngine.Material[]>(ES3Type_MaterialArray.Instance);
						break;
					case "material":
						instance.material = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "sharedMaterial":
						instance.sharedMaterial = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "sharedMaterials":
						instance.sharedMaterials = reader.Read<UnityEngine.Material[]>(ES3Type_MaterialArray.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SpriteRendererArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SpriteRendererArray() : base(typeof(UnityEngine.SpriteRenderer[]), ES3UserType_SpriteRenderer.Instance)
		{
			Instance = this;
		}
	}
}