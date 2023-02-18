using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBody : AlienBodyPartInfo {
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {new FatTag(FatType.Fat)};
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;

  
}


