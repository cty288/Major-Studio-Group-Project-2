using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_LongBlackHair : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new HairCutTagLongBlackHair() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
