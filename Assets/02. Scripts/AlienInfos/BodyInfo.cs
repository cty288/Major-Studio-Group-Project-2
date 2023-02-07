using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;

//using NHibernate.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class BodyInfo : ICanRegisterEvent {
    //https://stackoverflow.com/questions/35577011/custom-string-formatter-in-c-sharp
    public AlienBodyPartInfo HeadInfoPrefab;
    public AlienBodyPartInfo MainBodyInfoPrefab;
    public AlienBodyPartInfo LegInfoPreab;
    public bool IsAlien = false;
    public long ID;
    public string BuiltBodyOverriddenPrefabName;
    /// <summary>
    /// ¥”œ¬Õ˘…œ
    /// </summary>
    public List<AlienBodyPartInfo> AllBodyInfoPrefabs = new List<AlienBodyPartInfo>();



    public IVoiceTag VoiceTag = null;
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

    public bool IsDead = false;
    

    public BodyPartDisplayType DisplayType;
    private BodyInfo() {

    }
    private BodyInfo(Gender voiceType, AlienBodyPartInfo headInfoPrefab, AlienBodyPartInfo mainBodyPartInfoPrefab, AlienBodyPartInfo legInfoPreab, BodyPartDisplayType displayType,
        HeightType height, string builtBodyOverriddenPrefabName) {
        this.RegisterEvent<OnBodyInfoBecomeAlien>(OnBodyInfoBecomeAlien);
        this.RegisterEvent<OnBodyInfoRemoved>(OnBodyInfoRemoved);
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
        this.ID = Random.Range(-1000000000, 1000000000);
        this.BuiltBodyOverriddenPrefabName = builtBodyOverriddenPrefabName;
    }

    private void OnBodyInfoRemoved(OnBodyInfoRemoved e) {
        if (e.ID == ID) {
            IsDead = true;
            UnregisterEvents();
        }
    }

    private void UnregisterEvents() {
        this.UnRegisterEvent<OnBodyInfoBecomeAlien>(OnBodyInfoBecomeAlien);
        this.UnRegisterEvent<OnBodyInfoRemoved>(OnBodyInfoRemoved);
    }

    private void OnBodyInfoBecomeAlien(OnBodyInfoBecomeAlien e) {
        if (e.ID == ID) {
            this.IsAlien = true;
        }
    }


    #region Static
    
    public static BodyInfo GetRandomBodyInfo(BodyPartDisplayType displayType, bool isAlien, 
        bool needDistinctive, string builtBodyOverriddenPrefabName = "") {
        Gender[] voiceValues = (Gender[]) Enum.GetValues(typeof(Gender));
        HeightType height = Random.Range(0, 2) == 0 ? HeightType.Short : HeightType.Tall;
        Gender gender = voiceValues[Random.Range(0, voiceValues.Length)];
        
        int bodyPartTypeEnumLength = Enum.GetValues(typeof(BodyPartType)).Length;
        List<AlienBodyPartInfo> allBodyPartInfos = new List<AlienBodyPartInfo>(3);

        int distinctBodyPartCount = needDistinctive ? Random.Range(1, 4) : 0;
        List<int> distinctBodyPartIndexes = new List<int>();
        for (int i = 0; i < distinctBodyPartCount; i++) {
            int distinctBodyPartIndex = Random.Range(0, bodyPartTypeEnumLength);
            while (distinctBodyPartIndexes.Contains(distinctBodyPartIndex)) {
                distinctBodyPartIndex = Random.Range(0, bodyPartTypeEnumLength);
            }
            distinctBodyPartIndexes.Add(distinctBodyPartIndex);
        }
        
        for (int i = 0; i < bodyPartTypeEnumLength; i++) {
            bool isDistinct = distinctBodyPartIndexes.Contains(i);
            AlienBodyPartInfo part = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, (BodyPartType)i, isAlien, height,isDistinct);
            allBodyPartInfos.Add(part);
        }
        
        return GetBodyInfo(allBodyPartInfos[2], allBodyPartInfos[1], allBodyPartInfos[0], height, gender, displayType, isAlien, builtBodyOverriddenPrefabName);
    }

    public static BodyInfo GetBodyInfo(AlienBodyPartInfo leg, AlienBodyPartInfo body, AlienBodyPartInfo head,
        HeightType height, Gender gender, BodyPartDisplayType displayType, bool isAlien, string builtBodyOverriddenPrefabName = "") {
        
        BodyInfo info = new BodyInfo(gender, head, body, leg, displayType,
            height, builtBodyOverriddenPrefabName);
        info.IsAlien = isAlien;
        return info;
    }

    public static BodyInfo GetBodyInfoOpposite(BodyInfo original, float bodyPartOppositeChance, float heightOppositeChance, bool originalIsAlien, bool isDistinct, string builtBodyOverriddenPrefabName = "") {
        AlienBodyPartInfo headInfoPrefab = original.HeadInfoPrefab;
        AlienBodyPartInfo mainBodyPartInfoPrefab = original.MainBodyInfoPrefab;
        AlienBodyPartInfo legInfoPreab = original.LegInfoPreab;
        HeightType height = original.Height;

        bool heightOpposite = heightOppositeChance >= Random.Range(0f, 1f);
       
        
        legInfoPreab = GetOpposite(original.DisplayType, original.LegInfoPreab, BodyPartType.Legs, height, originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct);
        mainBodyPartInfoPrefab = GetOpposite(original.DisplayType, original.MainBodyInfoPrefab, BodyPartType.Body,
            height, originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct);
        headInfoPrefab = GetOpposite(original.DisplayType, original.HeadInfoPrefab, BodyPartType.Head, height,
            originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct);



        Gender voice = GetOpposite(original.VoiceType, bodyPartOppositeChance);
        height = heightOpposite ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short) : height;

        BodyInfo info = new BodyInfo(voice, headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab, original.DisplayType,
            height, builtBodyOverriddenPrefabName);
        info.IsAlien = !originalIsAlien;
        return info;
    }


    public static BodyInfo GetBodyInfoForDisplay(BodyInfo original, BodyPartDisplayType targetDisplayType, float reality = 1, bool sameID = true, string builtBodyOverriddenPrefabName = "") {
        HeightType height = original.Height;
        AlienBodyPartInfo head = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.HeadInfoPrefab, height, reality);

        AlienBodyPartInfo body = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.MainBodyInfoPrefab, height, reality);

        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType, original.DisplayType,
            original.LegInfoPreab, height, reality);

        BodyInfo info = new BodyInfo(original.VoiceType, head, body, leg, targetDisplayType, height,
            builtBodyOverriddenPrefabName);
        info.IsAlien = original.IsAlien;
        if (sameID) {
            info.ID = original.ID;
        }
        return info;
    }
    
    private static AlienBodyPartInfo GetOpposite(BodyPartDisplayType displayType, AlienBodyPartInfo original, BodyPartType bodyPartType,  HeightType height, bool originalIsAlien, bool changeHeight,
        float changeToOppositeChance, bool isDistinct) {
        if (originalIsAlien) {
            if (original.IsAlienOnly) {
                return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, false, height, isDistinct);
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
                               targetHeight, isDistinct);
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
                        targetHeight, isDistinct);
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

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
