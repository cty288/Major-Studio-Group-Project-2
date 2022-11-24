using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales.RTVoice.Model.Enum;

//using NHibernate.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class BodyInfo {
    //https://stackoverflow.com/questions/35577011/custom-string-formatter-in-c-sharp
    public AlienBodyPartInfo HeadInfoPrefab;
    public AlienBodyPartInfo MainBodyInfoPrefab;
    public AlienBodyPartInfo LegInfoPreab;
    public bool IsAlien = false;
    /// <summary>
    /// ¥”œ¬Õ˘…œ
    /// </summary>
    public List<AlienBodyPartInfo> AllBodyInfoPrefabs = new List<AlienBodyPartInfo>();


    public Gender VoiceType;
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

    public IFatTag GetFatness() {
        if (CheckContainTag<IFatTag>(out IFatTag fatTag)) {
            return fatTag;
        }

        return null;
    }

    public HeightType Height;

    
    

    public BodyPartDisplayType DisplayType;
    private BodyInfo() {

    }
    private BodyInfo(Gender voiceType, AlienBodyPartInfo headInfoPrefab, AlienBodyPartInfo mainBodyPartInfoPrefab, AlienBodyPartInfo legInfoPreab, BodyPartDisplayType displayType,
        HeightType height) {
        HeadInfoPrefab = headInfoPrefab;
        MainBodyInfoPrefab = mainBodyPartInfoPrefab;
        LegInfoPreab = legInfoPreab;
        VoiceType = voiceType;
        if (displayType == BodyPartDisplayType.Shadow) {
            AllBodyInfoPrefabs = new List<AlienBodyPartInfo>() { legInfoPreab, mainBodyPartInfoPrefab, headInfoPrefab };
        }else if (displayType == BodyPartDisplayType.Newspaper) {
            AllBodyInfoPrefabs = new List<AlienBodyPartInfo>() { headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab };
        }
       
        this.DisplayType = displayType;
        this.Height = height;
    }


    #region Static
    
    public static BodyInfo GetRandomBodyInfo(BodyPartDisplayType displayType, bool isAlien) {
        Gender[] voiceValues = (Gender[]) Enum.GetValues(typeof(Gender));
        HeightType height = Random.Range(0, 2) == 0 ? HeightType.Short : HeightType.Tall;
        
        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, BodyPartType.Legs, isAlien, height);
        AlienBodyPartInfo body = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo( displayType, BodyPartType.Body, isAlien, height);
        AlienBodyPartInfo head = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, BodyPartType.Head, isAlien, height);
        return new BodyInfo(voiceValues[Random.Range(0, voiceValues.Length)], head, body, leg, displayType, height);
    }

    public static BodyInfo GetBodyInfoOpposite(BodyInfo original, float bodyPartOppositeChance, float heightOppositeChance, bool originalIsAlien) {
        AlienBodyPartInfo headInfoPrefab = original.HeadInfoPrefab;
        AlienBodyPartInfo mainBodyPartInfoPrefab = original.MainBodyInfoPrefab;
        AlienBodyPartInfo legInfoPreab = original.LegInfoPreab;
        HeightType height = original.Height;

        bool heightOpposite = heightOppositeChance >= Random.Range(0f, 1f);
       
        
        legInfoPreab = GetOpposite(original.DisplayType, original.LegInfoPreab, BodyPartType.Legs, height, originalIsAlien, heightOpposite, bodyPartOppositeChance);
        mainBodyPartInfoPrefab = GetOpposite(original.DisplayType, original.MainBodyInfoPrefab, BodyPartType.Body,
            height, originalIsAlien, heightOpposite, bodyPartOppositeChance);
        headInfoPrefab = GetOpposite(original.DisplayType, original.HeadInfoPrefab, BodyPartType.Head, height,
            originalIsAlien, heightOpposite, bodyPartOppositeChance);



        Gender voice = GetOpposite(original.VoiceType, bodyPartOppositeChance);
        height = heightOpposite ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short) : height;

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
    
    private static AlienBodyPartInfo GetOpposite(BodyPartDisplayType displayType, AlienBodyPartInfo original, BodyPartType bodyPartType,  HeightType height, bool originalIsAlien, bool changeHeight,
        float changeToOppositeChance) {
        if (originalIsAlien) {
            if (original.IsAlienOnly) {
                return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, false, height);
            }else {
               // if (Random.Range(0f, 1f) < oppositeChance) {
                   AlienBodyPartInfo heightReflectedBodyPartInfo = original;
                   if (changeHeight) {
                       heightReflectedBodyPartInfo = AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original, false, height);
                   }
                   
                   HeightType targetHeight = changeHeight
                       ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short)
                       : height;
                   
                if (heightReflectedBodyPartInfo != null) {
                    if (changeToOppositeChance < Random.Range(0f, 1f)) {
                        return heightReflectedBodyPartInfo;
                    }
                    
                       if (heightReflectedBodyPartInfo.OppositeTraitBodyParts.Any()) {
                           return heightReflectedBodyPartInfo.OppositeTraitBodyParts[Random.Range(0, heightReflectedBodyPartInfo.OppositeTraitBodyParts.Count)].GetComponent<AlienBodyPartInfo>();
                       }else {
                           return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType,
                               false,
                               targetHeight);
                       }
                   }
                   //}
            }
        }else {
            //if (Random.Range(0f, 1f) < oppositeChance) {
                AlienBodyPartInfo heightReflectedBodyPartInfo = original;
                if (changeHeight)
                {
                    heightReflectedBodyPartInfo = AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original, false, height);
                }

                HeightType targetHeight = changeHeight
                    ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short)
                    : height;

                if (heightReflectedBodyPartInfo != null) {
                    if (changeToOppositeChance < Random.Range(0f, 1f))
                    {
                        return heightReflectedBodyPartInfo;
                    }
                    if (heightReflectedBodyPartInfo.OppositeTraitBodyParts.Any())
                    {
                        return heightReflectedBodyPartInfo.OppositeTraitBodyParts[Random.Range(0, heightReflectedBodyPartInfo.OppositeTraitBodyParts.Count)].GetComponent<AlienBodyPartInfo>();
                    }
                    else
                    {
                    return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType,
                        true,
                        targetHeight);
                }
                }
                // }
        }
        
        return original;
    }
    private static Gender GetOpposite(Gender original, float oppositeChance) {
        bool isOpposite = Random.Range(0f, 1f) < oppositeChance;
        if (isOpposite) {
            if (original == Gender.FEMALE) {
                return Gender.MALE;
            }
            else {
                return Gender.FEMALE;
            }
        }

        return original;
    }
    #endregion

}
