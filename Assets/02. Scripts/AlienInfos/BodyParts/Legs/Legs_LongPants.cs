using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs_LongPants : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagLongPants() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
