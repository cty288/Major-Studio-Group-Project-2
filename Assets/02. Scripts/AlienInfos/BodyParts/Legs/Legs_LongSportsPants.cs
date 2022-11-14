using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs_LongSportsPants : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagLongSportsPants() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
