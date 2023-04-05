using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;

//using NHibernate.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class BodyInfo : ICanRegisterEvent {
    //https://stackoverflow.com/questions/35577011/custom-string-formatter-in-c-sharp
    public BodyPartPrefabInfo HeadInfoPrefab;
    public BodyPartPrefabInfo MainBodyInfoPrefab;
    public BodyPartPrefabInfo LegInfoPreab;
    public bool IsAlien = false;
    public long ID;
    public string BuiltBodyOverriddenPrefabName;
    /// <summary>
    /// ¥”œ¬Õ˘…œ
    /// </summary>
    public List<BodyPartPrefabInfo> AllBodyInfoPrefabs = new List<BodyPartPrefabInfo>();



    public IVoiceTag VoiceTag = null;

    public IKnockBehavior KnockBehavior = null;
    public override bool Equals(object obj) {
        // If parameter is null return false.
        if (obj == null) {
            return false;
        }
        if (obj is BodyInfo) {
            return ID == ((BodyInfo) obj).ID;
        }
        return false;
    }

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
            foreach (IAlienTag tag in bodyInfo.AllTags) {
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
    public BodyInfo() {
        this.RegisterEvent<OnBodyInfoBecomeAlien>(OnBodyInfoBecomeAlien);
        this.RegisterEvent<OnBodyInfoKilled>(OnBodyInfoKilled);
    }
    
    private BodyInfo(IVoiceTag voiceTag, BodyPartPrefabInfo headInfoPrefab, BodyPartPrefabInfo mainBodyPartInfoPrefab, BodyPartPrefabInfo legInfoPreab, BodyPartDisplayType displayType,
        HeightType height, IKnockBehavior knockBehavior, string builtBodyOverriddenPrefabName): this() {
       
        HeadInfoPrefab = headInfoPrefab;
        MainBodyInfoPrefab = mainBodyPartInfoPrefab;
        LegInfoPreab = legInfoPreab;
        this.VoiceTag = voiceTag;
        if (displayType == BodyPartDisplayType.Shadow) {
            AllBodyInfoPrefabs = new List<BodyPartPrefabInfo>() { legInfoPreab, mainBodyPartInfoPrefab, headInfoPrefab };
        }else if (displayType == BodyPartDisplayType.Newspaper) {
            AllBodyInfoPrefabs = new List<BodyPartPrefabInfo>() { headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab };
        }
       
        this.DisplayType = displayType;
        this.Height = height;
        this.ID = Random.Range(-1000000000, 1000000000);
        this.BuiltBodyOverriddenPrefabName = builtBodyOverriddenPrefabName;
        this.KnockBehavior = knockBehavior;
    }

  
    private void OnBodyInfoKilled(OnBodyInfoKilled e) {
        if (e.ID == ID) {
            IsDead = true;
            UnregisterEvents();
        }
    }

    private void UnregisterEvents() {
        this.UnRegisterEvent<OnBodyInfoBecomeAlien>(OnBodyInfoBecomeAlien);
        this.UnRegisterEvent<OnBodyInfoKilled>(OnBodyInfoKilled);
    }

    private void OnBodyInfoBecomeAlien(OnBodyInfoBecomeAlien e) {
        if (e.ID == ID) {
            this.IsAlien = true;
        }
    }


    #region Static
    
    public static BodyInfo GetRandomBodyInfo(BodyPartDisplayType displayType, bool isAlien, 
        float distinctiveWeight, IKnockBehavior knockBehavior,  Dictionary<BodyPartType, HashSet<int>> usedIndices, int subBodyPartChance, string builtBodyOverriddenPrefabName = "") {
        Gender[] voiceValues = (Gender[]) Enum.GetValues(typeof(Gender));
        HeightType height = Random.Range(0, 2) == 0 ? HeightType.Short : HeightType.Tall;
        //Gender gender = voiceValues[Random.Range(0, voiceValues.Length)];
        
        int bodyPartTypeEnumLength = Enum.GetValues(typeof(BodyPartType)).Length-1;
        List<BodyPartPrefabInfo> allBodyPartInfos = new List<BodyPartPrefabInfo>(3);

        int distinctBodyPartCount = Mathf.RoundToInt(3 * distinctiveWeight);
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
            HashSet<int> usedIndicesInThisType = null;
            if (usedIndices!=null && usedIndices.ContainsKey((BodyPartType) i)) {
                usedIndicesInThisType = usedIndices?[(BodyPartType) i];
            }
           
            BodyPartPrefabInfo part = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType,
                (BodyPartType) i, isAlien, height, isDistinct, usedIndicesInThisType, subBodyPartChance);
            allBodyPartInfos.Add(part);
        }

        IVoiceTag voiceTag =
            new VoiceTag(
                AudioMixerList.Singleton.AlienVoiceGroups[
                    Random.Range(0, AudioMixerList.Singleton.AlienVoiceGroups.Count)]);
        
        return GetBodyInfo(allBodyPartInfos[2], allBodyPartInfos[1], allBodyPartInfos[0], height, voiceTag,knockBehavior, displayType, isAlien, builtBodyOverriddenPrefabName);
    }

    public static BodyInfo GetBodyInfo(BodyPartPrefabInfo leg, BodyPartPrefabInfo body, BodyPartPrefabInfo head,
        HeightType height, IVoiceTag voiceTag, IKnockBehavior knockBehavior, BodyPartDisplayType displayType, bool isAlien, string builtBodyOverriddenPrefabName = "") {
        
        BodyInfo info = new BodyInfo(voiceTag, head, body, leg, displayType,
            height, knockBehavior, builtBodyOverriddenPrefabName);
        info.IsAlien = isAlien;
        return info;
    }

    public static BodyInfo GetBodyInfoOpposite(BodyInfo original, float bodyPartOppositeChance, float heightOppositeChance, bool originalIsAlien, bool isDistinct, Dictionary<BodyPartType, HashSet<int>> usedIndices, string builtBodyOverriddenPrefabName = "") {
        BodyPartPrefabInfo headInfoPrefab = original.HeadInfoPrefab;
        BodyPartPrefabInfo mainBodyPartInfoPrefab = original.MainBodyInfoPrefab;
        BodyPartPrefabInfo legInfoPreab = original.LegInfoPreab;
        HeightType height = original.Height;

        bool heightOpposite = heightOppositeChance >= Random.Range(0f, 1f);
       
        
        legInfoPreab = GetOpposite(original.DisplayType, original.LegInfoPreab, BodyPartType.Legs, height, originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct, null,
            original.LegInfoPreab.SubBodyPartChance);
        mainBodyPartInfoPrefab = GetOpposite(original.DisplayType, original.MainBodyInfoPrefab, BodyPartType.Body,
            height, originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct,usedIndices?[BodyPartType.Body], original.MainBodyInfoPrefab.SubBodyPartChance);
        headInfoPrefab = GetOpposite(original.DisplayType, original.HeadInfoPrefab, BodyPartType.Head, height,
            originalIsAlien, heightOpposite, bodyPartOppositeChance, isDistinct, usedIndices?[BodyPartType.Head],
            original.HeadInfoPrefab.SubBodyPartChance);



        IVoiceTag voice = original.VoiceTag.GetOpposite();
        IKnockBehavior knockBehavior = original.KnockBehavior.GetOpposite();
        height = heightOpposite ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short) : height;

        BodyInfo info = new BodyInfo(voice, headInfoPrefab, mainBodyPartInfoPrefab, legInfoPreab, original.DisplayType,
            height,knockBehavior, builtBodyOverriddenPrefabName);
        info.IsAlien = !originalIsAlien;
        return info;
    }


    public static BodyInfo GetBodyInfoForDisplay(BodyInfo original, BodyPartDisplayType targetDisplayType,  bool isSpecial, float reality = 1, bool sameID = true, string builtBodyOverriddenPrefabName = "") {
        HeightType height = original.Height;
        
        BodyPartPrefabInfo head = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.HeadInfoPrefab?.BodyPartInfo, height, reality, isSpecial, original.HeadInfoPrefab==null? -1: original.HeadInfoPrefab.SubBodyPartInfoIndex);

        BodyPartPrefabInfo body = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType,
            original.DisplayType, original.MainBodyInfoPrefab?.BodyPartInfo, height, reality, isSpecial, original.MainBodyInfoPrefab==null? -1: original.MainBodyInfoPrefab.SubBodyPartInfoIndex);

        BodyPartPrefabInfo leg = AlienBodyPartCollections.Singleton.GetBodyPartInfoForDisplay(targetDisplayType, original.DisplayType,
            original.LegInfoPreab?.BodyPartInfo, height, reality, isSpecial, original.LegInfoPreab==null? -1: original.LegInfoPreab.SubBodyPartInfoIndex);

        BodyInfo info = new BodyInfo(original.VoiceTag, head, body, leg, targetDisplayType, height,
            original.KnockBehavior,builtBodyOverriddenPrefabName);
        info.IsAlien = original.IsAlien;
        if (sameID) {
            info.ID = original.ID;
        }
        return info;
    }
    
    private static BodyPartPrefabInfo GetOpposite(BodyPartDisplayType displayType, BodyPartPrefabInfo original, BodyPartType bodyPartType,  HeightType height, bool originalIsAlien, bool changeHeight,
        float changeToOppositeChance, bool isDistinct, HashSet<int> usedIndices, int subBodyPartChance) {
        if (originalIsAlien) {
            if (original.BodyPartInfo.IsAlienOnly) {
                return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType, false, height, isDistinct, usedIndices, subBodyPartChance);
            }else {
               // if (Random.Range(0f, 1f) < oppositeChance) {
               BodyPartPrefabInfo heightReflectedBodyPartInfo = original;
                   if (changeHeight) {
                       heightReflectedBodyPartInfo = AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original.BodyPartInfo, false, height);
                   }
                   
                   HeightType targetHeight = changeHeight
                       ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short)
                       : height;
                   
                if (heightReflectedBodyPartInfo != null) {
                    if (changeToOppositeChance < Random.Range(0f, 1f)) {
                        return heightReflectedBodyPartInfo;
                    }
                    
                       if (heightReflectedBodyPartInfo.BodyPartInfo.OppositeTraitBodyParts.Any()) {
                           return heightReflectedBodyPartInfo.BodyPartInfo
                               .OppositeTraitBodyParts[
                                   Random.Range(0,
                                       heightReflectedBodyPartInfo.BodyPartInfo.OppositeTraitBodyParts.Count)]
                               .GetComponent<AlienBodyPartInfo>()
                               .GetBodyPartPrefabInfo();
                       }else {
                           return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType,
                               false,
                               targetHeight, isDistinct,usedIndices, 40);
                       }
                   }
                   //}
            }
        }else {
            //if (Random.Range(0f, 1f) < oppositeChance) {
                BodyPartPrefabInfo heightReflectedBodyPartInfo = original;
                if (changeHeight)
                {
                    heightReflectedBodyPartInfo = AlienBodyPartCollections.Singleton.TryGetHeightOppositeBodyPartInfo(displayType, original.BodyPartInfo, false, height);
                }

                HeightType targetHeight = changeHeight
                    ? (height == HeightType.Short ? HeightType.Tall : HeightType.Short)
                    : height;

                if (heightReflectedBodyPartInfo != null) {
                    if (changeToOppositeChance < Random.Range(0f, 1f))
                    {
                        return heightReflectedBodyPartInfo;
                    }
                    if (heightReflectedBodyPartInfo.BodyPartInfo.OppositeTraitBodyParts.Any()) {
                        return heightReflectedBodyPartInfo.BodyPartInfo
                            .OppositeTraitBodyParts[
                                Random.Range(0, heightReflectedBodyPartInfo.BodyPartInfo.OppositeTraitBodyParts.Count)]
                            .GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();
                    }
                    else
                    {
                    return AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(displayType, bodyPartType,
                        true,
                        targetHeight, isDistinct, usedIndices, 40);
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
