using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DescriptionFormatter : IFormatProvider, ICustomFormatter {

    public static float Reality = 0.5f;
    
    public object GetFormat(Type formatType) {
        if (formatType == typeof(ICustomFormatter)) {
            return this;
        }
        return null;
    }

    public string Format(string format, object arg, IFormatProvider formatProvider) {
        if (arg.GetType() != typeof(BodyInfo)) {
            return String.Empty;
        }

        BodyInfo body = (BodyInfo) arg;
        
        switch (format) {
            case "hair":
                return GetByTag<IHaircutTag>(body, Reality);
            case "cloth":
                return GetByTag<IClothTag>(body, Reality);
            case "clothb":
                return GetByTag<IClothTag>(body.MainBodyInfoPrefab, Reality);
            case "clothl":
                return GetByTag<IClothTag>(body.LegInfoPreab, Reality);
            case "fat":
                return GetByTag<IFatTag>(body, Reality);
            case "height":
                return GetHeightDescriptions(body, Reality);
            case "voice":
                return GetVoiceDescription(body, Reality);
            case "acc":
                return GetByTag<IAccessoryTag>(body, Reality);
        }
        return String.Empty;
    }

    private string GetVoiceDescription(BodyInfo body, float reality) {
        if (body.VoiceTag != null) {
            return body.VoiceTag.GetRandomRadioDescription(Random.Range(0f, 1f) < reality);
        }
        return "It's voice is unclear.";
    }

    public static string GetHeightDescriptions(BodyInfo body, float reality) {
        List<string> shortDescriptions = new List<string>() {
            "The victim is short."
        };
        List<string> highDescription = new List<string>() {
            "The victim has a relatively large body figure."
        };

        List<string> targetList = body.Height == HeightType.Short ? shortDescriptions : highDescription;
        if (Random.Range(0f, 1f) > reality)
        {
            targetList = targetList == shortDescriptions ? highDescription : shortDescriptions;
        }

        return targetList[Random.Range(0, targetList.Count)];
    }

    public static string GetByTag<T>(BodyInfo body, float reality) where T : class, IAlienTag {
        if (body.CheckContainTags<T>(out List<T> t)) {
            return t[Random.Range(0, t.Count)].GetRandomRadioDescription(IsReal(reality));
        }

        return String.Empty;
    }

    public static string GetByTag<T>(BodyPartPrefabInfo bodyPart, float reality) where T : class, IAlienTag {
        var allTags = bodyPart.Tags.FindAll(x => x is T);
        if (allTags.Count > 0) {
            return ((T) allTags[Random.Range(0, allTags.Count)]).GetRandomRadioDescription(IsReal(reality));
        }
        return String.Empty;
    }

    private static bool IsReal(float reality) {
        return Random.Range(0f, 1f) < reality;
    }
}
