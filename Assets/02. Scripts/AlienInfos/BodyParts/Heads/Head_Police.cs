using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Police : AlienBodyPartInfo {
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag> {
        new PoliceHairTag()
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}

public class PoliceHairTag : HaircutTag {
    public override string GetRandomRadioDescription(out bool isReal) {
        isReal = true;
        return "Police Hair";
    }

    public override string GetRandomRadioDescription(bool isReal) {
        return "Police Hair";
    }

    public override List<string> GetShortDescriptions() {
        return new List<string> {
            "police hair"
        };
    }
    

    public override string TagName { get; } = "Police Hair";
}
