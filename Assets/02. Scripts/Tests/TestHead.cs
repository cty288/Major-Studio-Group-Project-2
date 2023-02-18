using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHead : AlienBodyPartInfo {
    
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>();
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
