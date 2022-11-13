using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs_SortPants : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagShortPants() };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
