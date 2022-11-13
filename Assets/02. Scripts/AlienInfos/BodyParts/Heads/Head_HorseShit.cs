using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_HorseShit : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new HairCutTagHorseShit() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
