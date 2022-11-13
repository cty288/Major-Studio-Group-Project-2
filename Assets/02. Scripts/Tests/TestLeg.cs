using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LegPartInfo : AlienBodyPartInfo {

}
public class TestLeg : LegPartInfo {
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() {};
    public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
