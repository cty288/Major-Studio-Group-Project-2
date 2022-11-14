using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_PonyTail : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new HairCutTagPonyTail() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
