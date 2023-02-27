using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Legs_GreyTrousers : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		//new DistinctiveTag(),
		new LegsTag_GreyTrousers()
	};

	public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}