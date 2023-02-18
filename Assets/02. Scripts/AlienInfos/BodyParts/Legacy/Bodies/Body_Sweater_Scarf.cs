using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Body_Sweater_Scarf : AlienBodyPartInfo
{
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() { new ClothTagSweater(), new ClothTagScarf(), new FatTag(FatType.Thin), new DistinctiveTag() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
