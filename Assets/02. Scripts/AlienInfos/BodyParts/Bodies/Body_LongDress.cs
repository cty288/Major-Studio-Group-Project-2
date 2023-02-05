using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Body_LongDress : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() {
        new ClothTagLongDress(), new FatTag(FatType.Thin),
        new DistinctiveTag()
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
