using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs1Long : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { };//height long
    public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}

