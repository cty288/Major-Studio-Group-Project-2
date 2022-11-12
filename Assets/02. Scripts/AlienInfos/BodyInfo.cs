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

    public static bool instancesCreated = false;
    
    public static List<AlienBodyPartInfo> allBodyPartInstances = null;
    
    
   

    

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

    public float GetAverageFatness() {
        float totalFatness = 0;
        foreach (var bodyInfo in AllBodyInfoPrefabs) {
            totalFatness += bodyInfo.HeightTag.Height;
        }

        return totalFatness / AllBodyInfoPrefabs.Count;
    }

    public float GetTotalHeight() {
        float totalHeight = 0;
        foreach (var bodyInfo in AllBodyInfoPrefabs) {
            totalHeight += bodyInfo.HeightTag.Height;
        }
        return totalHeight;
    }

    
    private BodyInfo() {

    }
    private BodyInfo(AlienVoiceType voiceType, AlienBodyPartInfo headInfoPrefab, AlienBodyPartInfo mainBodyPartInfoPrefab, AlienBodyPartInfo legInfoPreab) {
        HeadInfoPrefab = headInfoPrefab;
        MainBodyInfoPrefab = mainBodyPartInfoPrefab;
        LegInfoPreab = legInfoPreab;
        VoiceType = voiceType;
        AllBodyInfoPrefabs = new List<AlienBodyPartInfo>() { legInfoPreab, mainBodyPartInfoPrefab, headInfoPrefab};
    }

    public static BodyInfo GetRandomBodyInfo(bool isAlien) {
        AlienVoiceType[] voiceValues = (AlienVoiceType[]) Enum.GetValues(typeof(AlienVoiceType));
        return new BodyInfo(voiceValues[Random.Range(0, voiceValues.Length)],
            AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartType.Head, isAlien),
            AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartType.Body, isAlien),
            AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartType.Legs, isAlien));
    }

    public static BodyInfo GetBodyInfoOpposite(BodyInfo original, float oppositeChance) {
        AlienBodyPartInfo headInfoPrefab;
        AlienBodyPartInfo mainBodyPartInfoPrefab;
        AlienBodyPartInfo legInfoPreab;

        headInfoPrefab = GetOpposite(original.HeadInfoPrefab, oppositeChance);
        mainBodyPartInfoPrefab = GetOpposite(original.MainBodyInfoPrefab, oppositeChance);
        legInfoPreab = GetOpposite(original.LegInfoPreab, oppositeChance);
        AlienVoiceType voice = GetOpposite(original.VoiceType, oppositeChance);
        return new BodyInfo(voice, headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab);
    }

    private static AlienBodyPartInfo GetOpposite(AlienBodyPartInfo original, float oppositeChance) {
        if (original.OppositeTraitBodyPart) {
            if (Random.Range(0f, 1f) < oppositeChance) {
                return original.OppositeTraitBodyPart.GetComponent<AlienBodyPartInfo>();
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

}
