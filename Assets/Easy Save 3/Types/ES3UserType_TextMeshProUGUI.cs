using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_hasFontAssetChanged", "m_baseMaterial", "m_maskOffset", "k_CalculateVerticesPositionMarker", "m_text", "m_TextPreprocessor", "m_isRightToLeft", "m_fontAsset", "m_sharedMaterial", "m_fontSharedMaterials", "m_fontMaterial", "m_fontMaterials", "m_fontColor32", "m_fontColor", "m_enableVertexGradient", "m_colorMode", "m_fontColorGradient", "m_fontColorGradientPreset", "m_spriteAsset", "m_tintAllSprites", "m_StyleSheet", "m_TextStyleHashCode", "m_overrideHtmlColors", "m_faceColor", "m_fontSize", "m_fontSizeBase", "m_fontWeight", "m_enableAutoSizing", "m_fontSizeMin", "m_fontSizeMax", "m_fontStyle", "m_HorizontalAlignment", "m_VerticalAlignment", "m_textAlignment", "m_characterSpacing", "m_wordSpacing", "m_lineSpacing", "m_lineSpacingMax", "m_paragraphSpacing", "m_charWidthMaxAdj", "m_enableWordWrapping", "m_wordWrappingRatios", "m_overflowMode", "m_linkedTextComponent", "parentLinkedComponent", "m_enableKerning", "m_enableExtraPadding", "checkPaddingRequired", "m_isRichText", "m_parseCtrlCharacters", "m_isOrthographic", "m_isCullingEnabled", "m_horizontalMapping", "m_verticalMapping", "m_uvLineOffset", "m_geometrySortingOrder", "m_IsTextObjectScaleStatic", "m_VertexBufferAutoSizeReduction", "m_useMaxVisibleDescender", "m_pageToDisplay", "m_margin", "m_isUsingLegacyAnimationComponent", "m_isVolumetricText")]
	public class ES3UserType_TextMeshProUGUI : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TextMeshProUGUI() : base(typeof(TMPro.TextMeshProUGUI)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (TMPro.TextMeshProUGUI)obj;
			
			writer.WritePrivateField("m_hasFontAssetChanged", instance);
			writer.WritePrivateFieldByRef("m_baseMaterial", instance);
			writer.WritePrivateField("m_maskOffset", instance);
			writer.WritePrivateField("k_CalculateVerticesPositionMarker", instance);
			writer.WritePrivateField("m_text", instance);
			writer.WritePrivateField("m_TextPreprocessor", instance);
			writer.WritePrivateField("m_isRightToLeft", instance);
			writer.WritePrivateFieldByRef("m_fontAsset", instance);
			writer.WritePrivateFieldByRef("m_sharedMaterial", instance);
			writer.WritePrivateField("m_fontSharedMaterials", instance);
			writer.WritePrivateFieldByRef("m_fontMaterial", instance);
			writer.WritePrivateField("m_fontMaterials", instance);
			writer.WritePrivateField("m_fontColor32", instance);
			writer.WritePrivateField("m_fontColor", instance);
			writer.WritePrivateField("m_enableVertexGradient", instance);
			writer.WritePrivateField("m_colorMode", instance);
			writer.WritePrivateField("m_fontColorGradient", instance);
			writer.WritePrivateFieldByRef("m_fontColorGradientPreset", instance);
			writer.WritePrivateFieldByRef("m_spriteAsset", instance);
			writer.WritePrivateField("m_tintAllSprites", instance);
			writer.WritePrivateFieldByRef("m_StyleSheet", instance);
			writer.WritePrivateField("m_TextStyleHashCode", instance);
			writer.WritePrivateField("m_overrideHtmlColors", instance);
			writer.WritePrivateField("m_faceColor", instance);
			writer.WritePrivateField("m_fontSize", instance);
			writer.WritePrivateField("m_fontSizeBase", instance);
			writer.WritePrivateField("m_fontWeight", instance);
			writer.WritePrivateField("m_enableAutoSizing", instance);
			writer.WritePrivateField("m_fontSizeMin", instance);
			writer.WritePrivateField("m_fontSizeMax", instance);
			writer.WritePrivateField("m_fontStyle", instance);
			writer.WritePrivateField("m_HorizontalAlignment", instance);
			writer.WritePrivateField("m_VerticalAlignment", instance);
			writer.WritePrivateField("m_textAlignment", instance);
			writer.WritePrivateField("m_characterSpacing", instance);
			writer.WritePrivateField("m_wordSpacing", instance);
			writer.WritePrivateField("m_lineSpacing", instance);
			writer.WritePrivateField("m_lineSpacingMax", instance);
			writer.WritePrivateField("m_paragraphSpacing", instance);
			writer.WritePrivateField("m_charWidthMaxAdj", instance);
			writer.WritePrivateField("m_enableWordWrapping", instance);
			writer.WritePrivateField("m_wordWrappingRatios", instance);
			writer.WritePrivateField("m_overflowMode", instance);
			writer.WritePrivateFieldByRef("m_linkedTextComponent", instance);
			writer.WritePrivateFieldByRef("parentLinkedComponent", instance);
			writer.WritePrivateField("m_enableKerning", instance);
			writer.WritePrivateField("m_enableExtraPadding", instance);
			writer.WritePrivateField("checkPaddingRequired", instance);
			writer.WritePrivateField("m_isRichText", instance);
			writer.WritePrivateField("m_parseCtrlCharacters", instance);
			writer.WritePrivateField("m_isOrthographic", instance);
			writer.WritePrivateField("m_isCullingEnabled", instance);
			writer.WritePrivateField("m_horizontalMapping", instance);
			writer.WritePrivateField("m_verticalMapping", instance);
			writer.WritePrivateField("m_uvLineOffset", instance);
			writer.WritePrivateField("m_geometrySortingOrder", instance);
			writer.WritePrivateField("m_IsTextObjectScaleStatic", instance);
			writer.WritePrivateField("m_VertexBufferAutoSizeReduction", instance);
			writer.WritePrivateField("m_useMaxVisibleDescender", instance);
			writer.WritePrivateField("m_pageToDisplay", instance);
			writer.WritePrivateField("m_margin", instance);
			writer.WritePrivateField("m_isUsingLegacyAnimationComponent", instance);
			writer.WritePrivateField("m_isVolumetricText", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (TMPro.TextMeshProUGUI)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_hasFontAssetChanged":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_hasFontAssetChanged", reader.Read<System.Boolean>(), instance);
					break;
					case "m_baseMaterial":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_baseMaterial", reader.Read<UnityEngine.Material>(), instance);
					break;
					case "m_maskOffset":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_maskOffset", reader.Read<UnityEngine.Vector4>(), instance);
					break;
					case "k_CalculateVerticesPositionMarker":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("k_CalculateVerticesPositionMarker", reader.Read<Unity.Profiling.ProfilerMarker>(), instance);
					break;
					case "m_text":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_text", reader.Read<System.String>(), instance);
					break;
					case "m_TextPreprocessor":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_TextPreprocessor", reader.Read<TMPro.ITextPreprocessor>(), instance);
					break;
					case "m_isRightToLeft":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isRightToLeft", reader.Read<System.Boolean>(), instance);
					break;
					case "m_fontAsset":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontAsset", reader.Read<TMPro.TMP_FontAsset>(), instance);
					break;
					case "m_sharedMaterial":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_sharedMaterial", reader.Read<UnityEngine.Material>(), instance);
					break;
					case "m_fontSharedMaterials":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontSharedMaterials", reader.Read<UnityEngine.Material[]>(), instance);
					break;
					case "m_fontMaterial":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontMaterial", reader.Read<UnityEngine.Material>(), instance);
					break;
					case "m_fontMaterials":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontMaterials", reader.Read<UnityEngine.Material[]>(), instance);
					break;
					case "m_fontColor32":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontColor32", reader.Read<UnityEngine.Color32>(), instance);
					break;
					case "m_fontColor":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontColor", reader.Read<UnityEngine.Color>(), instance);
					break;
					case "m_enableVertexGradient":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_enableVertexGradient", reader.Read<System.Boolean>(), instance);
					break;
					case "m_colorMode":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_colorMode", reader.Read<TMPro.ColorMode>(), instance);
					break;
					case "m_fontColorGradient":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontColorGradient", reader.Read<TMPro.VertexGradient>(), instance);
					break;
					case "m_fontColorGradientPreset":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontColorGradientPreset", reader.Read<TMPro.TMP_ColorGradient>(), instance);
					break;
					case "m_spriteAsset":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_spriteAsset", reader.Read<TMPro.TMP_SpriteAsset>(), instance);
					break;
					case "m_tintAllSprites":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_tintAllSprites", reader.Read<System.Boolean>(), instance);
					break;
					case "m_StyleSheet":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_StyleSheet", reader.Read<TMPro.TMP_StyleSheet>(), instance);
					break;
					case "m_TextStyleHashCode":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_TextStyleHashCode", reader.Read<System.Int32>(), instance);
					break;
					case "m_overrideHtmlColors":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_overrideHtmlColors", reader.Read<System.Boolean>(), instance);
					break;
					case "m_faceColor":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_faceColor", reader.Read<UnityEngine.Color32>(), instance);
					break;
					case "m_fontSize":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontSize", reader.Read<System.Single>(), instance);
					break;
					case "m_fontSizeBase":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontSizeBase", reader.Read<System.Single>(), instance);
					break;
					case "m_fontWeight":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontWeight", reader.Read<TMPro.FontWeight>(), instance);
					break;
					case "m_enableAutoSizing":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_enableAutoSizing", reader.Read<System.Boolean>(), instance);
					break;
					case "m_fontSizeMin":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontSizeMin", reader.Read<System.Single>(), instance);
					break;
					case "m_fontSizeMax":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontSizeMax", reader.Read<System.Single>(), instance);
					break;
					case "m_fontStyle":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_fontStyle", reader.Read<TMPro.FontStyles>(), instance);
					break;
					case "m_HorizontalAlignment":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_HorizontalAlignment", reader.Read<TMPro.HorizontalAlignmentOptions>(), instance);
					break;
					case "m_VerticalAlignment":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_VerticalAlignment", reader.Read<TMPro.VerticalAlignmentOptions>(), instance);
					break;
					case "m_textAlignment":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_textAlignment", reader.Read<TMPro.TextAlignmentOptions>(), instance);
					break;
					case "m_characterSpacing":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_characterSpacing", reader.Read<System.Single>(), instance);
					break;
					case "m_wordSpacing":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_wordSpacing", reader.Read<System.Single>(), instance);
					break;
					case "m_lineSpacing":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_lineSpacing", reader.Read<System.Single>(), instance);
					break;
					case "m_lineSpacingMax":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_lineSpacingMax", reader.Read<System.Single>(), instance);
					break;
					case "m_paragraphSpacing":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_paragraphSpacing", reader.Read<System.Single>(), instance);
					break;
					case "m_charWidthMaxAdj":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_charWidthMaxAdj", reader.Read<System.Single>(), instance);
					break;
					case "m_enableWordWrapping":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_enableWordWrapping", reader.Read<System.Boolean>(), instance);
					break;
					case "m_wordWrappingRatios":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_wordWrappingRatios", reader.Read<System.Single>(), instance);
					break;
					case "m_overflowMode":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_overflowMode", reader.Read<TMPro.TextOverflowModes>(), instance);
					break;
					case "m_linkedTextComponent":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_linkedTextComponent", reader.Read<TMPro.TMP_Text>(), instance);
					break;
					case "parentLinkedComponent":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("parentLinkedComponent", reader.Read<TMPro.TMP_Text>(), instance);
					break;
					case "m_enableKerning":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_enableKerning", reader.Read<System.Boolean>(), instance);
					break;
					case "m_enableExtraPadding":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_enableExtraPadding", reader.Read<System.Boolean>(), instance);
					break;
					case "checkPaddingRequired":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("checkPaddingRequired", reader.Read<System.Boolean>(), instance);
					break;
					case "m_isRichText":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isRichText", reader.Read<System.Boolean>(), instance);
					break;
					case "m_parseCtrlCharacters":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_parseCtrlCharacters", reader.Read<System.Boolean>(), instance);
					break;
					case "m_isOrthographic":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isOrthographic", reader.Read<System.Boolean>(), instance);
					break;
					case "m_isCullingEnabled":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isCullingEnabled", reader.Read<System.Boolean>(), instance);
					break;
					case "m_horizontalMapping":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_horizontalMapping", reader.Read<TMPro.TextureMappingOptions>(), instance);
					break;
					case "m_verticalMapping":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_verticalMapping", reader.Read<TMPro.TextureMappingOptions>(), instance);
					break;
					case "m_uvLineOffset":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_uvLineOffset", reader.Read<System.Single>(), instance);
					break;
					case "m_geometrySortingOrder":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_geometrySortingOrder", reader.Read<TMPro.VertexSortingOrder>(), instance);
					break;
					case "m_IsTextObjectScaleStatic":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_IsTextObjectScaleStatic", reader.Read<System.Boolean>(), instance);
					break;
					case "m_VertexBufferAutoSizeReduction":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_VertexBufferAutoSizeReduction", reader.Read<System.Boolean>(), instance);
					break;
					case "m_useMaxVisibleDescender":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_useMaxVisibleDescender", reader.Read<System.Boolean>(), instance);
					break;
					case "m_pageToDisplay":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_pageToDisplay", reader.Read<System.Int32>(), instance);
					break;
					case "m_margin":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_margin", reader.Read<UnityEngine.Vector4>(), instance);
					break;
					case "m_isUsingLegacyAnimationComponent":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isUsingLegacyAnimationComponent", reader.Read<System.Boolean>(), instance);
					break;
					case "m_isVolumetricText":
					instance = (TMPro.TextMeshProUGUI)reader.SetPrivateField("m_isVolumetricText", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TextMeshProUGUIArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TextMeshProUGUIArray() : base(typeof(TMPro.TextMeshProUGUI[]), ES3UserType_TextMeshProUGUI.Instance)
		{
			Instance = this;
		}
	}
}