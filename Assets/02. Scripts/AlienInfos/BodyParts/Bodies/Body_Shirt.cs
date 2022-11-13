using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Shirt : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() { new ClothTagShirt(), new FatTag(FatType.Thin) };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
