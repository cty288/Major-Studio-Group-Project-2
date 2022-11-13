using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_MiddleScore : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new HairCutTagMiddleScore() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
