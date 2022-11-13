using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//using NHibernate.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AlienVoiceType {
    Male,
    Female
}

public class BodyInfo {
    //https://stackoverflow.com/questions/35577011/custom-string-formatter-in-c-sharp
    public AlienBodyPartInfo HeadInfoPrefab;
    public AlienBodyPartInfo MainBodyInfoPrefab;
    public AlienBodyPartInfo LegInfoPreab;

    /// <summary>
    /// ¥”œ¬Õ˘…œ
    /// </summary>
    public List<AlienBodyPartInfo> AllBodyInfoPrefabs = new List<AlienBodyPartInfo>();


    public AlienVoiceType VoiceType;
    public bool CheckContainTag<T>(out T tag) where T : class, IAlienTag {
        tag = null;
        bool hasTags = CheckContainTags(out List<T> tags);
        if (hasTags) {
            tag = tags[Random.Range(0, tags.Count)];
        }
        return hasTags;
    }

    public bool CheckContainTags<T>(out List<T> tags) where T : IAlienTag {
        List<T> allTags = new List<T>();
        foreach (var bodyInfo in AllBodyInfoPrefabs) {
            foreach (IAlienTag tag in bodyInfo.Tags) {
                if (tag is T t) {
                    allTags.Add(t);
                }
            }
        }
        tags = allTags;
        return allTags.Count > 0;
    }

    public FatType GetFatness() {
        if (CheckContainTag<IFatTag>(out IFatTag fatTag)) {
            return fatTag.Fatness;
        }
        return FatType.Fat;
    }

    public HeightType Height;

    
    

    public BodyPartDisplayType DisplayType;
    private BodyInfo() {

    }
    private BodyInfo(AlienVoiceType voiceType, AlienBodyPartInfo headInfoPrefab, AlienBodyPartInfo mainBodyPartInfoPrefab, AlienBodyPartInfo legInfoPreab, BodyPartDisplayType displayType,
        HeightType height) {
        HeadInfoPrefab = headInfoPrefab;
        MainBodyInfoPrefab = mainBodyPartInfoPrefab;
        LegInfoPreab = legInfoPreab;
        VoiceType = voiceType;
        AllBodyInfoPrefabs = new List<AlienBodyPartInfo>() { legInfoPreab, mainBodyPartInfoPrefab, headInfoPrefab};
        this.DisplayType = displayType;
        this.Height = height;
    }


    #region Static
    
    public static BodyInfo GetRandomBodyInfo(BodyPartDisplayType displayType, bool isAlien) {
        AlienVoiceType[] voiceValues = (AlienVoiceType[]) Enum.GetValues(typeof(AlienVoiceType));
        HeightType height = Random.Range(0, 2) == 0 ? HeightType.Short : HeightType.Tall;
        
        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, BodyPartType.Legs, isAlien, height);
        AlienBodyPartInfo body = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo( displayType, BodyPartType.Body, isAlien, height);
        AlienBodyPartInfo head = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, BodyPartType.Head, isAlien, height);
        return new BodyInfo(voiceValues[Random.Range(0, voiceValues.Length)], head, body, leg, displayType, height);
    }

    public static BodyInfo GetBodyInfoOpposite(BodyInfo original, float oppositeChance, bool originalIsAlien) {
        AlienBodyPartInfo headInfoPrefab;
        AlienBodyPartInfo mainBodyPartInfoPrefab;
        AlienBodyPartInfo legInfoPreab;
        HeightType height = original.Height;
        
        legInfoPreab = GetOpposite(original.DisplayType, original.LegInfoPreab, BodyPartType.Legs, height, oppositeChance, originalIsAlien);
        headInfoPrefab = GetOpposite(original.DisplayType, original.HeadInfoPrefab, BodyPartType.Head, height, oppositeChance, originalIsAlien);
        mainBodyPartInfoPrefab = GetOpposite(original.DisplayType, original.MainBodyInfoPrefab, BodyPartType.Body, height, oppositeChance, originalIsAlien);
        
        
        AlienVoiceType voice = GetOpposite(original.VoiceType, oppositeChance);
        return new BodyInfo(voice, headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab, original.DisplayType, height);
    }


    public static BodyInfo GetBodyInfoForDisplay(BodyInfo original, BodyPartDisplayType targetDisplayType, float reality = 1) {
        HeightType height = original.Height;
        AlienBodyPartInfo head = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.HeadInfoPrefab, height, reality);

        AlienBodyPartInfo body = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.MainBodyInfoPrefab, height, reality);

        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType, original.DisplayType,
            original.LegInfoPreab, height, reality);
        
        return new BodyInfo(original.VoiceType, head, body, leg, targetDisplayType, height);
    }
    
    private static AlienBodyPartInfo GetOpposite(BodyPartDisplayType displayType, AlienBodyPartInfo original, BodyPartType bodyPartType, HeightType height, float oppositeChance, bool originalIsAlien) {
        if (originalIsAlien) {
            if (original.IsAlienOnly) {
                return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, false, height);
            }else {
                if (Random.Range(0f, 1f) < oppositeChance) {
                    AlienBodyPartInfo heightOpposite = AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original, false, height);

                    if (heightOpposite != null) {
                        if (heightOpposite.OppositeTraitBodyPart) {
                            return heightOpposite.OppositeTraitBodyPart.GetComponent<AlienBodyPartInfo>();
                        }else {
                            return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, false,
                                height == HeightType.Tall ? HeightType.Short : HeightType.Tall);
                        }
                    }
                }
            }
        }else {
            if (Random.Range(0f, 1f) < oppositeChance) {
                AlienBodyPartInfo heightOpposite =
                    AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original, false, height);

                if (heightOpposite != null) {
                    if (heightOpposite.OppositeTraitBodyPart) {
                        return heightOpposite.OppositeTraitBodyPart.GetComponent<AlienBodyPartInfo>();
                    }
                    else {
                        return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, true,
                            height == HeightType.Tall ? HeightType.Short : HeightType.Tall);
                    }
                }
            }
        }
        
        return original;
    }
    private static AlienVoiceType GetOpposite(AlienVoiceType original, float oppositeChance) {
        bool isOpposite = Random.Range(0f, 1f) < oppositeChance;
        if (isOpposite) {
            if (original == AlienVoiceType.Female) {
                return AlienVoiceType.Male;
            }
            else {
                return AlienVoiceType.Female;
            }
        }

        return original;
    }
    #endregion

}
