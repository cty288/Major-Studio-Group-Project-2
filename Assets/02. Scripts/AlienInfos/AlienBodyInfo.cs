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

public class AlienBodyInfo {
    //https://stackoverflow.com/questions/35577011/custom-string-formatter-in-c-sharp
    public AlienBodyPartInfo HeadInfo;
    public AlienBodyPartInfo MainBodyInfo;
    public AlienBodyPartInfo LegInfo;

    private List<AlienBodyPartInfo> allBodyInfos = new List<AlienBodyPartInfo>();


    public AlienVoiceType VoiceType;

    public static bool instancesCreated = false;
    
    public static List<AlienBodyPartInfo> allBodyPartInstances = null;
    
    
    public static List<AlienBodyPartInfo> headBodyPartInstances = null;
    public static List<AlienBodyPartInfo> mainBodyPartInstances = null;
    public static List<AlienBodyPartInfo> legBodyPartInstances = null;

    public static void CreateAllInstances() {
        allBodyPartInstances =  typeof(AlienBodyInfo).Assembly.GetTypes() 
            .Where(t => typeof(AlienBodyPartInfo).IsAssignableFrom(t)) 
            .Where(t => !t.IsAbstract && t.IsClass) 
            .Select(t => (AlienBodyPartInfo)Activator.CreateInstance(t)).ToList();
        headBodyPartInstances = allBodyPartInstances.Where((info => info.BodyPartType == BodyPartType.Head)).ToList();
        mainBodyPartInstances = allBodyPartInstances.Where((info => info.BodyPartType == BodyPartType.Body)).ToList();
        legBodyPartInstances = allBodyPartInstances.Where((info => info.BodyPartType == BodyPartType.Legs)).ToList();
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
        foreach (var bodyInfo in allBodyInfos) {
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
        foreach (var bodyInfo in allBodyInfos) {
            totalFatness += bodyInfo.HeightTag.Height;
        }

        return totalFatness / allBodyInfos.Count;
    }

    public float GetTotalHeight() {
        float totalHeight = 0;
        foreach (var bodyInfo in allBodyInfos) {
            totalHeight += bodyInfo.HeightTag.Height;
        }
        return totalHeight;
    }

    
    private AlienBodyInfo() {

    }
    private AlienBodyInfo(AlienVoiceType voiceType, AlienBodyPartInfo headInfo, AlienBodyPartInfo mainBodyPartInfo, AlienBodyPartInfo legInfo) {
        HeadInfo = headInfo;
        MainBodyInfo = mainBodyPartInfo;
        LegInfo = legInfo;
        VoiceType = voiceType;
        allBodyInfos = new List<AlienBodyPartInfo>() {headInfo, mainBodyPartInfo, legInfo};
    }

    public static AlienBodyInfo GetRandomAlienInfo() {
        if (!instancesCreated) { 
            CreateAllInstances();
        }

        AlienVoiceType[] voiceValues = (AlienVoiceType[]) Enum.GetValues(typeof(AlienVoiceType));
        return new AlienBodyInfo(voiceValues[Random.Range(0, voiceValues.Length)],
            headBodyPartInstances[Random.Range(0, headBodyPartInstances.Count)],
            mainBodyPartInstances[Random.Range(0, mainBodyPartInstances.Count)],
            legBodyPartInstances[Random.Range(0, legBodyPartInstances.Count)]);
    }

}
