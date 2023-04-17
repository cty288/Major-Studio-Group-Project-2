using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos;
using UnityEngine;
using Random = UnityEngine.Random;

public class DescriptionFormatter : IFormatProvider, ICustomFormatter {

    public static float Reality = 0.5f;

    private AlienNameModel _alienNameModel;

    private AlienNameModel alienNameModel {
        get {
            if (_alienNameModel == null) {
                _alienNameModel = MainGame.Interface.GetModel<AlienNameModel>();
            }
            return _alienNameModel;
        }
    }
    
    public object GetFormat(Type formatType) {
        if (formatType == typeof(ICustomFormatter)) {
            return this;
        }
        return null;
    }

    public string Format(string format, object arg, IFormatProvider formatProvider) {
        if (arg == null) {
            return String.Empty;
        }
        
        if (arg.GetType() == typeof(BodyInfo)) {
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
        }

        return arg.ToString();
    }

    private string GetVoiceDescription(BodyInfo body, float reality) {
        if (body.VoiceTag != null) {
            return body.VoiceTag.GetRandomRadioDescription(alienNameModel.AlienName, Random.Range(0f, 1f) < reality);
        }
        return "It's voice is unclear.";
    }

    public  string GetHeightDescriptions(BodyInfo body, float reality) {
        List<string> shortDescriptions = new List<string>() {
            "It is short."
        };
        List<string> highDescription = new List<string>() {
            "It has a relatively large body figure."
        };

        List<string> targetList = body.Height == HeightType.Short ? shortDescriptions : highDescription;
        if (Random.Range(0f, 1f) > reality) {
            targetList = targetList == shortDescriptions ? highDescription : shortDescriptions;
        }

        return targetList[Random.Range(0, targetList.Count)];
    }

    public  string GetByTag<T>(BodyInfo body, float reality) where T : class, IAlienTag {
        if (body.CheckContainTags<T>(out List<T> allTags)) {
            if (allTags.Count > 0) {
                string result = ((T)allTags[Random.Range(0, allTags.Count)]).GetRandomRadioDescription(alienNameModel.AlienName, IsReal(reality));
                int tryCount = 0;
                while (string.IsNullOrEmpty(result) && allTags.Count > 0 && tryCount < 100) {
                    result = ((T)allTags[Random.Range(0, allTags.Count)]).GetRandomRadioDescription(alienNameModel.AlienName, IsReal(reality));
                    tryCount++;

                }

                return result;
            }
            return String.Empty; 
        }

        return String.Empty;
    }

    public  string GetByTag<T>(BodyPartPrefabInfo bodyPart, float reality) where T : class, IAlienTag {
        var allTags = bodyPart.AllTags.FindAll(x => x is T);
        if (allTags.Count > 0) {
            string result = ((T)allTags[Random.Range(0, allTags.Count)]).GetRandomRadioDescription(alienNameModel.AlienName,IsReal(reality));
            int tryCount = 0;
            while (string.IsNullOrEmpty(result) && allTags.Count > 0 && tryCount < 100) {
                result = ((T)allTags[Random.Range(0, allTags.Count)]).GetRandomRadioDescription(alienNameModel.AlienName, IsReal(reality));
                tryCount++;

            }
            return result;
        }
        return String.Empty;
    }

    private static bool IsReal(float reality) {
        return Random.Range(0f, 1f) < reality;
    }
}
