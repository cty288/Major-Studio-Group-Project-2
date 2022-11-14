using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Sweater_Scarf : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagSweater(), new ClothTagScarf(), new FatTag(FatType.Thin) };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
