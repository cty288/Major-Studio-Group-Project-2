using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Sweater : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagSweater (), new FatTag(FatType.Fat) };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
