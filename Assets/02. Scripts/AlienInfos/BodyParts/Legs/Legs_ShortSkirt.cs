using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs_ShortSkirt : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagShortSkirt() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
