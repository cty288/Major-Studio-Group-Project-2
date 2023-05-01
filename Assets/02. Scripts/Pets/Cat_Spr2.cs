using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Cat_Spr2 : HeadBodyPartInfo
{
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
        new CatTag_2(),
        new DistinctiveTag()
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}