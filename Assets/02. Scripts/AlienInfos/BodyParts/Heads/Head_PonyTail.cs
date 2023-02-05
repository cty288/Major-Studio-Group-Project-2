using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Head_PonyTail : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() {
        new HairCutTagPonyTail(),
        new DistinctiveTag()
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
