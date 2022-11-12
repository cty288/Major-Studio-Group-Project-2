using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBody : AlienBodyPartInfo {
    public override IFatTag FatTag { get; } = new FatTag(1f);
    public override IHeightTag HeightTag { get; } = new HeightTag(0.5f);
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() {new TestClothTag()};
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;

  
}


