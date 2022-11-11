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
    public AlienBodyPartInfo HeadInfo;
    public AlienBodyPartInfo MainBodyInfo;
    public AlienBodyPartInfo LegInfo;
    
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
    
    private AlienBodyInfo() {

    }
    private AlienBodyInfo(AlienVoiceType voiceType, AlienBodyPartInfo headInfo, AlienBodyPartInfo mainBodyPartInfo, AlienBodyPartInfo legInfo) {
        HeadInfo = headInfo;
        MainBodyInfo = mainBodyPartInfo;
        LegInfo = legInfo;
        VoiceType = voiceType;
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
