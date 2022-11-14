using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_CottonVest_Watch : AlienBodyPartInfo
{
    public override List<IAlienTag> Tags { get; } = new List<IAlienTag>() {
        new ClothTagCottonVest(),
        new ClothTagWatch(),
        new FatTag(FatType.Fat)
    };
    public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
