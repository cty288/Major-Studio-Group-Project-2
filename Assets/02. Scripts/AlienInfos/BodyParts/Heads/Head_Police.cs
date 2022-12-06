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
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "Police"
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Police"
            };
        }
    }
}
