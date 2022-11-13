using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs1Short : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { };//height short
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
