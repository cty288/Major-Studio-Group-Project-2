using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Legs_ShortSkirt_Legacy : AlienBodyPartInfo
{
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
        new ClothTagShortSkirt(),
        new DistinctiveTag()
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
