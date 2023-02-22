using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_BuzzCut_Old : AlienBodyPartInfo
{
    public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() { new HairCutTagBuzzCut() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
