using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_BuzzCut : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new HairCutTagBuzzCut() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
