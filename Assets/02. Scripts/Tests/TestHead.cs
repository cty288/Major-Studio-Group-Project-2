using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHead : AlienBodyPartInfo {

    public override IFatTag FatTag { get; } = new FatTag(0.3f);
    public override IHeightTag HeightTag { get; } = new HeightTag(1f);
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>();
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
